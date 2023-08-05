using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectFailedUI : MonoBehaviour {
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI reasonText;

    private void Start() {
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction += OnFailedToJoinGameAction;

        closeBtn.onClick.AddListener(Hide);
        Hide();
    }

    private void OnDestroy() {
        // MultiplayNetworkManager标记了DontDestroyOnLoad，与UI对象的生命周期不同，所以需要注销事件
        MultiplayerNetworkManager.Instance.FailedToJoinGameAction -= OnFailedToJoinGameAction;
    }

    private void OnFailedToJoinGameAction() {
        var reason = NetworkManager.Singleton.DisconnectReason;
        if (reason is null or "") {
            reason = "Failed to connect";
        }

        reasonText.text = reason;
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}