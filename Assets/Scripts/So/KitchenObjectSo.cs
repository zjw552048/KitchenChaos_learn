using UnityEngine;

[CreateAssetMenu(menuName = "GameSo/KitchenObjectSo")]
public class KitchenObjectSo : ScriptableObject {
    public string objectName;
    public Sprite icon;
    public Transform prefab;
}