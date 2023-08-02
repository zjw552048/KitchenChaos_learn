using UnityEngine;

public class StoveCounterSounds : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private bool needPlayWarningSound;
    private float playWarningSoundTimer;
    private const float PLAY_WARNING_SOUND_INTERVAL = 0.2f;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.StoveStateChangedAction += OnStoveStateChangedAction;
        stoveCounter.RefreshProgressAction += OnRefreshProgressAction;
        audioSource.clip = SoundManager.Instance.GetStoveSizzleClip();
    }

    private void Update() {
        TryPlayWarningSound();
    }

    private void TryPlayWarningSound() {
        if (!needPlayWarningSound) {
            return;
        }

        playWarningSoundTimer += Time.deltaTime;
        if (playWarningSoundTimer < PLAY_WARNING_SOUND_INTERVAL) {
            return;
        }

        playWarningSoundTimer -= PLAY_WARNING_SOUND_INTERVAL;
        SoundManager.Instance.PlayWarning(stoveCounter.transform.position);
    }

    private void OnRefreshProgressAction(float progress) {
        needPlayWarningSound = stoveCounter.IsBurningState() && progress > StoveCounter.BURNED_WARNING_PROGRESS;
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