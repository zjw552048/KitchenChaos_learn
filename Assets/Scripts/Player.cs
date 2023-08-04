using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent {
    public static Player LocalInstance { get; private set; }

    [Header("出生点")] [SerializeField] private Vector3[] spawnPositionList;

    [Header("速度参数")] [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("体积参数")] [SerializeField] private float playerRadius = 0.7f;

    [Header("交互参数")] [SerializeField] private float interactiveDistance = 2f;
    [SerializeField] private LayerMask interactiveLayerMask;
    [SerializeField] private LayerMask moveColliderLayerMask;

    [Header("持有物品点")] [SerializeField] private Transform kitchenObjectHoldPoint;

    private Transform playTransform;

    private bool isWalking;

    private BaseCounter selectedCounter;

    private KitchenObject holdKitchenObject;

    public event Action<BaseCounter> SelectedCounterChangedAction;
    public static event Action AnyPlayerSpawnedAction;

    public static void ResetStaticData() {
        AnyPlayerSpawnedAction = null;
    }

    private void Awake() {
        playTransform = transform;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        /*
         * 注意，每个build.exe运行时，的NetworkManager都会实例化多个player对象:
         * 1. 判断IsOwner时，每个build.exe仅找到自己所拥有的player，执行逻辑
         * 2. 判断IsClient时，每个非host的build.exe找到本地所有player执行逻辑
         * 3. 判断IsServer时，每个host的build.exe找到本地所有player执行逻辑
         */
        if (IsOwner) {
            LocalInstance = this;

            transform.position = spawnPositionList[(int) OwnerClientId];
            AnyPlayerSpawnedAction?.Invoke();
        }

        if (IsServer) {
            // 服务器本地实例化的Player都会注册该事件
            NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnectCallback;
        }
    }

    private void OnNetworkClientDisconnectCallback(ulong clientId) {
        // 如果断线的Player手持物体，则销毁物体
        if (clientId == OwnerClientId && HasKitchenObject()) {
            KitchenObject.DespawnKitchenObject(GetKitchenObject());
        }
    }

    private void Start() {
        PlayerInput.Instance.InteractAction += PlayerInputOnInteractAction;
        PlayerInput.Instance.InteractAlternateAction += PlayerInputOnInteractAlternateAction;
    }

    public override void OnDestroy() {
        PlayerInput.Instance.InteractAction -= PlayerInputOnInteractAction;
        PlayerInput.Instance.InteractAlternateAction -= PlayerInputOnInteractAlternateAction;
    }

    private void PlayerInputOnInteractAction() {
        if (!MainGameManager.Instance.IsGamePlayingState()) {
            return;
        }

        if (selectedCounter == null) {
            return;
        }

        selectedCounter.Interact(this);
    }

    private void PlayerInputOnInteractAlternateAction() {
        if (!MainGameManager.Instance.IsGamePlayingState()) {
            return;
        }

        if (selectedCounter == null) {
            return;
        }

        selectedCounter.InteractAlternate(this);
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }

        HandleMovement();
        HandleInteract();
    }

    private void HandleMovement() {
        var inputVector = PlayerInput.Instance.GetMovementVector2Normalized();
        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        isWalking = moveDir != Vector3.zero;
        if (!isWalking) {
            return;
        }

        // 更新角度
        playTransform.forward = Vector3.Slerp(playTransform.forward, moveDir, rotateSpeed * Time.deltaTime);

        // check move on x and y dir
        var playerPos = playTransform.position;
        var moveDistance = moveSpeed * Time.deltaTime;
        var canMove = !Physics.BoxCast(
            playerPos,
            Vector3.one * playerRadius,
            moveDir,
            Quaternion.identity,
            moveDistance,
            moveColliderLayerMask
        );
        if (!canMove) {
            // try move on x dir
            var moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = moveDir.x != 0 &&
                      !Physics.BoxCast(
                          playerPos,
                          Vector3.one * playerRadius,
                          moveDirX,
                          Quaternion.identity,
                          moveDistance,
                          moveColliderLayerMask
                      );
            if (canMove) {
                moveDir = moveDirX;
            } else {
                // try move on y dir
                var moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = moveDir.z != 0 &&
                          !Physics.BoxCast(
                              playerPos,
                              Vector3.one * playerRadius,
                              moveDirZ,
                              Quaternion.identity,
                              moveDistance,
                              moveColliderLayerMask
                          );
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

    public void SetKitchenObject(KitchenObject targetObject) {
        holdKitchenObject = targetObject;
        if (targetObject != null) {
            SoundManager.Instance.PlayKitchenObjectPickUp(transform.position);
        }
    }

    public bool HasKitchenObject() {
        return holdKitchenObject != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }

    #endregion
}