using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private float interactiveDistance = 2f;
    [SerializeField] private LayerMask interactiveLayerMask;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private PlayerInput playerInput;
    private Transform playTransform;

    private bool isWalking;

    private void Awake() {
        playTransform = transform;
    }

    private void Update() {
        HandleMovement();
        HandleInteractive();
    }

    private void HandleInteractive() {
        if (!Physics.Raycast(
                playTransform.position,
                playTransform.forward,
                out var hitInfo,
                interactiveDistance,
                interactiveLayerMask)) {
            return;
        }

        if (!hitInfo.transform.TryGetComponent(out ClearCounter counter)) {
            return;
        }

        counter.Interactive();
    }

    private void HandleMovement() {
        var inputVector = playerInput.GetMovementVector2Normalized();

        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        var moveDistance = moveSpeed * Time.deltaTime;

        // 更新角度
        playTransform.forward = Vector3.Slerp(playTransform.forward, moveDir, rotateSpeed * Time.deltaTime);

        // check move on x and y dir
        var playerPos = playTransform.position;
        var canMove = !Physics.CapsuleCast(
            playerPos,
            playerPos + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance);
        if (!canMove) {
            // try move on x dir
            var moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = !Physics.CapsuleCast(
                playerPos,
                playerPos + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance);
            if (canMove) {
                moveDir = moveDirX;
            } else {
                // try move on y dir
                var moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = !Physics.CapsuleCast(
                    playerPos,
                    playerPos + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance);
                if (canMove) {
                    moveDir = moveDirZ;
                } else {
                    // totally can not move
                }
            }
        }

        if (canMove) {
            playTransform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
    }

    public bool IsWalking() {
        return isWalking;
    }
}