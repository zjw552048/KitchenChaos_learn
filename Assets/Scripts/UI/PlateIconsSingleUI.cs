using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour {
    [SerializeField] private Image image;

    public void SetKitchenObjectSo(KitchenObjectSo kitchenObjectSo) {
        image.sprite = kitchenObjectSo.icon;
    }
}