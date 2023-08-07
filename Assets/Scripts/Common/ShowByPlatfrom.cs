using UnityEngine;

public class ShowByPlatfrom : MonoBehaviour {
    private void Awake() {
        var platform = Application.platform;
        if (platform is RuntimePlatform.Android or RuntimePlatform.IPhonePlayer) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}