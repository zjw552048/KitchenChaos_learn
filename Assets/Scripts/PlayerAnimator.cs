using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour {
    private static readonly int IS_WALKING = Animator.StringToHash("IsWalking");

    private Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }

        var isWalking = player.IsWalking();
        UpdatePlayerAnimatorServerRpc(isWalking);
        // animator.SetBool(IS_WALKING, player.IsWalking());
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerAnimatorServerRpc(bool isWalking) {
        animator.SetBool(IS_WALKING, isWalking);
    }
}