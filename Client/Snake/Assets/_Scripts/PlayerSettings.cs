using UnityEngine;

public class PlayerSettings : MonoBehaviour {
    public static PlayerSettings Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;
    }

    public string Login { get; set; }

    public void SetLogin(string login) {
        Login = login;
    }
}
