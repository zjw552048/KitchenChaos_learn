using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipsRefsSo audioClipsRefsSo;

    private float volumeScaler;
    private const float MODIFY_VOLUME_STEP = 0.1f;
    private const float MAX_VOLUME = 1.0f;
    private const string SOUND_VOLUME_KEY = "SoundVolume";

    private void Awake() {
        Instance = this;

        volumeScaler = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, MAX_VOLUME / 2);
    }

    public float GetVolume() {
        return volumeScaler;
    }

    public void ModifyVolume() {
        volumeScaler += MODIFY_VOLUME_STEP;
        if (volumeScaler > MAX_VOLUME) {
            volumeScaler = 0f;
        }

        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, volumeScaler);
    }

    #region playSound logic

    public void PlayChopSounds(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.chop, pos, volume);
    }

    public void PlayDeliveryFail(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.deliveryFail, pos, volume);
    }

    public void PlayDeliverySuccess(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.deliverySuccess, pos, volume);
    }

    public void PlayFootstep(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.footStep, pos, volume);
    }

    public void PlayKitchenObjectDrop(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.kitchenObjectDrop, pos, volume);
    }

    public void PlayKitchenObjectPickUp(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.kitchenObjectPickUp, pos, volume);
    }

    public void PlayTrash(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.trash, pos, volume);
    }

    public void PlayCountdown(float volume = 1f) {
        PlaySound(audioClipsRefsSo.countdown, new Vector3(), volume);
    }

    public void PlayWarning(Vector3 pos, float volume = 1f) {
        PlaySound(audioClipsRefsSo.warning, pos, volume);
    }

    private void PlaySound(IReadOnlyList<AudioClip> audioClips, Vector3 pos, float volume = 1f) {
        PlaySound(audioClips[Random.Range(0, audioClips.Count)], pos, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 pos, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, pos, volume * volumeScaler);
    }

    #endregion

    #region getAudioClip logic

    public AudioClip GetStoveSizzleClip() {
        return audioClipsRefsSo.stoveSizzle;
    }

    #endregion
}