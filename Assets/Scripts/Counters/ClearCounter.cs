using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent {
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private KitchenObject kitchenObject;

    public void Interact(Player player) {
        if (kitchenObject == null) {
            var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        } else {
            Debug.Log("Interact with has KitchenObject counter");
            kitchenObject.SetKitchenObjectParent(player);
        }
    }

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