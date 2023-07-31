using UnityEngine;

[CreateAssetMenu(menuName = "GameSo/CuttingRecipeSo")]
public class CuttingRecipeSo : ScriptableObject {
    public KitchenObjectSo input;
    public KitchenObjectSo output;
    public int needCutCount;
}