using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO kitchenObjectSo;
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

        PlayerHoldKitchenObjectAction?.Invoke();
    }

    public override void InteractAlternate(Player player) {
    }
}