using UnityEngine;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

    private float volume;
    private const float MODIFY_VOLUME_STEP = 0.1f;
    private const float MAX_VOLUME = 1.0f;

    private void Awake() {
        Instance = this;

        volume = 0.5f;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    public float GetVolume() {
        return volume;
    }

    public void ModifyVolume() {
        volume += MODIFY_VOLUME_STEP;
        if (volume > MAX_VOLUME) {
            volume = 0f;
        }

        audioSource.volume = volume;
    }
}