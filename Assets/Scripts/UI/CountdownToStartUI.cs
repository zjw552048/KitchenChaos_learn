using TMPro;
using UnityEngine;

public class CountdownToStartUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start() {
        MainGameManager.Instance.OnGameStateChangedAction += OnGameStateChangedAction;

        Hide();
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsCountDownToStartState()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        countdownText.text = MainGameManager.Instance.GetCountdownToStartTimer().ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}