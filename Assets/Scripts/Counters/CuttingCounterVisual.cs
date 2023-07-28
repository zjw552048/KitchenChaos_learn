using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour {
    [SerializeField] private Animator animator;
    
    private CuttingCounter counter;
    private static readonly int CUT = Animator.StringToHash("Cut");


    private void Awake() {
        counter = GetComponent<CuttingCounter>();
    }

    private void Start() {
        counter.PlayerCutKitchenObjectAction += OnPlayerCutKitchenObjectAction;
    }

    private void OnPlayerCutKitchenObjectAction() {
        animator.SetTrigger(CUT);
    }
}
