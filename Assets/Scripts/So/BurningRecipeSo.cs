using UnityEngine;

[CreateAssetMenu(menuName = "GameSo/BurningRecipeSo")]
public class BurningRecipeSo : ScriptableObject {
    public KitchenObjectSo input;
    public KitchenObjectSo output;
    public float needBurningSeconds;
}