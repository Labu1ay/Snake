using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour {
    public void InputLogin(string login) {
        PlayerSettings.Instance.SetLogin(login);
    }

    public void ClickConnect() {
        if (string.IsNullOrEmpty(PlayerSettings.Instance.Login))
            PlayerSettings.Instance.Login = $"User{Random.Range(100, 1000)}";

        SceneManager.LoadScene("Game");
    }
}
