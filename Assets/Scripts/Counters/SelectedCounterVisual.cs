using UnityEngine;

public class SelectedCounterVistual : MonoBehaviour {

    [SerializeField]private GameObject counterSelectedVisual;
    
    private BaseCounter counter;

    private void Awake() {
        counter = GetComponent<BaseCounter>();
    }

    private void Start() {
        Player.Instance.SelectedCounterChanged += InstanceOnSelectedCounterChanged;
    }

    private void OnDestroy() {
        Player.Instance.SelectedCounterChanged -= InstanceOnSelectedCounterChanged;
    }

    private void InstanceOnSelectedCounterChanged(BaseCounter selectedCounter) {
        counterSelectedVisual.SetActive(counter == selectedCounter);
    }
}