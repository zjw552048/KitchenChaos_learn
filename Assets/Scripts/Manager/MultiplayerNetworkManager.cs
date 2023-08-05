using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerNetworkManager : NetworkBehaviour {
    [SerializeField] private KitchenObjectListSo kitchenObjectListSo;

    public static MultiplayerNetworkManager Instance { get; private set; }

    private NetworkList<PlayerData> selectCharacterPlayers;

    private const int MAX_PLAYER_COUNT = 4;

    public event Action TryingToJoinGameAction;
    public event Action FailedToJoinGameAction;
    public event Action SelectCharacterPlayersChangedAction;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 必须在awake初始化，否则报错：A Native Collection has not been disposed, resulting in a memory leak
        selectCharacterPlayers = new NetworkList<PlayerData>();
        selectCharacterPlayers.OnListChanged += OnNetworkListSelectCharacterPlayersChanged;
    }

    private void OnNetworkListSelectCharacterPlayersChanged(NetworkListEvent<PlayerData> changeEvent) {
        SelectCharacterPlayersChangedAction?.Invoke();
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
        selectCharacterPlayers.Add(new PlayerData {
            clientId = clientId,
        });
    }

    private void ServerOnClientDisconnectCallback(ulong clientId) {
        foreach (var selectCharacterPlayer in selectCharacterPlayers) {
            if (selectCharacterPlayer.clientId != clientId) {
                continue;
            }

            selectCharacterPlayers.Remove(selectCharacterPlayer);
            return;
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response) {
        if (SceneManager.GetActiveScene().name != SceneLoader.SceneName.SelectCharacterScene.ToString()) {
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

    #region SelectCharacter 选择角色

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < selectCharacterPlayers.Count;
    }

    public PlayerData GetPlayerDataByPlayerIndex(int playerIndex) {
        return selectCharacterPlayers[playerIndex];
    }

    #endregion
}