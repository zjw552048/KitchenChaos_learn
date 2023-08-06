using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button quickJoinBtn;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private Button codeJoinBtn;

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        playerNameInputField.text = MultiplayerNetworkManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener(newPlayerName => {
            MultiplayerNetworkManager.Instance.SetPlayerName(newPlayerName);
        });

        createLobbyBtn.onClick.AddListener(() => { lobbyCreateUI.Show(); });

        quickJoinBtn.onClick.AddListener(() => { GameLobbyManager.Instance.QuickJoin(); });

        codeJoinBtn.onClick.AddListener(() => {
            var code = lobbyCodeInputField.text;
            GameLobbyManager.Instance.JoinByCode(code);
        });
    }
}