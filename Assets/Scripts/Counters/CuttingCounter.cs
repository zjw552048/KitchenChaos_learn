using System;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    public event Action<float> RefreshCuttingProgressAction;
    public event Action PlayerCutKitchenObjectAction;
    

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {
                // player手持物体，counter被占用，无逻辑
            } else {
                // player手持物体，counter空闲，放下物体
                var kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);
                // 隐藏进度条
                var cutProgress = kitchenObject.GetCurrentCutProgress();
                RefreshCuttingProgressAction?.Invoke(cutProgress);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);
                // 隐藏进度条
                RefreshCuttingProgressAction?.Invoke(0f);
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
        var cutOutputSo = kitchenObjectSo.cutOutputSo;
        if (cutOutputSo == null) {
            return;
        }

        var cutProgress = kitchenObject.AddCutCountAndReturnProgress();
        RefreshCuttingProgressAction?.Invoke(cutProgress);
        PlayerCutKitchenObjectAction?.Invoke();

        if (cutProgress < 1) {
            return;
        }

        kitchenObject.DestroySelf();
        KitchenObject.SpawnKitchenObject(cutOutputSo, this);
    }
}