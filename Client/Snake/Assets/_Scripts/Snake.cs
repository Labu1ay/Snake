using UnityEngine;
using UnityEngine.Serialization;

public class Snake : MonoBehaviour {
    public float Speed {
        get { return _speed; }
    }

    [SerializeField] private int _playerLayer = 6;
    [SerializeField] private Tail _tailPrefab;
    [field:SerializeField] public Transform Head { get; private set; }
    [SerializeField] private float _speed = 2f;
    
    [SerializeField] private Transform _nicknameTargetPoint;
    [SerializeField] private Nickname _nicknamePrefab;
    private Nickname _nickname;

    private Tail _tail;

    public void Init(int detailCount, byte color,string playerLogin, bool isPlayer = false) {
        if (isPlayer) {
            gameObject.layer = _playerLayer;
            
            var childrens = GetComponentsInChildren<Transform>();
            foreach (var children in childrens) {
                children.gameObject.layer = _playerLayer;
            }
        }
        
        _nickname = Instantiate(_nicknamePrefab, _nicknamePrefab.transform.position, Quaternion.identity);
        _nickname.Init(_nicknameTargetPoint, playerLogin);
        
        GetComponent<SetSkin>().SetMaterial(color);
        
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(Head, _speed, detailCount, color, _playerLayer, isPlayer);
    }

    public void SetDetailCount(int detailCount) => _tail.SetDetailCount(detailCount);
    
    public void Destroy(string clientID) {
        var detailPositions = _tail.GetDetailPositions();

        detailPositions.id = clientID; 
        
        string json = JsonUtility.ToJson(detailPositions);
        MultiplayerManager.Instance.SendMessage("gameOver", json);
        
        _tail.Destroy();
        _nickname.Destroy();
        Destroy(gameObject);
    }

    private void Update() {
        Move();
    }

    private void Move() {
        transform.position += Head.forward * Time.deltaTime * _speed;
    }
    
    public void SetRotation(Vector3 pointToLook) {
        Head.LookAt(pointToLook);
    }
}
