using UnityEngine;
using UnityEngine.UI;

public class GamePlayingTimerUI : MonoBehaviour {
    [SerializeField] private Image timerImage;

    private void Start() {
        MainGameManager.Instance.GameStateChangedAction += OnGameStateChangedAction;
        Hide();
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsGamePlayingState()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        timerImage.fillAmount = MainGameManager.Instance.GetGamePlayingTimerNormalized();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}