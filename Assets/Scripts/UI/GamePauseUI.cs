using Unity.Netcode;
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
        mainMenuBtn.onClick.AddListener(() => {
            SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene);
        });

        MainGameManager.Instance.LocalGamePausedAction += OnLocalGamePausedAction;
        MainGameManager.Instance.LocalGameUnpausedAction += OnLocalGameUnpausedAction;

        Hide();
    }

    private void OnLocalGamePausedAction() {
        Show();
    }

    private void OnLocalGameUnpausedAction() {
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