using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;
    private static readonly int IS_FLASHING = Animator.StringToHash("IsFlashing");

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        stoveCounter.RefreshProgressAction += OnRefreshProgressAction;
        
        animator.SetBool(IS_FLASHING, false);
    }

    private void OnRefreshProgressAction(float progress) {
        var isFlashing = stoveCounter.IsBurningState() && progress > StoveCounter.BURNED_WARNING_PROGRESS;
        animator.SetBool(IS_FLASHING, isFlashing);
    }
    
    
}
