using System;
using UnityEngine;

public class MainGameManager : MonoBehaviour {
    public static MainGameManager Instance { get; private set; }

    private const float GAME_PLAYER_TOTAL_TIME = 600f;

    public event Action GameStateChangedAction;
    public event Action GamePausedAction;
    public event Action GameUnpausedAction;

    private enum GameState {
        WaitToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState gameState;
    private float countDownToStartTimer = 3f;
    private float gamePlayingTimer;

    private bool gamePaused;

    private void Awake() {
        Instance = this;
        gameState = GameState.WaitToStart;
    }

    private void Start() {
        PlayerInput.Instance.PauseAction += OnPauseAction;
        PlayerInput.Instance.InteractAction += OnInteractAction;

        // FIXME:方便Netcode逻辑测试，直接进入倒计时状态
        gameState = GameState.CountdownToStart;
        GameStateChangedAction?.Invoke();
        countDownToStartTimer = 1f;
    }

    private void OnInteractAction() {
        if (gameState != GameState.WaitToStart) {
            return;
        }

        gameState = GameState.CountdownToStart;
        GameStateChangedAction?.Invoke();
    }

    private void OnPauseAction() {
        TogglePauseGame();
    }

    private void Update() {
        switch (gameState) {
            case GameState.WaitToStart:
                break;

            case GameState.CountdownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer > 0) {
                    return;
                }

                gamePlayingTimer = GAME_PLAYER_TOTAL_TIME;
                gameState = GameState.GamePlaying;
                GameStateChangedAction?.Invoke();
                break;

            case GameState.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer > 0) {
                    return;
                }

                gameState = GameState.GameOver;
                GameStateChangedAction?.Invoke();
                break;

            case GameState.GameOver:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public int GetCountdownToStartTimer() {
        return Mathf.CeilToInt(countDownToStartTimer);
    }

    public bool IsCountDownToStartState() {
        return gameState == GameState.CountdownToStart;
    }

    public bool IsGamePlayingState() {
        return gameState == GameState.GamePlaying;
    }

    public bool IsGameOverState() {
        return gameState == GameState.GameOver;
    }

    public float GetGamePlayingTimerNormalized() {
        return gamePlayingTimer / GAME_PLAYER_TOTAL_TIME;
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