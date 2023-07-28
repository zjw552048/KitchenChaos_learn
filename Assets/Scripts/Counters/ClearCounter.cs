using UnityEngine;

public class ClearCounter : MonoBehaviour {
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    public void Interact() {
        Debug.Log("Interact with " + transform);
        var kitchenObject = Instantiate(kitchenObjectSo.prefab, counterTopPoint);
    }
}