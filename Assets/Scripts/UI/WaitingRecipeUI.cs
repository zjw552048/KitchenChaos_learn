using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRecipeUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Start() {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetWaitingRecipeSo(RecipeSo waitingRecipeSo) {
        recipeNameText.text = waitingRecipeSo.recipeName;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (var kitchenObjectSo in waitingRecipeSo.KitchenObjectSos) {
            var iconTransform = Instantiate(iconTemplate, iconContainer);
            var iconImage = iconTransform.GetComponent<Image>();
            iconImage.sprite = kitchenObjectSo.icon;
            iconImage.gameObject.SetActive(true);
        }
    }
}