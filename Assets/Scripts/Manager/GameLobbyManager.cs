using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameLobbyManager : MonoBehaviour {
    public static GameLobbyManager Instance { get; private set; }

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

    private async void QueryLobby() {
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

            MultiplayerNetworkManager.Instance.StartHost();
            SceneLoader.LoadNetworkScene(SceneLoader.SceneName.CharacterSelectScene);
        } catch (LobbyServiceException e) {
            CreateLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void QuickJoin() {
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            QuickJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void JoinByCode(string code) {
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            CodeJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }
    
    public async void JoinById(string id) {
        try {
            JoinLobbyStartAction?.Invoke();
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            CodeJoinLobbyFailedAction?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async void LeaveLobby() {
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
        return AuthenticationService.Instance.PlayerId;
    }
}