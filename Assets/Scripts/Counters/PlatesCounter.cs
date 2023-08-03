using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    [SerializeField] private KitchenObjectSo plateKitchenObjectSo;

    private float spawnPlateTimer;
    private const float SPAWN_PLATE_INTERVAL = 4f;
    private int currentPlateCount;
    private const int MAX_SPAWN_PLATE_COUNT = 5;

    public event Action SpawnPlateAction;
    public event Action RemovePlateAction;

    private void Update() {
        if (!IsServer) {
            return;
        }

        if (!MainGameManager.Instance.IsGamePlayingState()) {
            return;
        }

        if (currentPlateCount >= MAX_SPAWN_PLATE_COUNT) {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer < SPAWN_PLATE_INTERVAL) {
            return;
        }

        spawnPlateTimer -= SPAWN_PLATE_INTERVAL;

        SpawnPlateServerRpc();
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc() {
        currentPlateCount++;
        SpawnPlateAction?.Invoke();
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            return;
        }

        if (currentPlateCount <= 0) {
            return;
        }

        KitchenObject.SpawnKitchenObject(plateKitchenObjectSo, player);

        PlayerTakePlateActionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerTakePlateActionServerRpc() {
        PlayerTakePlateActionClientRpc();
    }

    [ClientRpc]
    private void PlayerTakePlateActionClientRpc() {
        currentPlateCount--;
        RemovePlateAction?.Invoke();
    }

    public override void InteractAlternate(Player player) {
    }
}