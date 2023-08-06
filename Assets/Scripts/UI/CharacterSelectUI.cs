using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button readyBtn;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });
        readyBtn.onClick.AddListener(() => { CharacterSelectReadyManager.Instance.SetPlayerReady(); });

        var lobby = GameLobbyManager.Instance.GetLobby();
        lobbyNameText.text = "LobbyName: " + lobby.Name;
        lobbyCodeText.text = "LobbyCode: " + lobby.LobbyCode;
    }
}