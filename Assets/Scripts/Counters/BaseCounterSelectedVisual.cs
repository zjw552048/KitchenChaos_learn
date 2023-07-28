using UnityEngine;

public class BaseCounterSelectedVisual : MonoBehaviour {

    [SerializeField]private GameObject counterSelectedVisual;
    
    private BaseCounter counter;

    private void Awake() {
        counter = GetComponent<BaseCounter>();
    }

    private void Start() {
        Player.Instance.SelectedCounterChangedAction += OnSelectedCounterChangedAction;
    }

    private void OnDestroy() {
        Player.Instance.SelectedCounterChangedAction -= OnSelectedCounterChangedAction;
    }

    private void OnSelectedCounterChangedAction(BaseCounter selectedCounter) {
        counterSelectedVisual.SetActive(counter == selectedCounter);
    }
}