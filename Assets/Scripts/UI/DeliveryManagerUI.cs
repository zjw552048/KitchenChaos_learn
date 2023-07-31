using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Start() {
        recipeTemplate.gameObject.SetActive(false);

        DeliveryManager.Instance.SpawnRecipeAction += OnSpawnRecipeAction;
        DeliveryManager.Instance.CompleteRecipeAction += OnCompleteRecipeAction;
    }

    private void OnSpawnRecipeAction() {
        UpdateVisualUI();
    }

    private void OnCompleteRecipeAction() {
        UpdateVisualUI();
    }

    private void UpdateVisualUI() {
        foreach (Transform child in container) {
            if (child == recipeTemplate) {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (var waitingRecipeSo in DeliveryManager.Instance.GetWaitingRecipeSos()) {
            var recipeTransform = Instantiate(recipeTemplate, container);
            var waitingRecipeUI = recipeTransform.GetComponent<WaitingRecipeUI>();
            waitingRecipeUI.SetWaitingRecipeSo(waitingRecipeSo);
            waitingRecipeUI.gameObject.SetActive(true);
        }
    }
}