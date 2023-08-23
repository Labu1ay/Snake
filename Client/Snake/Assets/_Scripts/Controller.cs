using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    [SerializeField] private Transform _cursor;
    private MultiplayerManager _multiplayerManager;
    private Camera _cameraMain;                
    private Snake _snake;
    private Plane _plane;
    public void Init(Snake snake) {
        _multiplayerManager = MultiplayerManager.Instance;
        
        _snake = snake;
        _cameraMain = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
    }
    private void Update() {
        if(Input.GetMouseButton(0)){
            MoveCursor();
            _snake.LookAt(_cursor.position);
        }

        SendMove();
    }

    private void SendMove() {
        _snake.GetMoveInfo(out Vector3 position);
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
}
