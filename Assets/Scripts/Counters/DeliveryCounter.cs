public class DeliveryCounter : BaseCounter {
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            return;
        }

        var playerHoldKitchenObject = player.GetKitchenObject();
        if (!playerHoldKitchenObject.TryGetPlate(out var plateKitchenObject)) {
            return;
        }

        DeliveryManager.Instance.DeliveryRecipe(plateKitchenObject);
        playerHoldKitchenObject.DestroySelf();
    }

    public override void InteractAlternate(Player player) {
    }
}