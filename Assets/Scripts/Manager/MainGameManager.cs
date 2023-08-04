using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainGameManager : NetworkBehaviour {
    public static MainGameManager Instance { get; private set; }

    private const float GAME_PLAYER_TOTAL_TIME = 120f;

    public event Action LocalPlayerReadyChangedAction;
    public event Action GameStateChangedAction;
    public event Action GamePausedAction;
    public event Action GameUnpausedAction;

    private enum GameState {
        WaitToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private readonly NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitToStart);
    private readonly NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    private readonly NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(GAME_PLAYER_TOTAL_TIME);

    private bool gamePaused;
    private bool localPlayerReady;
    private Dictionary<ulong, bool> playersReadyDic;

    private void Awake() {
        Instance = this;
        playersReadyDic = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        gameState.OnValueChanged += OnGameStateValueChanged;
    }

    private void OnGameStateValueChanged(GameState previousvalue, GameState newvalue) {
        GameStateChangedAction?.Invoke();
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

    public int GetCountdownToStartTimer() {
        return Mathf.CeilToInt(countDownToStartTimer.Value);
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
        gamePaused = !gamePaused;
        if (gamePaused) {
            Time.timeScale = 0f;
            GamePausedAction?.Invoke();
        } else {
            Time.timeScale = 1f;
            GameUnpausedAction?.Invoke();
        }
    }
}