using System;
using UnityEngine;

public class MainGameManager : MonoBehaviour {
    public static MainGameManager Instance { get; private set; }

    public event Action OnGameStateChangedAction;

    private enum GameState {
        WaitToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState gameState;
    private float waitToStartTimer = 1f;
    private float countDownToStartTimer = 3f;
    private float gamePlayingTimer = 10f;

    private void Awake() {
        Instance = this;
        gameState = GameState.WaitToStart;
    }

    private void Update() {
        switch (gameState) {
            case GameState.WaitToStart:
                waitToStartTimer -= Time.deltaTime;
                if (waitToStartTimer > 0) {
                    return;
                }

                gameState = GameState.CountdownToStart;
                OnGameStateChangedAction?.Invoke();

                break;

            case GameState.CountdownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer > 0) {
                    return;
                }

                gameState = GameState.GamePlaying;
                OnGameStateChangedAction?.Invoke();
                break;

            case GameState.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer > 0) {
                    return;
                }

                gameState = GameState.GameOver;
                OnGameStateChangedAction?.Invoke();
                break;

            case GameState.GameOver:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log("GameState: " + gameState);
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
}