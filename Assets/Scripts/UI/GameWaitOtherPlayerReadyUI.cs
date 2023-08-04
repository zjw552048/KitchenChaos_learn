using UnityEngine;

public class GameWaitOtherPlayerReadyUI : MonoBehaviour {
    private void Start() {
        MainGameManager.Instance.LocalPlayerReadyChangedAction += OnLocalPlayerReadyChangedAction;
        MainGameManager.Instance.GameStateChangedAction += OnGameStateChangedAction;

        Hide();
    }

    private void OnLocalPlayerReadyChangedAction() {
        if (MainGameManager.Instance.IsLocalPlayerReady()) {
            Show();
        }
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsCountDownToStartState()) {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}