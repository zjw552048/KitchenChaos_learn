using UnityEngine;

public class SelectCharacterPlayer : MonoBehaviour {
    private int playerIndex;

    private void Awake() {
        playerIndex = transform.GetSiblingIndex();
    }

    private void Start() {
        MultiplayerNetworkManager.Instance.SelectCharacterPlayersChangedAction += OnSelectCharacterPlayersChangedAction;

        UpdatePlayer();
    }

    private void OnSelectCharacterPlayersChangedAction() {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (MultiplayerNetworkManager.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();
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