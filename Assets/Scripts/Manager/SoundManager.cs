using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipsRefsSo audioClipsRefsSo;

    private void Awake() {
        Instance = this;
    }

    public void playChopSounds(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.chop, pos, volume);
    }

    public void playDeliveryFail(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.deliveryFail, pos, volume);
    }

    public void playDeliverySuccess(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.deliverySuccess, pos, volume);
    }

    public void playFootstep(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.footStep, pos, volume);
    }

    public void playKitchenObjectDrop(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.kitchenObjectDrop, pos, volume);
    }

    public void playKitchenObjectPickUp(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.kitchenObjectPickUp, pos, volume);
    }

    public AudioClip GetStoveSizzleClip() {
        return audioClipsRefsSo.stoveSizzle;
    }

    public void playTrash(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.trash, pos, volume);
    }

    public void playWarning(Vector3 pos, float volume = 1f) {
        playSound(audioClipsRefsSo.warning, pos, volume);
    }

    private void playSound(IReadOnlyList<AudioClip> audioClips, Vector3 pos, float volume) {
        AudioSource.PlayClipAtPoint(audioClips[Random.Range(0, audioClips.Count)], pos, volume);
    }

    private void playSound(AudioClip audioClip, Vector3 pos, float volume) {
        AudioSource.PlayClipAtPoint(audioClip, pos, volume);
    }
}