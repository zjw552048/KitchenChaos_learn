using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameHostLostUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        Hide();
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    private void OnClientDisconnectCallback(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}