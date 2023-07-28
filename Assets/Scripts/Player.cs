using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    public static Player Instance { get; private set; }

    [Header("速度参数")] [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("体积参数")] [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;

    [Header("交互参数")] [SerializeField] private float interactiveDistance = 2f;
    [SerializeField] private LayerMask interactiveLayerMask;

    [Header("持有物品点")] [SerializeField] private Transform kitchenObjectHoldPoint;

    private Transform playTransform;

    private bool isWalking;

    private BaseCounter selectedCounter;

    private KitchenObject holdKitchenObject;

    public event Action<BaseCounter> SelectedCounterChangedAction;

    private void Awake() {
        Instance = this;
        playTransform = transform;
    }

    private void Start() {
        PlayerInput.Instance.InteractAction += PlayerInputOnInteractAction;
    }

    private void OnDestroy() {
        PlayerInput.Instance.InteractAction -= PlayerInputOnInteractAction;
    }

    private void PlayerInputOnInteractAction() {
        if (selectedCounter == null) {
            return;
        }

        selectedCounter.Interact(this);
    }

    private void Update() {
        HandleMovement();
        HandleInteract();
    }

    private void HandleMovement() {
        var inputVector = PlayerInput.Instance.GetMovementVector2Normalized();

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
            canMove = moveDir.x != 0 &&
                      !Physics.CapsuleCast(
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
                canMove = moveDir.z != 0 &&
                          !Physics.CapsuleCast(
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

    private void HandleInteract() {
        if (!Physics.Raycast(
                playTransform.position,
                playTransform.forward,
                out var hitInfo,
                interactiveDistance,
                interactiveLayerMask)) {
            ChangeSelectedCounter(null);
            return;
        }

        if (!hitInfo.transform.TryGetComponent(out BaseCounter counter)) {
            ChangeSelectedCounter(null);
            return;
        }

        if (counter != null && counter == selectedCounter) {
            return;
        }

        ChangeSelectedCounter(counter);
    }

    private void ChangeSelectedCounter(BaseCounter counter) {
        if (selectedCounter != counter) {
            SelectedCounterChangedAction?.Invoke(counter);
        }

        selectedCounter = counter;
    }

    public bool IsWalking() {
        return isWalking;
    }

    #region IKitchenObjectParent实现

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject() {
        return holdKitchenObject;
    }

    public void SetKitChenObject(KitchenObject targetObject) {
        holdKitchenObject = targetObject;
    }

    public bool HasKitchenObject() {
        return holdKitchenObject != null;
    }

    #endregion
}