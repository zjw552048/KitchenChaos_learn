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
        var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
        
        PlayerHoldKitchenObjectAction?.Invoke();
    }
}