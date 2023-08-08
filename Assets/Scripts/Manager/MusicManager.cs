using UnityEngine;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

    private float volume;
    private const float MODIFY_VOLUME_STEP = 0.1f;
    private const float MAX_VOLUME = 1.0f;
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Awake() {
        Instance = this;

        volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, MAX_VOLUME / 10);
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
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}