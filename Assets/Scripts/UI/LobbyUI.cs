using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button quickJoinBtn;

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        createLobbyBtn.onClick.AddListener(() => { GameLobbyManager.Instance.CreateLobby("TestLobby", false); });

        quickJoinBtn.onClick.AddListener(() => { GameLobbyManager.Instance.QuickJoin(); });
    }
}