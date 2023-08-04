using TMPro;
using UnityEngine;

public class GameTutorialUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI moveUpKeyText;
    [SerializeField] private TextMeshProUGUI moveDownKeyText;
    [SerializeField] private TextMeshProUGUI moveLeftKeyText;
    [SerializeField] private TextMeshProUGUI moveRightKeyText;
    [SerializeField] private TextMeshProUGUI interactKeyText;
    [SerializeField] private TextMeshProUGUI interactAlternateKeyText;
    [SerializeField] private TextMeshProUGUI pauseKeyText;
    [SerializeField] private TextMeshProUGUI gamepadInteractKeyText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateKeyText;
    [SerializeField] private TextMeshProUGUI gamepadPauseKeyText;


    private void Start() {
        PlayerInput.Instance.InputKeyRebindAction += InstanceOnInputKeyRebindAction;
        MainGameManager.Instance.LocalPlayerReadyChangedAction += OnLocalPlayerReadyChangedAction;

        UpdateVisual();
        Show();
    }

    private void OnLocalPlayerReadyChangedAction() {
        if (MainGameManager.Instance.IsLocalPlayerReady()) {
            Hide();
        }
    }


    private void UpdateVisual() {
        moveUpKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveUp);
        moveDownKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveDown);
        moveLeftKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveLeft);
        moveRightKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.MoveRight);
        interactKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.Interact);
        interactAlternateKeyText.text =
            PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.InteractAlternate);
        pauseKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.Pause);
        gamepadInteractKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadInteract);
        gamepadInteractAlternateKeyText.text =
            PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadInteractAlternate);
        gamepadPauseKeyText.text = PlayerInput.Instance.GetBindingInputKeyText(PlayerInput.InputKey.GamepadPause);
    }

    private void InstanceOnInputKeyRebindAction() {
        UpdateVisual();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}