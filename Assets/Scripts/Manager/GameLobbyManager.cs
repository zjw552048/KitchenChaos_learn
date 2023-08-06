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
            await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerNetworkManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Console.WriteLine(e);
        }
    }
}