using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playBtn;
    [SerializeField] private Button quitBtn;

    private void Start() {
        playBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.GameScene); });

        quitBtn.onClick.AddListener(Application.Quit);
    }
}