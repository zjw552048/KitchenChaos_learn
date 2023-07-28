using System;
using UnityEngine;
using UnityEngine.UI;

public class CuttingProgressBarUI : MonoBehaviour {
    [SerializeField] private GameObject uiParent;
    [SerializeField] private Image cuttingProgressBarImage;

    private CuttingCounter counter;

    private void Awake() {
        counter = GetComponent<CuttingCounter>();
    }

    private void Start() {
        counter.RefreshCuttingProgressAction += OnRefreshCuttingProgressAction;

        cuttingProgressBarImage.fillAmount = 0f;

        Hide();
    }

    private void OnRefreshCuttingProgressAction(float progress) {
        cuttingProgressBarImage.fillAmount = progress;
        if (progress == 0.0f || Math.Abs(progress - 1.0f) < float.Epsilon) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        uiParent.SetActive(true);
    }

    private void Hide() {
        uiParent.SetActive(false);
    }
}