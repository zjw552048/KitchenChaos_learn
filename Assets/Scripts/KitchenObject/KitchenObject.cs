using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {
    [SerializeField] private KitchenObjectSo kitchenObjectSo;
    
    private IKitchenObjectParent kitchenObjectParent;
    private int currentCutCount;
    private float fryingSeconds;
    private float burningSeconds;

    private FollowTransform followTransform;

    private void Awake() {
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
        kitchenObjectParent.SetKitchenObject(null);
        Destroy(gameObject);
    }

    public static void SpawnKitchenObject(KitchenObjectSo kitchenObjectSo, IKitchenObjectParent kitchenObjectParent) {
        // 多人模式下，预制生成对象必须在sever端进行，所以改为SeverRpc
        MultiplayerNetworkManager.Instance.SpawnKitchenObjectByServer(kitchenObjectSo, kitchenObjectParent);
    }
}