using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeDeliveredCountText;

    private void Start() {
        MainGameManager.Instance.GameStateChangedAction += OnGameStateChangedAction;

        Hide();
    }

    private void OnGameStateChangedAction() {
        if (MainGameManager.Instance.IsGameOverState()) {
            recipeDeliveredCountText.text = DeliveryManager.Instance.GetRecipeSuccessfulCount().ToString();
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}