using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {
    [SerializeField] private KitchenObjectSo kitchenObjectSo;

    private IKitchenObjectParent kitchenObjectParent;
    private int currentCutCount;
    private float fryingSeconds;
    private float burningSeconds;

    private FollowTransform followTransform;

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSo GetKitchenObjectSo() {
        return kitchenObjectSo;
    }

    public int GetCurrentCutCount() {
        return currentCutCount;
    }

    public int AddCurrentCutCount() {
        return ++currentCutCount;
    }

    public float GetCurrentFryingSeconds() {
        return fryingSeconds;
    }

    public float AddCurrentFryingSeconds(float dt) {
        fryingSeconds += dt;
        return fryingSeconds;
    }

    public float GetCurrentBurningSeconds() {
        return burningSeconds;
    }

    public float AddCurrentBurningSeconds(float dt) {
        burningSeconds += dt;
        return burningSeconds;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }

    public void SetKitchenObjectParent(IKitchenObjectParent targetParent) {
        SetKitchenObjectParentServerRpc(targetParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference targetParentNetworkObjectReference) {
        SetKitchenObjectParentClientRpc(targetParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference targetParentNetworkObjectReference) {
        targetParentNetworkObjectReference.TryGet(out var targetParentNetworkObject);
        var targetParent = targetParentNetworkObject.GetComponent<IKitchenObjectParent>();
        if (targetParent.HasKitchenObject()) {
            Debug.LogError("targetParent already has a kitchenObject!");
        }

        kitchenObjectParent?.SetKitchenObject(null);

        kitchenObjectParent = targetParent;
        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(targetParent.GetKitchenObjectFollowTransform());
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectForParent() {
        kitchenObjectParent.SetKitchenObject(null);
    }

    public static void SpawnKitchenObject(KitchenObjectSo kitchenObjectSo, IKitchenObjectParent kitchenObjectParent) {
        // 多人模式下，Network必须在sever端进行生成，所以改为SeverRpc
        MultiplayerNetworkManager.Instance.SpawnKitchenObjectByServer(kitchenObjectSo, kitchenObjectParent);
    }

    public static void DespawnKitchenObject(KitchenObject kitchenObject) {
        // 多人模式下，Network必须在sever端进行销毁，所以改为SeverRpc
        MultiplayerNetworkManager.Instance.DespawnKitchenObjectByServer(kitchenObject);
    }
}