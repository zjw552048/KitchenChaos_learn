using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : NetworkBehaviour {
    public static MainGameManager Instance { get; private set; }

    [SerializeField] private Transform playerPrefab;

    private const float GAME_PLAYER_TOTAL_TIME = 120f;

    public event Action LocalPlayerReadyChangedAction;
    public event Action GameStateChangedAction;

    public event Action LocalGamePausedAction;
    public event Action LocalGameUnpausedAction;

    public event Action MultiplayerGamePausedAction;
    public event Action MultiplayerGameUnpausedAction;

    private enum GameState {
        WaitToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private readonly NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitToStart);
    private readonly NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    private readonly NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(GAME_PLAYER_TOTAL_TIME);

    private bool localPlayerReady;
    private Dictionary<ulong, bool> playersReadyDic;

    private bool localGamePaused;
    private Dictionary<ulong, bool> playersPausedDir;

    private readonly NetworkVariable<bool> gamePaused = new NetworkVariable<bool>(false);

    private bool autoCheckGamePaused;

    private void Awake() {
        Instance = this;
        playersReadyDic = new Dictionary<ulong, bool>();
        playersPausedDir = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        gameState.OnValueChanged += OnGameStateValueChanged;
        gamePaused.OnValueChanged += OnGamePauseValueChanged;

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnectCallback;
            // OnLoad客户端场景加载完成、OnLoadEventCompleted所有客户端场景加载完成
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkSceneManagerOnLoadEventCompleted;
        }
    }

    private void OnGameStateValueChanged(GameState previousValue, GameState newValue) {
        GameStateChangedAction?.Invoke();
    }

    private void OnGamePauseValueChanged(bool previousValue, bool newValue) {
        if (gamePaused.Value) {
            Time.timeScale = 0f;
            MultiplayerGamePausedAction?.Invoke();
        } else {
            Time.timeScale = 1f;
            MultiplayerGameUnpausedAction?.Invoke();
        }
    }

    private void OnNetworkClientDisconnectCallback(ulong client) {
        autoCheckGamePaused = true;
    }

    private void NetworkSceneManagerOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (var clientId in clientsCompleted) {
            var playerTransform = Instantiate(playerPrefab);
            var playerNetworkObject = playerTransform.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnAsPlayerObject(clientId, true);
        }
    }

    private void Start() {
        PlayerInput.Instance.PauseAction += OnPauseAction;
        PlayerInput.Instance.InteractAction += OnInteractAction;
    }

    private void OnInteractAction() {
        if (gameState.Value != GameState.WaitToStart) {
            return;
        }

        localPlayerReady = true;
        LocalPlayerReadyChangedAction?.Invoke();

        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playersReadyDic[serverRpcParams.Receive.SenderClientId] = true;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyDic.ContainsKey(clientId) || !playersReadyDic[clientId]) {
                Debug.Log("Wait other player ready!");
                return;
            }
        }

        gameState.Value = GameState.CountdownToStart;
    }

    private void OnPauseAction() {
        TogglePauseGame();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        switch (gameState.Value) {
            case GameState.WaitToStart:
                break;

            case GameState.CountdownToStart:
                countDownToStartTimer.Value -= Time.deltaTime;
                if (countDownToStartTimer.Value > 0) {
                    return;
                }

                gamePlayingTimer.Value = GAME_PLAYER_TOTAL_TIME;
                gameState.Value = GameState.GamePlaying;
                GameStateChangedAction?.Invoke();
                break;

            case GameState.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value > 0) {
                    return;
                }

                gameState.Value = GameState.GameOver;
                GameStateChangedAction?.Invoke();
                break;

            case GameState.GameOver:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void LateUpdate() {
        if (autoCheckGamePaused) {
            autoCheckGamePaused = false;
            CheckGamePaused();
        }
    }

    public int GetCountdownToStartTimer() {
        return Mathf.CeilToInt(countDownToStartTimer.Value);
    }

    public bool IsWaitToStartState() {
        return gameState.Value == GameState.WaitToStart;
    }

    public bool IsCountDownToStartState() {
        return gameState.Value == GameState.CountdownToStart;
    }

    public bool IsGamePlayingState() {
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsGameOverState() {
        return gameState.Value == GameState.GameOver;
    }

    public float GetGamePlayingTimerNormalized() {
        return gamePlayingTimer.Value / GAME_PLAYER_TOTAL_TIME;
    }

    public bool IsLocalPlayerReady() {
        return localPlayerReady;
    }

    public void TogglePauseGame() {
        if (localGamePaused) {
            localGamePaused = false;
            SetGameUnpausedServerRpc();

            LocalGameUnpausedAction?.Invoke();
        } else {
            localGamePaused = true;
            SetGamePausedServerRpc();

            LocalGamePausedAction?.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetGamePausedServerRpc(ServerRpcParams serverRpcParams = default) {
        playersPausedDir[serverRpcParams.Receive.SenderClientId] = true;

        CheckGamePaused();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetGameUnpausedServerRpc(ServerRpcParams serverRpcParams = default) {
        playersPausedDir[serverRpcParams.Receive.SenderClientId] = false;

        CheckGamePaused();
    }

    private void CheckGamePaused() {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playersPausedDir.ContainsKey(clientId) && playersPausedDir[clientId]) {
                // 游戏处于暂停状态
                gamePaused.Value = true;
                return;
            }
        }

        // 游戏处于非暂停状态
        gamePaused.Value = false;
    }
}