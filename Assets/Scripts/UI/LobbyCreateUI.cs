using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createPrivateBtn;
    [SerializeField] private Button createPublicBtn;
    [SerializeField] private Button closeBtn;

    private void Start() {
        createPrivateBtn.onClick.AddListener(() => {
            var lobbyName = lobbyNameInputField.text;
            GameLobbyManager.Instance.CreateLobby(lobbyName, true);
        });
        createPublicBtn.onClick.AddListener(() => {
            var lobbyName = lobbyNameInputField.text;
            GameLobbyManager.Instance.CreateLobby(lobbyName, false);
        });
        
        closeBtn.onClick.AddListener(() => {
            Hide();
        });
        
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}