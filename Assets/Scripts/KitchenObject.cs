using UnityEngine;

public class KitchenObject : MonoBehaviour {
    private IKitchenObjectParent kitchenObjectParent;

    public void SetKitchenObjectParent(IKitchenObjectParent targetParent) {
        if (targetParent.HasKitchenObject()) {
            Debug.LogError("targetParent already has a kitchenObject!");
        }

        kitchenObjectParent?.SetKitChenObject(null);

        kitchenObjectParent = targetParent;
        kitchenObjectParent.SetKitChenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public void DestroySelf() {
        kitchenObjectParent.SetKitChenObject(null);
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSo,
        IKitchenObjectParent kitchenObjectParent) {
        var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }
}