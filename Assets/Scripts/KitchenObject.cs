using UnityEngine;

public class KitchenObject : MonoBehaviour {
    private KitchenObjectSo kitchenObjectSo;
    private IKitchenObjectParent kitchenObjectParent;
    private int currentCutCount;

    private void SetKitchenObjectSo(KitchenObjectSo targetKitchenObjectSo) {
        kitchenObjectSo = targetKitchenObjectSo;
    }

    public KitchenObjectSo GetKitchenObjectSo() {
        return kitchenObjectSo;
    }

    public float GetCurrentCutProgress() {
        if (kitchenObjectSo.needCutCount == 0) {
            return 0f;
        }
        return (float) currentCutCount / kitchenObjectSo.needCutCount;
    }

    public float AddCutCountAndReturnProgress() {
        if (kitchenObjectSo.needCutCount == 0) {
            return 0f;
        }

        currentCutCount++;
        return (float) currentCutCount / kitchenObjectSo.needCutCount;
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

    public void DestroySelf() {
        kitchenObjectParent.SetKitChenObject(null);
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSo kitchenObjectSo,
        IKitchenObjectParent kitchenObjectParent) {
        var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        kitchenObject.SetKitchenObjectSo(kitchenObjectSo);
        return kitchenObject;
    }
}