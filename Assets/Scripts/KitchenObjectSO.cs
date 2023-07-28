using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSo : ScriptableObject {
    public string objectName;
    public Sprite icon;
    public Transform prefab;
    public KitchenObjectSo cutOutputSo;
    public int needCutCount;
}