using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    [SerializeField] private Snake _snake;
    [SerializeField] private Transform _cursor;
    private Camera _cameraMain;
    private Plane _plane;
    private void Awake() {
        _cameraMain = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
    }
    private void Update() {
        if(Input.GetMouseButton(0)){
            MoveCursor();
            _snake.LookAt(_cursor.position);
        }
    }
    private void MoveCursor() {
        Ray ray = _cameraMain.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        Vector3 point = ray.GetPoint(distance);

        _cursor.position = point;
    }
}
