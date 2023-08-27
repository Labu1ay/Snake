using System;
using UnityEngine;

public class Skins : MonoBehaviour {
    public static Skins Instance;

    [SerializeField] private Material[] _Materials;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public Material GetMaterial(byte index) => _Materials[index];
    
}