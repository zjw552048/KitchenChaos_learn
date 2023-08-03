using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour {
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSo recipeListSo;
    private List<RecipeSo> waitingRecipeSos;

    public event Action SpawnRecipeAction;
    public event Action CompleteRecipeAction;

    public event Action DeliverySuccessAction;
    public event Action DeliveryFailAction;

    private float spawnRecipeTimer;
    private const float SPAWN_RECIPE_INTERVAL = 4f;
    private const int MAX_WAITING_RECIPE_COUNT = 4;

    private int recipeSuccessfulCount;

    private void Awake() {
        Instance = this;
        waitingRecipeSos = new List<RecipeSo>();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        if (!MainGameManager.Instance.IsGamePlayingState()) {
            return;
        }

        spawnRecipeTimer += Time.deltaTime;
        if (spawnRecipeTimer < SPAWN_RECIPE_INTERVAL) {
            return;
        }

        spawnRecipeTimer -= SPAWN_RECIPE_INTERVAL;

        if (waitingRecipeSos.Count >= MAX_WAITING_RECIPE_COUNT) {
            return;
        }

        var recipeIndex = Random.Range(0, recipeListSo.recipeSos.Length);
        AddWaitingRecipeClientRpc(recipeIndex);
    }

    [ClientRpc]
    private void AddWaitingRecipeClientRpc(int recipeIndex) {
        var newRecipeSo = recipeListSo.recipeSos[recipeIndex];
        waitingRecipeSos.Add(newRecipeSo);
        SpawnRecipeAction?.Invoke();
        Debug.Log("newRecipeSo: " + newRecipeSo.recipeName);
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject) {
        var plateKitchenObjectSos = plateKitchenObject.GetKitchenObjectSos();

        for (var i = 0; i < waitingRecipeSos.Count; i++) {
            var waitingRecipeSo = waitingRecipeSos[i];
            if (waitingRecipeSo.KitchenObjectSos.Count != plateKitchenObjectSos.Count) {
                continue;
            }

            var matchRecipe = true;
            foreach (var targetKitchenObject in waitingRecipeSo.KitchenObjectSos) {
                var matchTargetIngredient = false;
                foreach (var kitchenObject in plateKitchenObjectSos) {
                    if (targetKitchenObject != kitchenObject) {
                        continue;
                    }

                    matchTargetIngredient = true;
                    break;
                }

                if (!matchTargetIngredient) {
                    matchRecipe = false;
                    break;
                }
            }

            if (matchRecipe) {
                DeliverySuccessServerRpc(i, plateKitchenObject.transform.position);
                return;
            }
        }

        DeliveryFailServerRpc(plateKitchenObject.transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverySuccessServerRpc(int recipeIndex, Vector3 platePos) {
        DeliverySuccessClientRpc(recipeIndex, platePos);
    }

    [ClientRpc]
    private void DeliverySuccessClientRpc(int recipeIndex, Vector3 platePos) {
        Debug.Log("player delivery the correct recipe, which index: " + recipeIndex + ", recipeName: " +
                  waitingRecipeSos[recipeIndex].recipeName);
        recipeSuccessfulCount++;
        waitingRecipeSos.RemoveAt(recipeIndex);
        CompleteRecipeAction?.Invoke();
        DeliverySuccessAction?.Invoke();
        SoundManager.Instance.PlayDeliverySuccess(platePos);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryFailServerRpc(Vector3 platePos) {
        DeliveryFailClientRpc(platePos);
    }

    [ClientRpc]
    private void DeliveryFailClientRpc(Vector3 platePos) {
        Debug.LogWarning("player delivery the wrong recipe!");
        DeliveryFailAction?.Invoke();
        SoundManager.Instance.PlayDeliveryFail(platePos);
    }

    public List<RecipeSo> GetWaitingRecipeSos() {
        return waitingRecipeSos;
    }

    public int GetRecipeSuccessfulCount() {
        return recipeSuccessfulCount;
    }
}