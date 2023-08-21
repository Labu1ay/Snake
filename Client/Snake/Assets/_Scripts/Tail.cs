using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour {
    [SerializeField] private Transform _head;
    [SerializeField] private List<Transform> _details;
    [SerializeField] private float _detailDistance = 1f;
    private List<Vector3> _positionHistory = new List<Vector3>();
    private void Awake() {
        _positionHistory.Add(_head.position);

        for (int i = 0; i < _details.Count; i++) {
            _positionHistory.Add(_details[i].position); 
        }
    }
    private void Update() {
        float distance = (_head.position - _positionHistory[0]).magnitude;
        while(distance > _detailDistance){
            Vector3 direction = (_head.position - _positionHistory[0]).normalized;

            _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
            _positionHistory.RemoveAt(_positionHistory.Count - 1);

            distance -= _detailDistance;

        }
    }
}
