using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    [SerializeField] private KitchenObjectSo[] validKitchenObjectSos;

    private List<KitchenObjectSo> kitchenObjectSos;

    public event Action<KitchenObjectSo> AddIngredientAction;

    protected override void Awake() {
        base.Awake();
        kitchenObjectSos = new List<KitchenObjectSo>();
    }

    public bool TryAddIngredient(KitchenObjectSo kitchenObjectSo) {
        if (!validKitchenObjectSos.Contains(kitchenObjectSo)) {
            return false;
        }

        if (kitchenObjectSos.Contains(kitchenObjectSo)) {
            return false;
        }

        var kitchenObjectSoIndex = MultiplayerNetworkManager.Instance.GetKitchenObjectIndexInList(kitchenObjectSo);
        AddIngredientServerRpc(kitchenObjectSoIndex);
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSoIndex) {
        AddIngredientClientRpc(kitchenObjectSoIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSoIndex) {
        var kitchenObjectSo = MultiplayerNetworkManager.Instance.GetKitchenObjectSoByIndex(kitchenObjectSoIndex);
        kitchenObjectSos.Add(kitchenObjectSo);
        AddIngredientAction?.Invoke(kitchenObjectSo);
    }

    public List<KitchenObjectSo> GetKitchenObjectSos() {
        return kitchenObjectSos;
    }
}