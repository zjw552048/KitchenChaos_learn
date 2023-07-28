using System;
using UnityEngine;

public class ClearCounter : MonoBehaviour {
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private KitchenObject kitchenObject;

    public void Interact() {
        if (kitchenObject == null) {
            var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this);
        } else {
            Debug.Log("Interact with has KitchenObject counter");
        }
    }

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

    [SerializeField] private ClearCounter otherClearCounter;
    [SerializeField] private bool testing;

    private void Update() {
        if (testing && Input.GetKey(KeyCode.T)) {
            if (HasKitchenObject()) {
                kitchenObject.SetClearCounter(otherClearCounter);
            }
        }
    }
}