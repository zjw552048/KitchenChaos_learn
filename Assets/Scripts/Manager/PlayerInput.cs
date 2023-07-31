using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    public static PlayerInput Instance { get; private set; }

    /**
     * InputActions资产自动生成的脚本
     */
    private PlayerInputActions playerInputActions;

    public event Action InteractAction;
    public event Action InteractAlternateAction;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += OnInteractAlternatePerformed;

        Instance = this;
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        InteractAction?.Invoke();
    }
    
    private void OnInteractAlternatePerformed(InputAction.CallbackContext obj) {
        InteractAlternateAction?.Invoke();
    }

    public Vector2 GetMovementVector2Normalized() {
        var inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }
}