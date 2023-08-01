using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    [SerializeField] private Player player;

    private float playerFootstepTimer;
    private const float PLAYER_FOOTSTEP_INTERVAL = 0.2f;

    private void Update() {
        playerFootstepTimer += Time.deltaTime;
        if (playerFootstepTimer < PLAYER_FOOTSTEP_INTERVAL) {
            return;
        }

        playerFootstepTimer -= PLAYER_FOOTSTEP_INTERVAL;
        if (!player.IsWalking()) {
            return;
        }

        SoundManager.Instance.playFootstep(transform.position);
    }
}