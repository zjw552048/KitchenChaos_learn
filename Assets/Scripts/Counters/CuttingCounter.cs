using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {
    [SerializeField] private CuttingRecipeSo[] cuttingRecipeSos;

    public event Action<float> RefreshProgressAction;
    public event Action PlayerCutKitchenObjectAction;


    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {
                var playerHoldKitchenObject = player.GetKitchenObject();
                var counterHoldKitchenObject = GetKitchenObject();

                if (playerHoldKitchenObject.TryGetPlate(out var plate)) {
                    // 如果player持有的是plate，则尝试将counter上的kitchenObject放入plate
                    if (plate.TryAddIngredient(counterHoldKitchenObject.GetKitchenObjectSo())) {
                        counterHoldKitchenObject.DestroySelf();
                    }
                }
            } else {
                // player手持物体，counter空闲，放下物体
                var kitchenObject = player.GetKitchenObject();
                var kitchenObjectSo = kitchenObject.GetKitchenObjectSo();
                var cuttingRecipeSo = GetCuttingRecipeByInputSo(kitchenObjectSo);
                if (cuttingRecipeSo == null) {
                    Debug.Log("KitchenObject can not be cut!");
                    return;
                }

                // 放下食材
                kitchenObject.SetKitchenObjectParent(this);
                // 隐藏进度条
                var cutCount = kitchenObject.GetCurrentCutCount();
                var cutProgress = (float) cutCount / cuttingRecipeSo.needCutCount;
                RefreshProgressAction?.Invoke(cutProgress);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);
                // 隐藏进度条
                RefreshProgressAction?.Invoke(0f);
            } else {
                // player未手持物体，counter空闲，无逻辑
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) {
            return;
        }

        var kitchenObject = GetKitchenObject();
        var kitchenObjectSo = kitchenObject.GetKitchenObjectSo();
        var cuttingRecipeSo = GetCuttingRecipeByInputSo(kitchenObjectSo);
        if (cuttingRecipeSo == null) {
            return;
        }

        var curCount = kitchenObject.AddCurrentCutCount();
        var cutProgress = (float) curCount / cuttingRecipeSo.needCutCount;
        RefreshProgressAction?.Invoke(cutProgress);
        PlayerCutKitchenObjectAction?.Invoke();
        SoundManager.Instance.playChopSounds(transform.position);

        if (cutProgress < 1) {
            return;
        }

        kitchenObject.DestroySelf();
        KitchenObject.SpawnKitchenObject(cuttingRecipeSo.output, this);
    }

    private CuttingRecipeSo GetCuttingRecipeByInputSo(KitchenObjectSo input) {
        foreach (var recipeSo in cuttingRecipeSos) {
            if (recipeSo.input == input) {
                return recipeSo;
            }
        }

        return null;
    }
}