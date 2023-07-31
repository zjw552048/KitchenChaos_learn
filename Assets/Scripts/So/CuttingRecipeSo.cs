using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSo : ScriptableObject {
    public KitchenObjectSo input;
    public KitchenObjectSo output;
    public int needCutCount;
}