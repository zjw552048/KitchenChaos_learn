using Unity.Netcode;

public class TrashCounter : BaseCounter {
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            return;
        }

        KitchenObject.DespawnKitchenObject(player.GetKitchenObject());

        InteractLogicServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        SoundManager.Instance.PlayTrash(transform.position);
    }

    public override void InteractAlternate(Player player) {
    }
}