using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button multiplayerBtn;
    [SerializeField] private Button singlePlayerBtn;
    [SerializeField] private Button quitBtn;

    private void Start() {
        Time.timeScale = 1f;
        multiplayerBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.SinglePlayerMode = false;
            SceneLoader.LoadScene(SceneLoader.SceneName.LobbyScene);
        });
        singlePlayerBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.SinglePlayerMode = true;
            SceneLoader.LoadScene(SceneLoader.SceneName.LobbyScene);
        });

        quitBtn.onClick.AddListener(Application.Quit);
    }
}