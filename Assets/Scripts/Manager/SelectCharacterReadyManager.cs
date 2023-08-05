using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectCharacterReadyManager : NetworkBehaviour{
    public static SelectCharacterReadyManager Instance { get; private set; }

    private Dictionary<ulong, bool> playersReadyDic;

    private void Awake() {
        Instance = this;
        playersReadyDic = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady() {
        SelectCharacterReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectCharacterReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playersReadyDic[serverRpcParams.Receive.SenderClientId] = true;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyDic.ContainsKey(clientId) || !playersReadyDic[clientId]) {
                Debug.Log("Wait other player select character ready!");
                return;
            }
        }

        SceneLoader.LoadNetworkScene(SceneLoader.SceneName.GameScene);
    }
}