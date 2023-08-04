using UnityEngine;

public class GameMultiplayerPauseUI : MonoBehaviour {
    private void Start() {
        MainGameManager.Instance.MultiplayerGamePausedAction += OnMultiplayerGamePausedAction;
        MainGameManager.Instance.MultiplayerGameUnpausedAction += OnMultiplayerGameUnpausedAction;
        Hide();
    }

    private void OnMultiplayerGamePausedAction() {
        Show();
    }

    private void OnMultiplayerGameUnpausedAction() {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}