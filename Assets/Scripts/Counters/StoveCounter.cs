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
    private FryingRecipeSo fryingRecipeSo;
    private BurningRecipeSo burningRecipeSo;

    public override void OnNetworkSpawn() {
        stoveState.OnValueChanged += OnNetworkVariable_StoveStateValueChanged;
    }

    private void OnNetworkVariable_StoveStateValueChanged(StoveState previousvalue, StoveState newvalue) {
        StoveStateChangedAction?.Invoke(stoveState.Value);

        if (stoveState.Value is StoveState.Idle or StoveState.Burned) {
            RefreshProgressAction?.Invoke(0f);
        }
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
                    var curProgress = fryingSeconds / fryingRecipeSo.needFryingSeconds;
                    SyncProgressServerRpc(curProgress);
                } else {
                    KitchenObject.DespawnKitchenObject(kitchenObject);
                    KitchenObject.SpawnKitchenObject(fryingRecipeSo.output, this);

                    if (!GetBurningRecipeByInputSo(fryingRecipeSo.output)) {
                        Debug.Log("BurningRecipeSo not found!");
                        stoveState.Value = StoveState.Idle;
                        return;
                    }

                    stoveState.Value = StoveState.Burning;
                    var kitchenObjectSoIndex =
                        MultiplayerNetworkManager.Instance.GetKitchenObjectIndexInList(fryingRecipeSo.output);
                    SetBurningRecipeSoClientRpc(kitchenObjectSoIndex);
                }

                break;

            case StoveState.Burning:
                var burningSeconds = kitchenObject.GetCurrentBurningSeconds();
                if (burningSeconds < burningRecipeSo.needBurningSeconds) {
                    burningSeconds = kitchenObject.AddCurrentBurningSeconds(Time.deltaTime);
                    var curProgress = burningSeconds / burningRecipeSo.needBurningSeconds;
                    SyncProgressServerRpc(curProgress);
                } else {
                    KitchenObject.DespawnKitchenObject(kitchenObject);
                    KitchenObject.SpawnKitchenObject(burningRecipeSo.output, this);

                    stoveState.Value = StoveState.Burned;
                }

                break;

            case StoveState.Burned:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [ServerRpc]
    private void SyncProgressServerRpc(float curProgress) {
        SyncProgressClientRpc(curProgress);
    }

    [ClientRpc]
    private void SyncProgressClientRpc(float curProgress) {
        RefreshProgressAction?.Invoke(curProgress);
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

                counterHoldKitchenObject.DestroySelf();

                SetStoveStateIdleServerRpc();
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
                SetStoveStateFryingServerRpc(kitchenObjectSoIndex);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStoveStateIdleServerRpc();
            } else {
                // player未手持物体，counter空闲，无逻辑
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStoveStateFryingServerRpc(int kitchenObjectSoIndex) {
        stoveState.Value = StoveState.Frying;

        SetFryingRecipeSoClientRpc(kitchenObjectSoIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSoClientRpc(int kitchenObjectSoIndex) {
        var kitchenObjectSo = MultiplayerNetworkManager.Instance.GetKitchenObjectSoByIndex(kitchenObjectSoIndex);
        fryingRecipeSo = GetFryingRecipeByInputSo(kitchenObjectSo);
    }

    [ClientRpc]
    private void SetBurningRecipeSoClientRpc(int kitchenObjectSoIndex) {
        var kitchenObjectSo = MultiplayerNetworkManager.Instance.GetKitchenObjectSoByIndex(kitchenObjectSoIndex);
        burningRecipeSo = GetBurningRecipeByInputSo(kitchenObjectSo);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStoveStateIdleServerRpc() {
        stoveState.Value = StoveState.Idle;
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