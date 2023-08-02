using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsUI : MonoBehaviour {
    [SerializeField] private GamePauseUI gamePauseUI;
    [SerializeField] private Button soundVolumeBtn;
    [SerializeField] private Button musicVolumeBtn;
    [SerializeField] private Button closeOptionsBtn;
    [SerializeField] private TextMeshProUGUI soundVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    [SerializeField] private Button moveUpBtn;
    [SerializeField] private Button moveDownBtn;
    [SerializeField] private Button moveLeftBtn;
    [SerializeField] private Button moveRightBtn;
    [SerializeField] private Button interactBtn;
    [SerializeField] private Button interactAlternateBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button gamepadInteractBtn;
    [SerializeField] private Button gamepadInteractAlternateBtn;
    [SerializeField] private Button gamepadPauseBtn;
    [SerializeField] private TextMeshProUGUI moveUpBtnText;
    [SerializeField] private TextMeshProUGUI moveDownBtnText;
    [SerializeField] private TextMeshProUGUI moveLeftBtnText;
    [SerializeField] private TextMeshProUGUI moveRightBtnText;
    [SerializeField] private TextMeshProUGUI interactBtnText;
    [SerializeField] private TextMeshProUGUI interactAlternateBtnText;
    [SerializeField] private TextMeshProUGUI pauseBtnText;
    [SerializeField] private TextMeshProUGUI gamepadInteractBtnText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateBtnText;
    [SerializeField] private TextMeshProUGUI gamepadPauseBtnText;

    [SerializeField] private GameObject rebindInputKeyTipsGameObject;

    private void Start() {
        soundVolumeBtn.onClick.AddListener(() => {
            SoundManager.Instance.ModifyVolume();
            UpdateVisual();
        });
        musicVolumeBtn.onClick.AddListener(() => {
            MusicManager.Instance.ModifyVolume();
            UpdateVisual();
        });
        closeOptionsBtn.onClick.AddListener(() => {
            gamePauseUI.Show();
            Hide();
        });

        moveUpBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.MoveUp); });
        moveDownBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.MoveDown); });
        moveLeftBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.MoveLeft); });
        moveRightBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.MoveRight); });
        interactBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.Interact); });
        interactAlternateBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.InteractAlternate); });
        pauseBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.Pause); });
        gamepadInteractBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.GamepadInteract); });
        gamepadInteractAlternateBtn.onClick.AddListener(() => {
            ClickRebindKey(PlayerInput.InputKey.GamepadInteractAlternate);
        });
        gamepadPauseBtn.onClick.AddListener(() => { ClickRebindKey(PlayerInput.InputKey.GamepadPause); });

        MainGameManager.Instance.GameUnpausedAction += OnGameUnpausedAction;
        PlayerInput.Instance.InputKeyRebindAction += OnInputKeyRebindAction;

        UpdateVisual();
        Hide();
        HideRebindTips();
    }

    private void OnGameUnpausedAction() {
        Hide();
    }

    private void UpdateVisual() {
        soundVolumeText.text = "Sound Volume: " + Mathf.RoundToInt(SoundManager.Instance.GetVolume() * 10);
        musicVolumeText.text = "Music Volume: " + Mathf.RoundToInt(MusicManager.Instance.GetVolume() * 10);

        moveUpBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveUp);
        moveDownBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveDown);
        moveLeftBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveLeft);
        moveRightBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveRight);
        interactBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.Interact);
        interactAlternateBtnText.text =
            PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.InteractAlternate);
        pauseBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.Pause);
        gamepadInteractBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadInteract);
        gamepadInteractAlternateBtnText.text =
            PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadInteractAlternate);
        gamepadPauseBtnText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadPause);
    }

    public void Show() {
        gameObject.SetActive(true);

        soundVolumeBtn.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void ShowRebindTips() {
        rebindInputKeyTipsGameObject.SetActive(true);
    }

    private void HideRebindTips() {
        rebindInputKeyTipsGameObject.SetActive(false);
    }

    private void ClickRebindKey(PlayerInput.InputKey inputKey) {
        ShowRebindTips();
        PlayerInput.Instance.RebindInputKey(inputKey);
    }

    private void OnInputKeyRebindAction() {
        UpdateVisual();
        HideRebindTips();
    }
}