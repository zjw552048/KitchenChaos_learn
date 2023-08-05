using UnityEngine;
using UnityEngine.UI;

public class SelectCharaterUI : MonoBehaviour {
    [SerializeField] private Button readyBtn;

    private void Start() {
        readyBtn.onClick.AddListener(() => { SelectCharacterReadyManager.Instance.SetPlayerReady(); });
    }
}