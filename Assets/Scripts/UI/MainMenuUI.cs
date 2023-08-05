using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playBtn;
    [SerializeField] private Button quitBtn;

    private void Start() {
        Time.timeScale = 1f;
        playBtn.onClick.AddListener(() => {
            SceneLoader.LoadScene(SceneLoader.SceneName.LobbyScene);
        });

        quitBtn.onClick.AddListener(Application.Quit);
    }
}