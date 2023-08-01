using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button mainMenuBtn;

    private void Start() {
        resumeBtn.onClick.AddListener(() => { MainGameManager.Instance.TogglePauseGame(); });
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        MainGameManager.Instance.OnGamePausedAction += OnGamePausedAction;
        MainGameManager.Instance.OnGameUnpausedAction += OnGameUnpausedAction;

        Hide();
    }

    private void OnGamePausedAction() {
        Show();
    }

    private void OnGameUnpausedAction() {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}