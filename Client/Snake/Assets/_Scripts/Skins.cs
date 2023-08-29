using UnityEngine;

public class Skins : MonoBehaviour {
    public static Skins Instance;

    [SerializeField] private Material[] _materials;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public byte GetLength() => (byte) _materials.Length;
    
    public Material GetMaterial(byte index) => _materials[index];
    
}