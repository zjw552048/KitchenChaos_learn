using UnityEngine;

[CreateAssetMenu(menuName = "GameSo/FryingRecipeSo")]
public class FryingRecipeSo : ScriptableObject {
    public KitchenObjectSo input;
    public KitchenObjectSo output;
    public float needFryingSeconds;
}