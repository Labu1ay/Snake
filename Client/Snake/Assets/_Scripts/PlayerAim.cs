using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour {
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _overlapRadius = .5f;
    [SerializeField] private float _rotateSpeed = 90f;
    private Transform _snakeHead;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed;
    
    public void Init(Transform snakeHead, float speed) {
        _snakeHead = snakeHead;
        _speed = speed;
    }
    private void Update() {
        Rotate();
        Move();
    }

    private void FixedUpdate() {
        CheckCollision();
    }

    private void CheckCollision() {
        Collider[] colliders = Physics.OverlapSphere(_snakeHead.position, _overlapRadius, _collisionLayer);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.TryGetComponent(out Apple apple)) {
                apple.Collect();
            }
            else {
                if (colliders[i].GetComponentInParent<Snake>()) {
                    Transform enemy = colliders[i].transform;
                    float playerAngle =
                        Vector3.Angle(enemy.position - _snakeHead.position, transform.forward);
                    float enemyAngle = Vector3.Angle(_snakeHead.position - enemy.position,
                        enemy.forward);

                    if (playerAngle < enemyAngle + 5) GameOver();
                }
                else {
                    GameOver();
                }
                
            }
        }
    }

    private void GameOver() {
        FindObjectOfType<Controller>().Destroy();
        Destroy(gameObject);
    }

    private void Rotate() {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
    }
    
    private void Move() {
        transform.position += transform.forward * Time.deltaTime * _speed;
    }

    public void SetTargetDirection(Vector3 pointToLook) {
        _targetDirection = pointToLook - transform.position;
    }
    
    public void GetMoveInfo(out Vector3 position) {
        position = transform.position;
    }
}