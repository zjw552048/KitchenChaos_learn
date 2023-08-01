using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsUI : MonoBehaviour {
    [SerializeField] private Button soundVolumeBtn;
    [SerializeField] private Button musicVolumeBtn;
    [SerializeField] private Button closeOptionsBtn;
    [SerializeField] private TextMeshProUGUI soundVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    private void Start() {
        soundVolumeBtn.onClick.AddListener(() => {
            SoundManager.Instance.ModifyVolume();
            UpdateVisual();
        });
        musicVolumeBtn.onClick.AddListener(() => {
            MusicManager.Instance.ModifyVolume();
            UpdateVisual();
        });
        closeOptionsBtn.onClick.AddListener(Hide);

        MainGameManager.Instance.OnGameUnpausedAction += OnGameUnpausedAction;

        UpdateVisual();
        Hide();
    }

    private void OnGameUnpausedAction() {
        Hide();
    }

    private void UpdateVisual() {
        soundVolumeText.text = "Sound Volume: " + Mathf.RoundToInt(SoundManager.Instance.GetVolume() * 10);
        musicVolumeText.text = "Music Volume: " + Mathf.RoundToInt(MusicManager.Instance.GetVolume() * 10);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}