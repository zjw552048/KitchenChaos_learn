using UnityEngine;

public class StoveCounterVisual : MonoBehaviour {
    
    [SerializeField] private GameObject particlesGameObject;
    [SerializeField] private GameObject stoveOnGameObjects;
    private StoveCounter stoveCounter;

    private void Awake() {
        stoveCounter = GetComponent<StoveCounter>();
    }

    private void Start() {
        stoveCounter.StoveStateChangedAction += OnStoveStateChangedAction;
    }

    private void OnStoveStateChangedAction(StoveCounter.StoveState stoveState) {
        var needShowEffect = stoveState is StoveCounter.StoveState.Frying or StoveCounter.StoveState.Burning;
        particlesGameObject.SetActive(needShowEffect);
        stoveOnGameObjects.SetActive(needShowEffect);
    }
}