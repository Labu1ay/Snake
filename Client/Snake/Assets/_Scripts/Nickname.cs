using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour {
    private Transform _targetPosition;
    [SerializeField] private Text _nickname;
    
    
    public void Init(Transform position, string nickname) {
        _targetPosition = position;
        _nickname.text = nickname;
    }

    private void LateUpdate() => transform.position = _targetPosition.position;

    public void Destroy() {
        Destroy(gameObject);
    }
    
}
