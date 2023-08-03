using Unity.Netcode;
using UnityEngine;

public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent {
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public abstract void Interact(Player player);
    public abstract void InteractAlternate(Player player);

    #region IKitchenObjectParent实现

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject targetObject) {
        kitchenObject = targetObject;
        if (targetObject != null) {
            SoundManager.Instance.PlayKitchenObjectDrop(transform.position);
        }
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }

    #endregion
}