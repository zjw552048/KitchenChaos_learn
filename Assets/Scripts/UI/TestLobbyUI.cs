using UnityEngine;
using UnityEngine.UI;

public class TestLobbyUI : MonoBehaviour {
    [SerializeField] private Button createBtn;
    [SerializeField] private Button joinBtn;

    private void Start() {
        createBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.Instance.StartHost();
            SceneLoader.LoadNetworkScene(SceneLoader.SceneName.CharacterSelectScene);
        });
        joinBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.Instance.StartClient();
        });
    }
}
