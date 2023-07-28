using UnityEngine;

public class SelectedCounterVistual : MonoBehaviour {

    private ClearCounter counter;
    [SerializeField]private GameObject counterSelectedVisual;

    private void Awake() {
        counter = GetComponent<ClearCounter>();
    }

    private void Start() {
        Player.Instance.SelectedCounterChanged += InstanceOnSelectedCounterChanged;
    }

    private void OnDestroy() {
        Player.Instance.SelectedCounterChanged -= InstanceOnSelectedCounterChanged;
    }

    private void InstanceOnSelectedCounterChanged(ClearCounter selectedCounter) {
        counterSelectedVisual.SetActive(counter == selectedCounter);
    }
}