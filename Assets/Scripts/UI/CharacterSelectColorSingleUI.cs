using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColorSingleUI : MonoBehaviour {
    [SerializeField] private Button selectBtn;
    [SerializeField] private Image colorImage;
    [SerializeField] private GameObject colorSelectedGameObject;

    private int colorId;

    private void Awake() {
        colorId = transform.GetSiblingIndex();
    }

    private void Start() {
        selectBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.Instance.TryChangeCharacterSelectColor(colorId);
        });

        MultiplayerNetworkManager.Instance.CharacterSelectPlayersChangedAction += OnCharacterSelectPlayersChangedAction;

        colorImage.color = MultiplayerNetworkManager.Instance.GetColorByColorId(colorId);
        UpdateIsSelected();
    }

    private void OnCharacterSelectPlayersChangedAction() {
        UpdateIsSelected();
    }

    private void UpdateIsSelected() {
        var playerData = MultiplayerNetworkManager.Instance.GetPlayerData();
        var selected = playerData.colorId == colorId;
        colorSelectedGameObject.SetActive(selected);
    }
}