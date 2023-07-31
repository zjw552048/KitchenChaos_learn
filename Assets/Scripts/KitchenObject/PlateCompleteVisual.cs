using System;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {
    [Serializable]
    private struct KitchenObjectSo_GameObject {
        public KitchenObjectSo KitchenObjectSo;
        public GameObject gameObject;
    }

    [SerializeField] private KitchenObjectSo_GameObject[] kitchenObjectSoGameObjects;

    private PlateKitchenObject plateKitchenObject;

    private void Awake() {
        plateKitchenObject = GetComponent<PlateKitchenObject>();
    }

    private void Start() {
        plateKitchenObject.AddIngredientAction += OnAddIngredientAction;

        foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjects) {
            kitchenObjectSoGameObject.gameObject.SetActive(false);
        }
    }

    private void OnAddIngredientAction(KitchenObjectSo kitchenObjectSo) {
        foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjects) {
            if (kitchenObjectSoGameObject.KitchenObjectSo != kitchenObjectSo) {
                continue;
            }

            kitchenObjectSoGameObject.gameObject.SetActive(true);
            return;
        }
    }
}