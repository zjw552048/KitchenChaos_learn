using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    [SerializeField] private KitchenObjectSo[] validKitchenObjectSos;
    private List<KitchenObjectSo> kitchenObjectSos;

    public event Action<KitchenObjectSo> AddIngredientAction;

    private void Awake() {
        kitchenObjectSos = new List<KitchenObjectSo>();
    }

    public bool TryAddIngredient(KitchenObjectSo kitchenObjectSo) {
        if (!validKitchenObjectSos.Contains(kitchenObjectSo)) {
            return false;
        }

        if (kitchenObjectSos.Contains(kitchenObjectSo)) {
            return false;
        }

        kitchenObjectSos.Add(kitchenObjectSo);
        AddIngredientAction?.Invoke(kitchenObjectSo);
        return true;
    }
}