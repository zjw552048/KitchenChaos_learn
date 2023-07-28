using UnityEngine;

public interface IKitchenObjectParent {
    public Transform GetKitchenObjectFollowTransform();

    public KitchenObject GetKitchenObject();

    public void SetKitChenObject(KitchenObject targetObject);

    public bool HasKitchenObject();
}