using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private PlayerInput playerInput;
    private Transform playTransform;

    private bool isWalking;

    private void Awake() {
        playTransform = transform;
    }

    private void Update() {
        var inputVector = playerInput.GetMovementVector2Normalized();
        
        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        playTransform.position += moveDir * moveSpeed * Time.deltaTime;
        playTransform.forward = Vector3.Slerp(playTransform.forward, moveDir, rotateSpeed * Time.deltaTime);

        isWalking = moveDir != Vector3.zero;
    }

    public bool IsWalking() {
        return isWalking;
    }
}