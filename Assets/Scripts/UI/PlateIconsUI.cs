using UnityEngine;

public class PlateIconsUI : MonoBehaviour {
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Start() {
        plateKitchenObject.AddIngredientAction += OnAddIngredientAction;
        iconTemplate.gameObject.SetActive(false);
    }

    private void OnAddIngredientAction(KitchenObjectSo obj) {
        foreach (Transform child in transform) {
            if (child == iconTemplate) {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (var kitchenObjectSo in plateKitchenObject.GetKitchenObjectSos()) {
            var iconTransform = Instantiate(iconTemplate, transform);
            var iconsSingleUI = iconTransform.GetComponent<PlateIconsSingleUI>();
            iconsSingleUI.SetKitchenObjectSo(kitchenObjectSo);
            iconsSingleUI.gameObject.SetActive(true);
        }
    }
}