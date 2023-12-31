using UnityEngine;

public class BaseCounterSelectedVisual : MonoBehaviour {
    [SerializeField] private GameObject counterSelectedVisual;

    private BaseCounter counter;

    private void Awake() {
        counter = GetComponent<BaseCounter>();
    }

    private void Start() {
        if (Player.LocalInstance != null) {
            Player.LocalInstance.SelectedCounterChangedAction += OnSelectedCounterChangedAction;
        } else {
            Player.OwnerPlayerSpawnedAction += OnOwnerPlayerSpawnedAction;
        }
    }

    private void OnOwnerPlayerSpawnedAction() {
        if (Player.LocalInstance != null) {
            Player.LocalInstance.SelectedCounterChangedAction += OnSelectedCounterChangedAction;
        }
    }

    private void OnDestroy() {
        if (Player.LocalInstance != null) {
            Player.LocalInstance.SelectedCounterChangedAction -= OnSelectedCounterChangedAction;
        }
    }

    private void OnSelectedCounterChangedAction(BaseCounter selectedCounter) {
        counterSelectedVisual.SetActive(counter == selectedCounter);
    }
}