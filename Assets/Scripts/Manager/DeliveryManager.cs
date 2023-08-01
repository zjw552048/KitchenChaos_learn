using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSo recipeListSo;
    private List<RecipeSo> waitingRecipeSos;

    public event Action SpawnRecipeAction;
    public event Action CompleteRecipeAction;

    private float spawnRecipeTimer;
    private const float SPAWN_RECIPE_INTERVAL = 4f;
    private const int MAX_WAITING_RECIPE_COUNT = 4;

    private int recipeSuccessfulCount;

    private void Awake() {
        Instance = this;
        waitingRecipeSos = new List<RecipeSo>();
    }

    private void Update() {
        spawnRecipeTimer += Time.deltaTime;
        if (spawnRecipeTimer < SPAWN_RECIPE_INTERVAL) {
            return;
        }

        spawnRecipeTimer -= SPAWN_RECIPE_INTERVAL;

        if (waitingRecipeSos.Count >= MAX_WAITING_RECIPE_COUNT) {
            return;
        }

        var newRecipeSo = recipeListSo.recipeSos[Random.Range(0, recipeListSo.recipeSos.Length)];
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
                Debug.Log("player delivery the correct recipe, which index: " + i + ", recipeName: " +
                          waitingRecipeSos[i].recipeName);
                recipeSuccessfulCount++;
                waitingRecipeSos.RemoveAt(i);
                CompleteRecipeAction?.Invoke();
                SoundManager.Instance.PlayDeliverySuccess(plateKitchenObject.transform.position);
                return;
            }
        }

        Debug.LogWarning("player delivery the wrong recipe!");
        SoundManager.Instance.PlayDeliveryFail(plateKitchenObject.transform.position);
    }

    public List<RecipeSo> GetWaitingRecipeSos() {
        return waitingRecipeSos;
    }

    public int GetRecipeSuccessfulCount() {
        return recipeSuccessfulCount;
    }

}