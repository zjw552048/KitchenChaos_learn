using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReadyManager : NetworkBehaviour {
    public static CharacterSelectReadyManager Instance { get; private set; }

    public event Action PlayersReadyStateChangedAction;
    private Dictionary<ulong, bool> playersReadyDic;

    private void Awake() {
        Instance = this;
        playersReadyDic = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady() {
        CharacterSelectReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CharacterSelectReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        CharacterSelectReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyDic.ContainsKey(clientId) || !playersReadyDic[clientId]) {
                Debug.Log("Wait other player select character ready!");
                return;
            }
        }

        SceneLoader.LoadNetworkScene(SceneLoader.SceneName.GameScene);
    }

    [ClientRpc]
    private void CharacterSelectReadyClientRpc(ulong clientId) {
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