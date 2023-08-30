using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class Apple : MonoBehaviour {
    private Vector2Float _apple;
    public void Init(Vector2Float apple) {
        _apple = apple;
        _apple.OnChange += OnChange;
    }

    private void OnChange(List<DataChange> changes) {
        
        Vector3 position = transform.position;
        foreach (var change in changes) {
            switch (change.Field) {
                case "x":
                    position.x = (float)change.Value;
                    break;
                case "z":
                    position.z = (float)change.Value;
                    break;
                default:
                    Debug.LogWarning("Яблоко не реагирует на изменения поля :" + change.Field);
                    break;
            }
        }

        Debug.Log($"Яблоко переместилось с позиции {transform.position} в {position}");
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Collect() {
        Dictionary<string, object> data = new Dictionary<string, object>() {
            {"id", _apple.id}
        };
        MultiplayerManager.Instance.SendMessage("collect", data);

        gameObject.SetActive(false);
    }

    public void Destroy() {
        if(_apple != null) _apple.OnChange -= OnChange;
        Destroy(gameObject);
    }
    
}