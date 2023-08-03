using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent {
    public Transform GetKitchenObjectFollowTransform();

    public KitchenObject GetKitchenObject();

    public void SetKitchenObject(KitchenObject targetObject);

    public bool HasKitchenObject();

    public NetworkObject GetNetworkObject();
}