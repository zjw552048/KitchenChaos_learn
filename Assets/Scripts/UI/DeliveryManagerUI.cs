using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Start() {
        recipeTemplate.gameObject.SetActive(false);

        MainGameManager.Instance.GameStateChangedAction += OnGameStateChangedAction;
        DeliveryManager.Instance.SpawnRecipeAction += OnSpawnRecipeAction;
        DeliveryManager.Instance.CompleteRecipeAction += OnCompleteRecipeAction;

        Hide();
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsGamePlayingState()) {
            Show();
        } else {
            Hide();
        }
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

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}