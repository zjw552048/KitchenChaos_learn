using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private static readonly int IS_WALKING = Animator.StringToHash("IsWalking");
    
    private Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
