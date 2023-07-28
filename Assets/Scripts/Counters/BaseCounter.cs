using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent {
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

    public void SetKitChenObject(KitchenObject targetObject) {
        kitchenObject = targetObject;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    #endregion
}