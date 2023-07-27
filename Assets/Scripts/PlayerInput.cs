using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    private PlayerInputActions playerInputActions;

    public event Action InteractAction;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed+= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        InteractAction?.Invoke();
    }

    public Vector2 GetMovementVector2Normalized() {
        var inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }
}