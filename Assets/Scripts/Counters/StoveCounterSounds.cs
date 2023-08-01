using UnityEngine;

public class StoveCounterSounds : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.StoveStateChangedAction += OnStoveStateChangedAction;
        audioSource.clip = SoundManager.Instance.GetStoveSizzleClip();
    }

    private void OnStoveStateChangedAction(StoveCounter.StoveState stoveState) {
        var needSound = stoveState is StoveCounter.StoveState.Frying or StoveCounter.StoveState.Burning;
        if (needSound) {
            audioSource.Play();
        } else {
            audioSource.Stop();
        }
    }
}