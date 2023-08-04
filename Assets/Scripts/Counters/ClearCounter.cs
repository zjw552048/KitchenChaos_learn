public class ClearCounter : BaseCounter {
    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {

                var playerHoldKitchenObject = player.GetKitchenObject();
                var counterHoldKitchenObject = GetKitchenObject();

                if (playerHoldKitchenObject.TryGetPlate(out var plate)) {
                    // 如果player持有的是plate，则尝试将counter上的kitchenObject放入plate
                    if (plate.TryAddIngredient(counterHoldKitchenObject.GetKitchenObjectSo())) {
                        MultiplayerNetworkManager.Instance.DespawnKitchenObjectByServer(counterHoldKitchenObject);
                    }
                }

                if (counterHoldKitchenObject.TryGetPlate(out plate)) {
                    // 如果counter放置的是plate，则尝试将player持有的kitchenObject放入plate
                    if (plate.TryAddIngredient(playerHoldKitchenObject.GetKitchenObjectSo())) {
                        MultiplayerNetworkManager.Instance.DespawnKitchenObjectByServer(playerHoldKitchenObject);
                    }
                }
            } else {
                // player手持物体，counter空闲，放下物体
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        } else {
            if (HasKitchenObject()) {
                // player未手持物体，counter被占用，拾取物体
                GetKitchenObject().SetKitchenObjectParent(player);
            } else {
                // player未手持物体，counter空闲，无逻辑
            }
        }
    }

    public override void InteractAlternate(Player player) {
    }
}