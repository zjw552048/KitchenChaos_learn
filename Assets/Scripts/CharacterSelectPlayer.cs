using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private Button kickPlayerBtn;
    private int playerIndex;

    private void Awake() {
        playerIndex = transform.GetSiblingIndex();
    }

    private void Start() {
        if (NetworkManager.Singleton.IsServer) {
            var playerData = MultiplayerNetworkManager.Instance.GetPlayerDataByPlayerIndex(playerIndex);
            if (playerData.clientId != NetworkManager.ServerClientId) {
                // 服务端显示踢其他玩家按钮
                kickPlayerBtn.gameObject.SetActive(true);
                kickPlayerBtn.onClick.AddListener(() => {
                    MultiplayerNetworkManager.Instance.KickPlayer(playerIndex);
                });
            } else {
                // 服务端不显示踢自己按钮
                kickPlayerBtn.gameObject.SetActive(false);
            }
        } else {
            // 客户端不显示踢人按钮
            kickPlayerBtn.gameObject.SetActive(false);
        }


        MultiplayerNetworkManager.Instance.CharacterSelectPlayersChangedAction += OnCharacterSelectPlayersChangedAction;
        CharacterSelectReadyManager.Instance.PlayersReadyStateChangedAction += OnPlayersReadyStateChangedAction;

        UpdatePlayer();
    }

    private void OnDestroy() {
        MultiplayerNetworkManager.Instance.CharacterSelectPlayersChangedAction -= OnCharacterSelectPlayersChangedAction;
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

            var playerColor = MultiplayerNetworkManager.Instance.GetColorByColorId(playerData.colorId);
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