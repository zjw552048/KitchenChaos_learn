using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameLobbyManager : MonoBehaviour {
    public static GameLobbyManager Instance { get; private set; }

    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";

    private Lobby joinedLobby;

    private float lobbyHeartbeatTimer;
    private const float LOBBY_HEARTBEAT_INTERVAL = 15f;

    private float lobbyQueryTimer;
    private const float LOBBY_QUERY_INTERVAL = 5f;

    public event Action CreateLobbyStartedAction;
    public event Action CreateLobbyFailedAction;

    public event Action JoinLobbyStartAction;
    public event Action QuickJoinLobbyFailedAction;
    public event Action CodeJoinLobbyFailedAction;

    public event Action<List<Lobby>> QueryLobbySuccessAction;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication() {
        if (UnityServices.State == ServicesInitializationState.Initialized) {
            return;
        }

        Debug.Log("Unity Authentication start!");
        var options = new InitializationOptions();
        options.SetProfile(Random.Range(0, 9999).ToString());
        await UnityServices.InitializeAsync(options);
        Debug.Log("UnityService initialize done!");

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("AuthenticationService SignIn done!");
    }

    private void Update() {
        HandleHostHeartbeat();
        HandleQueryLobby();
    }

    private void HandleHostHeartbeat() {
        if (!IsHost()) {
            return;
        }

        lobbyHeartbeatTimer += Time.deltaTime;
        if (lobbyHeartbeatTimer < LOBBY_HEARTBEAT_INTERVAL) {
            return;
        }

        lobbyHeartbeatTimer -= LOBBY_HEARTBEAT_INTERVAL;

        LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
    }

    private void HandleQueryLobby() {
        if (SceneManager.GetActiveScene().name != SceneLoader.SceneName.LobbyScene.ToString()) {
            return;
        }

        if (!AuthenticationService.Instance.IsSignedIn) {
            return;
        }

        if (joinedLobby != null) {
            return;
        }

        lobbyQueryTimer += Time.deltaTime;
        if (lobbyQueryTimer < LOBBY_QUERY_INTERVAL) {
            return;
        }

        lobbyQueryTimer -= LOBBY_QUERY_INTERVAL;

        QueryLobby();
    }

    private bool IsHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    #region Relay Logic

    private async Task<Allocation> AllocationRelay() {
        try {
            var allocation =
                await RelayService.Instance.CreateAllocationAsync(MultiplayerNetworkManager.MAX_PLAYER_COUNT - 1);
            return allocation;
        } catch (RelayServiceException e) {
            Console.WriteLine(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return joinCode;
        } catch (RelayServiceException e) {
            Console.WriteLine(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return allocation;
        } catch (RelayServiceException e) {
            Console.WriteLine(e);
            return default;
        }
    }

    #endregion

    #region Lobby Logic

    private async void QueryLobby() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        try {
            var options = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, 0.ToString(), QueryFilter.OpOptions.GT),
                }
            };
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
            QueryLobbySuccessAction?.Invoke(queryResponse.Results);
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        try {
            CreateLobbyStartedAction?.Invoke();
            var options = new CreateLobbyOptions {
                IsPrivate = isPrivate
            };
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName,
                MultiplayerNetworkManager.MAX_PLAYER_COUNT,
                options
            );

            var allocation = await AllocationRelay();
            var relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {
                        RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            });

            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerNetworkManager.Instance.StartHost();
            SceneLoader.LoadNetworkScene(SceneLoader.SceneName.CharacterSelectScene);
        } catch (LobbyServiceException e) {
            CreateLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void QuickJoin() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var joinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            var joinAllocation = await JoinRelay(joinCode);
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            QuickJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void JoinByCode(string code) {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            
            var joinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            var joinAllocation = await JoinRelay(joinCode);
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            CodeJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void JoinById(string id) {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            
            var joinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            var joinAllocation = await JoinRelay(joinCode);
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            CodeJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void LeaveLobby() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        if (joinedLobby == null) {
            return;
        }

        try {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public async void KickLeaveLobby(string playerId) {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        if (joinedLobby == null) {
            return;
        }

        if (!IsHost()) {
            return;
        }

        try {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            joinedLobby = null;
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public async void DeleteLobby() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            return;
        }
        if (!IsHost()) {
            return;
        }

        try {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public Lobby GetLobby() {
        return joinedLobby;
    }

    public string GetLobbyPlayerId() {
        if (MultiplayerNetworkManager.SinglePlayerMode) {
            return "localSinglePlayer";
        }

        return AuthenticationService.Instance.PlayerId;
    }

    #endregion
}