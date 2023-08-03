using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetcodeUI : MonoBehaviour {
    [SerializeField] private Button hostBtn;

    [SerializeField] private Button clientBtn;

    private void Start() {
        hostBtn.onClick.AddListener(() => {
            Debug.Log("StartHost!");
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        clientBtn.onClick.AddListener(() => {
            Debug.Log("StartClient!");
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}