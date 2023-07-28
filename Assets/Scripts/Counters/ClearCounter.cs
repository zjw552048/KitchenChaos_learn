using UnityEngine;

public class ClearCounter : BaseCounter {
    public override void Interact(Player player) {
        Debug.Log("Interact with ClearCounter " + transform);
    }
}