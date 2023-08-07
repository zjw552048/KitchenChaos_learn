using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {
    
    [SerializeField] private LobbyUI lobbyUI;

    private void Start() {
        MultiplayerNetworkManager.Instance.TryingToJoinGameAction += OnTryingToJoinGameAction;
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction += OnFailedToJoinGameAction;
        Hide();
    }

    private void OnDestroy() {
        // MultiplayNetworkManager标记了DontDestroyOnLoad，与UI对象的生命周期不同，所以需要注销事件
        MultiplayerNetworkManager.Instance.TryingToJoinGameAction -= OnTryingToJoinGameAction;
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction -= OnFailedToJoinGameAction;
    }

    private void OnTryingToJoinGameAction() {
        Show();
    }

    private void OnFailedToJoinGameAction() {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
        
        lobbyUI.Show();
    }
}