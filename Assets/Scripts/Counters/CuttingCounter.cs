public class CuttingCounter : BaseCounter {

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (HasKitchenObject()) {
                // player手持物体，counter被占用，无逻辑
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
        if (!HasKitchenObject()) {
            return;
        }

        var kitchenObject = GetKitchenObject();
        var kitchenObjectSo = kitchenObject.GetKitchenObjectSo();
        var cutOutputSo = kitchenObjectSo.cutOutputSo;
        if (cutOutputSo == null) {
            return;
        }

        kitchenObject.DestroySelf();
        KitchenObject.SpawnKitchenObject(cutOutputSo, this);
    }
}