using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour {
    [SerializeField] private Transform _cursor;
    [SerializeField] private float _cameraOffsetY = 15f;
    private MultiplayerManager _multiplayerManager;
    private PlayerAim _playerAim;
    private Player _player;
    private string _clientID;
    private Camera _cameraMain;                
    private Snake _snake;
    private Plane _plane;
    
    public void Init(string clientID, PlayerAim aim, Player player, Snake snake) {
        _multiplayerManager = MultiplayerManager.Instance;
        _playerAim = aim;
        _player = player;
        _clientID = clientID;
        _snake = snake;
        
        _cameraMain = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
        
        _cameraMain.transform.parent = _snake.transform;
        _cameraMain.transform.localPosition = Vector3.up * _cameraOffsetY;

        _player.OnChange += OnChange;
    }
    private void Update() {
        if(Input.GetMouseButton(0)){
            MoveCursor();
            _playerAim.SetTargetDirection(_cursor.position);
        }

        SendMove();
    }
    
    private void SendMove() {
        _playerAim.GetMoveInfo(out Vector3 position);
        Dictionary<string, object> data = new Dictionary<string, object>() {
            { "x", position.x },
            { "z", position.z }
        };
        _multiplayerManager.SendMessage("move", data);
    }

    private void MoveCursor() {
        Ray ray = _cameraMain.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        Vector3 point = ray.GetPoint(distance);

        _cursor.position = point;
    }
    
    private void OnChange(List<DataChange> changes) {
        Vector3 position = _snake.transform.position;
        for (int i = 0; i < changes.Count; i++) {
            switch (changes[i].Field) {
                case "x":
                    position.x = (float)changes[i].Value;
                    break;
                case "z":
                    position.z = (float)changes[i].Value;
                    break;
                case "d": _snake.SetDetailCount((byte)changes[i].Value);
                    break;
                default:
                    Debug.LogWarning("Не обрабатывается изменениe поля :" + changes[i].Field);
                    break;
            }
        }
        _snake.SetRotation(position);
    }

    public void Destroy() {
        _cameraMain.transform.parent = null;
        
        if(_player != null) _player.OnChange -= OnChange;
        _snake.Destroy(_clientID);
        Destroy(gameObject);
    }
}
