using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    public static PlayerInput Instance { get; private set; }

    private const string PLAYER_INPUT_KEY = "PlayerInput";

    /**
     * InputActions资产自动生成的脚本
     */
    private PlayerInputActions playerInputActions;

    public enum InputKey {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
    }

    public event Action InteractAction;
    public event Action InteractAlternateAction;
    public event Action PauseAction;

    private void Awake() {
        Instance = this;
        
        playerInputActions = new PlayerInputActions();
        if (PlayerPrefs.HasKey(PLAYER_INPUT_KEY)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_INPUT_KEY));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += OnInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed += OnPausePerformed;
    }

    private void OnDestroy() {
        playerInputActions.Player.Interact.performed -= OnInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= OnInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed -= OnPausePerformed;

        playerInputActions.Dispose();
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        InteractAction?.Invoke();
    }

    private void OnInteractAlternatePerformed(InputAction.CallbackContext obj) {
        InteractAlternateAction?.Invoke();
    }

    private void OnPausePerformed(InputAction.CallbackContext obj) {
        PauseAction?.Invoke();
    }

    public Vector2 GetMovementVector2Normalized() {
        var inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }

    public string GetBindingInputKeyText(InputKey inputKey) {
        return inputKey switch {
            InputKey.MoveUp => playerInputActions.Player.Movement.bindings[1].ToDisplayString(),
            InputKey.MoveDown => playerInputActions.Player.Movement.bindings[2].ToDisplayString(),
            InputKey.MoveLeft => playerInputActions.Player.Movement.bindings[3].ToDisplayString(),
            InputKey.MoveRight => playerInputActions.Player.Movement.bindings[4].ToDisplayString(),
            InputKey.Interact => playerInputActions.Player.Interact.bindings[0].ToDisplayString(),
            InputKey.InteractAlternate => playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString(),
            InputKey.Pause => playerInputActions.Player.Pause.bindings[0].ToDisplayString(),
            _ => throw new ArgumentOutOfRangeException(nameof(inputKey), inputKey, null)
        };
    }

    public void RebindInputKey(InputKey inputKey, Action rebindCall) {
        InputAction inputAction;
        int bindingIndex;
        switch (inputKey) {
            case InputKey.MoveUp:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 1;
                break;

            case InputKey.MoveDown:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 2;
                break;

            case InputKey.MoveLeft:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 3;
                break;

            case InputKey.MoveRight:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 4;
                break;

            case InputKey.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;

            case InputKey.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;

            case InputKey.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(inputKey), inputKey, null);
        }

        playerInputActions.Player.Disable();
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                Debug.Log("Rebind inputKey: " + inputKey +
                          ", old key: " + callback.action.bindings[bindingIndex].path +
                          ", new key: " + callback.action.bindings[bindingIndex].overridePath);
                playerInputActions.Player.Enable();
                callback.Dispose();
                rebindCall?.Invoke();

                PlayerPrefs.SetString(PLAYER_INPUT_KEY, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}