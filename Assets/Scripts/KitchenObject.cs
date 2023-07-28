using UnityEngine;

public class KitchenObject : MonoBehaviour {
    
    private IKitchenObjectParent kitchenObjectParent;
    

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

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
}