using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLobbyManager : MonoBehaviour {
    public static GameLobbyManager Instance { get; private set; }

    private Lobby joinedLobby;

    private float lobbyHeartbeatTimer;
    private const float LOBBY_HEARTBEAT_INTERVAL = 15f;

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

        Debug.Log("send heartbeat!");
        LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
    }

    private bool IsHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
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
            Console.WriteLine(e);
        }
    }

    public async void QuickJoin() {
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public async void JoinByCode(string code) {
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public async void LeaveLobby() {
        try {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }

    public Lobby GetLobby() {
        return joinedLobby;
    }
}