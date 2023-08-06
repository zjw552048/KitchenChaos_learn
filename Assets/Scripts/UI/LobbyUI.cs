using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button quickJoinBtn;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private Button codeJoinBtn;

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        createLobbyBtn.onClick.AddListener(() => { lobbyCreateUI.Show(); });

        quickJoinBtn.onClick.AddListener(() => { GameLobbyManager.Instance.QuickJoin(); });

        codeJoinBtn.onClick.AddListener(() => {
            var code = lobbyCodeInputField.text;
            GameLobbyManager.Instance.JoinByCode(code);
        });
    }
}