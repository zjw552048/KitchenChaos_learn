using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
    [SerializeField] private FryingRecipeSo[] fryingRecipeSos;
    [SerializeField] private BurningRecipeSo[] burningRecipeSos;

    public const float BURNED_WARNING_PROGRESS = 0.3f;

    public event Action<float> RefreshProgressAction;
    public event Action<StoveState> StoveStateChangedAction;

    public enum StoveState {
        Idle,
        Frying,
        Burning,
        Burned,
    }

    private readonly NetworkVariable<StoveState> stoveState = new NetworkVariable<StoveState>(StoveState.Idle);
    private readonly NetworkVariable<float> curProgress = new NetworkVariable<float>(0f);
    private FryingRecipeSo fryingRecipeSo;
    private BurningRecipeSo burningRecipeSo;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        curProgress.OnValueChanged += OnCurProgressValueChanged;
    }

    private void OnCurProgressValueChanged(float previousvalue, float newvalue) {
        RefreshProgressAction?.Invoke(curProgress.Value);
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        if (!MainGameManager.Instance.IsGamePlayingState()) {
            return;
        }

        var kitchenObject = GetKitchenObject();
        switch (stoveState.Value) {
            case StoveState.Idle:
                break;

            case StoveState.Frying:
                var fryingSeconds = kitchenObject.GetCurrentFryingSeconds();
                if (fryingSeconds < fryingRecipeSo.needFryingSeconds) {
                    fryingSeconds = kitchenObject.AddCurrentFryingSeconds(Time.deltaTime);
                    curProgress.Value = fryingSeconds / fryingRecipeSo.needFryingSeconds;
                } else {
                    KitchenObject.DespawnKitchenObject(kitchenObject);
                    KitchenObject.SpawnKitchenObject(fryingRecipeSo.output, this);

                    if (!GetBurningRecipeByInputSo(fryingRecipeSo.output)) {
                        Debug.Log("BurningRecipeSo not found!");
                        SetStoveStateServerRpc(StoveState.Idle);
                        return;
                    }

                    var kitchenObjectSoIndex =
                        MultiplayerNetworkManager.Instance.GetKitchenObjectIndexInList(fryingRecipeSo.output);
                    SetStoveStateServerRpc(StoveState.Burning, kitchenObjectSoIndex);
                }

                break;

            case StoveState.Burning:
                var burningSeconds = kitchenObject.GetCurrentBurningSeconds();
                if (burningSeconds < burningRecipeSo.needBurningSeconds) {
                    burningSeconds = kitchenObject.AddCurrentBurningSeconds(Time.deltaTime);
                    curProgress.Value = burningSeconds / burningRecipeSo.needBurningSeconds;
                } else {
                    KitchenObject.DespawnKitchenObject(kitchenObject);
                    KitchenObject.SpawnKitchenObject(burningRecipeSo.output, this);
                    SetStoveStateServerRpc(StoveState.Burned);
                }

                break;

            case StoveState.Burned:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStoveStateServerRpc(StoveState nextStoveState, int nextKitchenObjectSoIndex = -1) {
        stoveState.Value = nextStoveState;
        if (nextStoveState is StoveState.Idle or StoveState.Burned) {
            curProgress.Value = 0f;
        }

        SetStoveStateClientRpc(nextStoveState, nextKitchenObjectSoIndex);
    }

    [ClientRpc]
    private void SetStoveStateClientRpc(StoveState nextStoveState, int nextKitchenObjectSoIndex) {
        StoveStateChangedAction?.Invoke(nextStoveState);

        KitchenObjectSo nextKitchenObjectSo;
        switch (nextStoveState) {
            case StoveState.Idle:
                fryingRecipeSo = null;
                burningRecipeSo = null;
                break;

            case StoveState.Frying:
                nextKitchenObjectSo =
                    MultiplayerNetworkManager.Instance.GetKitchenObjectSoByIndex(nextKitchenObjectSoIndex);
                fryingRecipeSo = GetFryingRecipeByInputSo(nextKitchenObjectSo);
                burningRecipeSo = null;
                break;

            case StoveState.Burning:
                nextKitchenObjectSo =
                    MultiplayerNetworkManager.Instance.GetKitchenObjectSoByIndex(nextKitchenObjectSoIndex);
                fryingRecipeSo = null;
                burningRecipeSo = GetBurningRecipeByInputSo(nextKitchenObjectSo);
                break;

            case StoveState.Burned:
                fryingRecipeSo = null;
                burningRecipeSo = null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(nextStoveState), nextStoveState, null);
        }
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {
                var playerHoldKitchenObject = player.GetKitchenObject();
                if (!playerHoldKitchenObject.TryGetPlate(out var plate)) {
                    return;
                }

                var counterHoldKitchenObject = GetKitchenObject();
                // 如果player持有的是plate，则尝试将counter上的kitchenObject放入plate
                if (!plate.TryAddIngredient(counterHoldKitchenObject.GetKitchenObjectSo())) {
                    return;
                }

                MultiplayerNetworkManager.Instance.DespawnKitchenObjectByServer(counterHoldKitchenObject);

                SetStoveStateServerRpc(StoveState.Idle);
            } else {
                // player手持物体，counter空闲，放下物体
                var kitchenObject = player.GetKitchenObject();
                var kitchenObjectSo = kitchenObject.GetKitchenObjectSo();
                var tempFryingRecipeSo = GetFryingRecipeByInputSo(kitchenObjectSo);
                if (tempFryingRecipeSo == null) {
                    Debug.Log("FryingRecipeSo not found!");
                    return;
                }


                // 放下食材
                kitchenObject.SetKitchenObjectParent(this);

                var kitchenObjectSoIndex =
                    MultiplayerNetworkManager.Instance.GetKitchenObjectIndexInList(kitchenObjectSo);
                SetStoveStateServerRpc(StoveState.Frying, kitchenObjectSoIndex);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStoveStateServerRpc(StoveState.Idle);
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

    public bool IsBurningState() {
        return stoveState.Value == StoveState.Burning;
    }
}