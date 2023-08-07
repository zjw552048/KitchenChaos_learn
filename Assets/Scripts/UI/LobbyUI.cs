using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button quickJoinBtn;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private Button codeJoinBtn;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbySingleUITemplate;

    private void Awake() {
        lobbySingleUITemplate.gameObject.SetActive(false);
    }

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        playerNameInputField.text = MultiplayerNetworkManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener(newPlayerName => {
            MultiplayerNetworkManager.Instance.SetPlayerName(newPlayerName);
        });

        createLobbyBtn.onClick.AddListener(() => { lobbyCreateUI.Show(); });

        quickJoinBtn.onClick.AddListener(() => { GameLobbyManager.Instance.QuickJoin(); });

        codeJoinBtn.onClick.AddListener(() => {
            var code = lobbyCodeInputField.text;
            GameLobbyManager.Instance.JoinByCode(code);
        });

        GameLobbyManager.Instance.QueryLobbySuccessAction += OnQueryLobbySuccessAction;
        UpdateLobbyContainer(new List<Lobby>());
    }

    private void OnQueryLobbySuccessAction(List<Lobby> lobbyList) {
        UpdateLobbyContainer(lobbyList);
    }

    private void UpdateLobbyContainer(List<Lobby> lobbyList) {
        foreach (Transform child in lobbyContainer) {
            if (child == lobbySingleUITemplate) {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbyList) {
            var lobbyTransform = Instantiate(lobbySingleUITemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);

            var lobbySingleUI = lobbyTransform.GetComponent<LobbySingleUI>();
            lobbySingleUI.SetLobby(lobby);
        }
    }
}