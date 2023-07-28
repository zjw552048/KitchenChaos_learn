using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour {
    [SerializeField] private Animator animator;

    private ContainerCounter counter;
    private static readonly int OPEN_CLOSE = Animator.StringToHash("OpenClose");

    private void Awake() {
        counter = GetComponent<ContainerCounter>();
    }

    private void Start() {
        counter.PlayerHoldKitchenObjectAction += OnPlayerHoldKitchenObjectAction;
    }

    private void OnPlayerHoldKitchenObjectAction() {
        animator.SetTrigger(OPEN_CLOSE);
    }
}