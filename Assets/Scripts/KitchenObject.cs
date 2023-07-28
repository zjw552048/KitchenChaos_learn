using UnityEngine;

public class KitchenObject : MonoBehaviour {
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private ClearCounter counter;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSo;
    }

    public void SetClearCounter(ClearCounter targetCounter) {
        if (targetCounter.HasKitchenObject()) {
            Debug.LogError("targetCounter already has a kitchenObject!");
        }

        if (counter != null) {
            counter.SetKitChenObject(null);
        }

        counter = targetCounter;
        transform.parent = counter.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
        
        counter.SetKitChenObject(this);
    }

    public ClearCounter GetClearCount() {
        return counter;
    }
}