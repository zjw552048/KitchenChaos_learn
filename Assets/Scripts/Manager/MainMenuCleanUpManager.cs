using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUpManager : MonoBehaviour {
    private void Awake() {
        if (MultiplayerNetworkManager.Instance != null) {
            Destroy(MultiplayerNetworkManager.Instance.gameObject);
        }

        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (GameLobbyManager.Instance != null) {
            Destroy(GameLobbyManager.Instance.gameObject);
        }
    }
}