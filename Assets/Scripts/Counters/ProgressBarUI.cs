using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
    [SerializeField] private Image cuttingProgressBarImage;
    [SerializeField] private GameObject hasProgressGameObject;

    private IHasProgress hasProgress;

    private void Awake() {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
    }

    private void Start() {
        hasProgress.RefreshProgressAction += OnRefreshProgressAction;

        cuttingProgressBarImage.fillAmount = 0f;

        Hide();
    }

    private void OnRefreshProgressAction(float progress) {
        cuttingProgressBarImage.fillAmount = progress;
        if (progress == 0.0f || Math.Abs(progress - 1.0f) < float.Epsilon) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}