using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
    [SerializeField] private FryingRecipeSo[] fryingRecipeSos;
    [SerializeField] private BurningRecipeSo[] burningRecipeSos;

    public event Action<float> RefreshProgressAction;
    public event Action<StoveState> StoveStateChanged;

    public enum StoveState {
        Idle,
        Frying,
        Burning,
        Burned,
    }

    private StoveState stoveState;
    private FryingRecipeSo fryingRecipeSo;
    private BurningRecipeSo burningRecipeSo;

    private void Update() {
        var kitchenObject = GetKitchenObject();
        switch (stoveState) {
            case StoveState.Idle:
                break;

            case StoveState.Frying:
                var fryingSeconds = kitchenObject.GetCurrentFryingSeconds();
                if (fryingSeconds < fryingRecipeSo.needFryingSeconds) {
                    fryingSeconds = kitchenObject.AddCurrentFryingSeconds(Time.deltaTime);
                    var curProgress = fryingSeconds / fryingRecipeSo.needFryingSeconds;
                    RefreshProgressAction?.Invoke(curProgress);
                } else {
                    kitchenObject.DestroySelf();
                    kitchenObject = KitchenObject.SpawnKitchenObject(fryingRecipeSo.output, this);

                    burningRecipeSo = GetBurningRecipeByInputSo(kitchenObject.GetKitchenObjectSo());

                    if (burningRecipeSo == null) {
                        Debug.Log("BurningRecipeSo not found!");
                        stoveState = StoveState.Idle;
                    } else {
                        stoveState = StoveState.Burning;
                    }

                    StoveStateChanged?.Invoke(stoveState);
                }

                break;

            case StoveState.Burning:
                var burningSeconds = kitchenObject.GetCurrentBurningSeconds();
                if (burningSeconds < burningRecipeSo.needBurningSeconds) {
                    burningSeconds = kitchenObject.AddCurrentBurningSeconds(Time.deltaTime);
                    var curProgress = burningSeconds / burningRecipeSo.needBurningSeconds;
                    RefreshProgressAction?.Invoke(curProgress);
                    // TODO 更新进度条
                } else {
                    kitchenObject.DestroySelf();
                    KitchenObject.SpawnKitchenObject(burningRecipeSo.output, this);

                    stoveState = StoveState.Burned;
                    StoveStateChanged?.Invoke(stoveState);
                    RefreshProgressAction?.Invoke(0f);
                }

                break;

            case StoveState.Burned:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log("StoveState: " + stoveState);
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {
                // player手持物体，counter被占用，无逻辑
            } else {
                // player手持物体，counter空闲，放下物体
                var kitchenObject = player.GetKitchenObject();
                var kitchenObjectSo = kitchenObject.GetKitchenObjectSo();
                fryingRecipeSo = GetFryingRecipeByInputSo(kitchenObjectSo);
                if (fryingRecipeSo == null) {
                    Debug.Log("FryingRecipeSo not found!");
                    return;
                }

                // 放下食材
                kitchenObject.SetKitchenObjectParent(this);

                stoveState = StoveState.Frying;
                StoveStateChanged?.Invoke(stoveState);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);

                stoveState = StoveState.Idle;
                StoveStateChanged?.Invoke(stoveState);
                RefreshProgressAction?.Invoke(0f);
            } else {
                // player未手持物体，counter空闲，无逻辑
            }
        }
    }

    public override void InteractAlternate(Player player) {
    }

    private FryingRecipeSo GetFryingRecipeByInputSo(KitchenObjectSo input) {
        foreach (var recipeSo in fryingRecipeSos) {
            if (recipeSo.input == input) {
                return recipeSo;
            }
        }

        return null;
    }

    private BurningRecipeSo GetBurningRecipeByInputSo(KitchenObjectSo input) {
        foreach (var recipeSo in burningRecipeSos) {
            if (recipeSo.input == input) {
                return recipeSo;
            }
        }

        return null;
    }
}