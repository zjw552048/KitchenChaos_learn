using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerNetworkManager : NetworkBehaviour {
    [SerializeField] private KitchenObjectListSo kitchenObjectListSo;

    [SerializeField] private List<Color> colors;

    public static MultiplayerNetworkManager Instance { get; private set; }

    private NetworkList<PlayerData> characterSelectPlayers;

    private const int MAX_PLAYER_COUNT = 4;

    public event Action TryingToJoinGameAction;
    public event Action FailedToJoinGameAction;
    public event Action CharacterSelectPlayersChangedAction;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 必须在awake初始化，否则报错：A Native Collection has not been disposed, resulting in a memory leak
        characterSelectPlayers = new NetworkList<PlayerData>();
        characterSelectPlayers.OnListChanged += OnNetworkListCharacterSelectPlayersChanged;
    }

    private void OnNetworkListCharacterSelectPlayersChanged(NetworkListEvent<PlayerData> changeEvent) {
        CharacterSelectPlayersChangedAction?.Invoke();
    }

    #region 生成、销毁KitchenObject

    public void SpawnKitchenObjectByServer(KitchenObjectSo kitchenObjectSo, IKitchenObjectParent kitchenObjectParent) {
        var kitchenObjectSoIndex = GetKitchenObjectIndexInList(kitchenObjectSo);
        SpawnKitchenObjectServerRpc(kitchenObjectSoIndex, kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex,
        NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        var kitchenObjectSo = GetKitchenObjectSoByIndex(kitchenObjectSoIndex);
        var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);

        var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetWorkObject);
        var kitchenObjectParent = kitchenObjectParentNetWorkObject.GetComponent<IKitchenObjectParent>();

        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectIndexInList(KitchenObjectSo kitchenObjectSo) {
        return kitchenObjectListSo.kitchenObjectSos.IndexOf(kitchenObjectSo);
    }

    public KitchenObjectSo GetKitchenObjectSoByIndex(int kitchenObjectSoIndex) {
        return kitchenObjectListSo.kitchenObjectSos[kitchenObjectSoIndex];
    }

    public void DespawnKitchenObjectByServer(KitchenObject kitchenObject) {
        DespawnKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        DespawnKitchenObjectClientRpc(kitchenObjectNetworkObjectReference);

        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void DespawnKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectForParent();
    }

    #endregion

    #region 创建host、创建client

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += ServerOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += ServerOnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void ServerOnClientConnectedCallback(ulong clientId) {
        characterSelectPlayers.Add(new PlayerData {
            clientId = clientId,
            colorId = GetFirstUnselectedColorId(),
        });
    }

    private void ServerOnClientDisconnectCallback(ulong clientId) {
        foreach (var characterSelectPlayer in characterSelectPlayers) {
            if (characterSelectPlayer.clientId != clientId) {
                continue;
            }

            characterSelectPlayers.Remove(characterSelectPlayer);
            return;
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response) {
        if (SceneManager.GetActiveScene().name != SceneLoader.SceneName.CharacterSelectScene.ToString()) {
            response.Approved = false;
            response.Reason = "Game already has started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_PLAYER_COUNT) {
            response.Approved = false;
            response.Reason = "Game player is full";
            return;
        }

        response.Approved = true;
    }

    public void StartClient() {
        TryingToJoinGameAction?.Invoke();

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientDisconnectCallback(ulong obj) {
        FailedToJoinGameAction?.Invoke();
    }

    #endregion

    #region CharacterSelect 选择角色

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < characterSelectPlayers.Count;
    }

    public PlayerData GetPlayerDataByPlayerIndex(int playerIndex) {
        return characterSelectPlayers[playerIndex];
    }

    public Color GetColorByColorId(int colorId) {
        // 因为playerIndex永远小于colors的最大index，暂时这么实现
        return colors[colorId];
    }

    public PlayerData GetPlayerData() {
        var clientId = NetworkManager.Singleton.LocalClientId;
        return GetPlayerDataByClientId(clientId);
    }

    public PlayerData GetPlayerDataByClientId(ulong clientId) {
        foreach (var characterSelectPlayer in characterSelectPlayers) {
            if (characterSelectPlayer.clientId == clientId) {
                return characterSelectPlayer;
            }
        }

        return default;
    }

    private int GetPlayerIndexByClientId(ulong clientId) {
        for (var index = 0; index < characterSelectPlayers.Count; index++) {
            if (characterSelectPlayers[index].clientId == clientId) {
                return index;
            }
        }

        return -1;
    }

    private int GetFirstUnselectedColorId() {
        for (var colorId = 0; colorId < colors.Count; colorId++) {
            if (IsColorUnselected(colorId)) {
                return colorId;
            }
        }

        return -1;
    }

    private bool IsColorUnselected(int colorId) {
        foreach (var characterSelectPlayer in characterSelectPlayers) {
            if (characterSelectPlayer.colorId == colorId) {
                return false;
            }
        }

        return true;
    }

    public void TryChangeCharacterSelectColor(int colorId) {
        if (!IsColorUnselected(colorId)) {
            return;
        }

        ChangeCharacterSelectColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCharacterSelectColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var playerIndex = GetPlayerIndexByClientId(clientId);
        if (playerIndex < 0) {
            return;
        }

        var playerData = characterSelectPlayers[playerIndex];
        playerData.colorId = colorId;

        characterSelectPlayers[playerIndex] = playerData;
    }

    #endregion
}