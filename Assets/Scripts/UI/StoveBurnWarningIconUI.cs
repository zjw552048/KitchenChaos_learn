using UnityEngine;

public class StoveBurnWarningIconUI : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;

    private void Start() {
        stoveCounter.RefreshProgressAction += OnRefreshProgressAction;

        Hide();
    }

    private void OnRefreshProgressAction(float progress) {
        var show = stoveCounter.IsBurningState() && progress > StoveCounter.BURNED_WARNING_PROGRESS;
        if (show) {
            Show();
        } else {
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