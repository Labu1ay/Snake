using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {
    [SerializeField] private Tail _tailPrefab;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _directionPoint;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _rotateSpeed = 90f;

    private Tail _tail;

    public void Init(int detailCount) {
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(_head, _speed, detailCount);
    }

    public void Destroy() {
        _tail.Destroy();
        Destroy(gameObject);
    }
    
    private void Update() {
        Rotate();
        Move();
    }
    private void Rotate(){
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        _head.rotation = Quaternion.RotateTowards(_head.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
        /* float diffY = _directionPoint.eulerAngles.y - _head.eulerAngles.y;

        if(diffY > 180) diffY = (diffY - 180) * -1;
        else if(diffY < -180) diffY = (diffY + 180) * -1;

        float maxAngle = Time.deltaTime * _rotateSpeed;
        float rotateY = Mathf.Clamp(diffY, -maxAngle, maxAngle);

        _head.Rotate(0, rotateY, 0); */
    }
    private void Move(){
        transform.position += _head.forward * Time.deltaTime * _speed;
    }
    private Vector3 _targetDirection = Vector3.zero;
    public void LookAt(Vector3 cursorPosition){
        _targetDirection = cursorPosition - _head.position;
        //_directionPoint.LookAt(cursorPosition);
    }
}
