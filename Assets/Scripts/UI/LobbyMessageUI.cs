using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI reasonText;

    private void Start() {
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction += OnFailedToJoinGameAction;
        GameLobbyManager.Instance.CreateLobbyStartedAction += OnCreateLobbyStartedAction;
        GameLobbyManager.Instance.CreateLobbyFailedAction += OnCreateLobbyFailedAction;
        GameLobbyManager.Instance.JoinLobbyStartAction += OnJoinLobbyStartAction;
        GameLobbyManager.Instance.QuickJoinLobbyFailedAction += OnQuickJoinLobbyFailedAction;
        GameLobbyManager.Instance.CodeJoinLobbyFailedAction += OnCodeJoinLobbyFailedAction;

        closeBtn.onClick.AddListener(Hide);
        Hide();
    }

    private void OnCreateLobbyStartedAction() {
        ShowMessage("Creating lobby...");
    }

    private void OnCreateLobbyFailedAction() {
        ShowMessage("Failed to create lobby...");
    }

    private void OnJoinLobbyStartAction() {
        ShowMessage("Joining lobby...");
    }

    private void OnQuickJoinLobbyFailedAction() {
        ShowMessage("Failed to quick join lobby...");
    }

    private void OnCodeJoinLobbyFailedAction() {
        ShowMessage("Failed to code join lobby...");
    }


    private void OnDestroy() {
        // MultiplayNetworkManager标记了DontDestroyOnLoad，与UI对象的生命周期不同，所以需要注销事件
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction -= OnFailedToJoinGameAction;
        GameLobbyManager.Instance.CreateLobbyStartedAction -= OnCreateLobbyStartedAction;
        GameLobbyManager.Instance.CreateLobbyFailedAction -= OnCreateLobbyFailedAction;
        GameLobbyManager.Instance.JoinLobbyStartAction -= OnJoinLobbyStartAction;
        GameLobbyManager.Instance.QuickJoinLobbyFailedAction -= OnQuickJoinLobbyFailedAction;
        GameLobbyManager.Instance.CodeJoinLobbyFailedAction -= OnCodeJoinLobbyFailedAction;
    }

    private void OnFailedToJoinGameAction() {
        var reason = NetworkManager.Singleton.DisconnectReason;
        if (reason is null or "") {
            reason = "Failed to connect";
        }

        ShowMessage(reason);
    }

    private void ShowMessage(string message) {
        reasonText.text = message;
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}