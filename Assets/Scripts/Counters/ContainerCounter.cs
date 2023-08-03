using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {
    [SerializeField] private KitchenObjectSo kitchenObjectSo;
    [SerializeField] private SpriteRenderer kitchenObjectIcon;

    public event Action PlayerHoldKitchenObjectAction;

    public void Start() {
        kitchenObjectIcon.sprite = kitchenObjectSo.icon;
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            return;
        }

        KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);

        InteractLogicServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    
    [ClientRpc]
    private void InteractLogicClientRpc() {
        PlayerHoldKitchenObjectAction?.Invoke();
    }

    public override void InteractAlternate(Player player) {
    }
}