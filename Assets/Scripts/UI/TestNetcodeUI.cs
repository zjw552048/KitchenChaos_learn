using UnityEngine;
using UnityEngine.UI;

public class TestNetcodeUI : MonoBehaviour {
    [SerializeField] private Button hostBtn;

    [SerializeField] private Button clientBtn;

    private void Start() {
        hostBtn.onClick.AddListener(() => {
            Debug.Log("StartHost!");
            MultiplayerNetworkManager.Instance.StartHost();
            Hide();
        });

        clientBtn.onClick.AddListener(() => {
            Debug.Log("StartClient!");
            MultiplayerNetworkManager.Instance.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}