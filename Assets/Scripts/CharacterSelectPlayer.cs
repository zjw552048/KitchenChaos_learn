using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour {
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    private int playerIndex;

    private void Awake() {
        playerIndex = transform.GetSiblingIndex();
    }

    private void Start() {
        MultiplayerNetworkManager.Instance.CharacterSelectPlayersChangedAction += OnCharacterSelectPlayersChangedAction;
        CharacterSelectReadyManager.Instance.PlayersReadyStateChangedAction += OnPlayersReadyStateChangedAction;

        UpdatePlayer();
    }

    private void OnPlayersReadyStateChangedAction() {
        UpdatePlayer();
    }

    private void OnCharacterSelectPlayersChangedAction() {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (MultiplayerNetworkManager.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            var playerData = MultiplayerNetworkManager.Instance.GetPlayerDataByPlayerIndex(playerIndex);
            var playerReady = CharacterSelectReadyManager.Instance.IsPlayerReady(playerData.clientId);
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