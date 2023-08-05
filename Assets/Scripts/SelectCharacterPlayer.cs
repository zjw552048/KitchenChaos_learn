using UnityEngine;

public class SelectCharacterPlayer : MonoBehaviour {
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    private int playerIndex;

    private void Awake() {
        playerIndex = transform.GetSiblingIndex();
    }

    private void Start() {
        MultiplayerNetworkManager.Instance.SelectCharacterPlayersChangedAction += OnSelectCharacterPlayersChangedAction;
        SelectCharacterReadyManager.Instance.PlayersReadyStateChangedAction += OnPlayersReadyStateChangedAction;

        UpdatePlayer();
    }

    private void OnPlayersReadyStateChangedAction() {
        UpdatePlayer();
    }

    private void OnSelectCharacterPlayersChangedAction() {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (MultiplayerNetworkManager.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            var playerData = MultiplayerNetworkManager.Instance.GetPlayerDataByPlayerIndex(playerIndex);
            var playerReady = SelectCharacterReadyManager.Instance.IsPlayerReady(playerData.clientId);
            readyTextGameObject.SetActive(playerReady);

            var playerColor = MultiplayerNetworkManager.Instance.GetPlayerColorByPlayerIndex(playerIndex);
            playerVisual.SetMaterialColor(playerColor);
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}