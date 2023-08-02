using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeDeliveredCountText;
    [SerializeField] private Button replayBtn;

    private void Start() {
        replayBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.GameScene); });

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