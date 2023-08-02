using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {
    [SerializeField] private GameOptionsUI gameOptionsUI;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button mainMenuBtn;

    private void Start() {
        optionsBtn.onClick.AddListener(() => {
            gameOptionsUI.Show();
            Hide();
        });
        resumeBtn.onClick.AddListener(() => { MainGameManager.Instance.TogglePauseGame(); });
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        MainGameManager.Instance.GamePausedAction += OnGamePausedAction;
        MainGameManager.Instance.GameUnpausedAction += OnGameUnpausedAction;

        Hide();
    }

    private void OnGamePausedAction() {
        Show();
    }

    private void OnGameUnpausedAction() {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        
        optionsBtn.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}