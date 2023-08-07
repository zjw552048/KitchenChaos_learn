using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySingleUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI lobbyNameText;

    private Lobby lobby;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => { GameLobbyManager.Instance.JoinById(lobby.Id); });
    }

    public void SetLobby(Lobby targetLobby) {
        lobby = targetLobby;
        lobbyNameText.text = lobby.Name;
        Debug.Log("LobbyInfo: " + lobby.Name + lobby.LobbyCode);
    }
}