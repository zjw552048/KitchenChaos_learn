using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSo : ScriptableObject {
    public string type;
    public Sprite icon;
    public Transform prefab;
    public KitchenObjectSo cutOutputSo;
}