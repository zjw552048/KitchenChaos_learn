using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectCharacterReadyManager : NetworkBehaviour {
    public static SelectCharacterReadyManager Instance { get; private set; }

    public event Action PlayersReadyStateChangedAction;
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
        SelectCharacterReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyDic.ContainsKey(clientId) || !playersReadyDic[clientId]) {
                Debug.Log("Wait other player select character ready!");
                return;
            }
        }

        SceneLoader.LoadNetworkScene(SceneLoader.SceneName.GameScene);
    }

    [ClientRpc]
    private void SelectCharacterReadyClientRpc(ulong clientId) {
        if (!playersReadyDic.ContainsKey(clientId)) {
            playersReadyDic[clientId] = false;
        }

        playersReadyDic[clientId] = !playersReadyDic[clientId];
        PlayersReadyStateChangedAction?.Invoke();
    }

    public bool IsPlayerReady(ulong clientId) {
        return playersReadyDic.ContainsKey(clientId) && playersReadyDic[clientId];
    }
}