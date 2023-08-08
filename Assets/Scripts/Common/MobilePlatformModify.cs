using UnityEngine;

public class MobilePlatformModify : MonoBehaviour {
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
        // 修改stick deadZone
        PlayerInput.Instance.ModifyMovementStickDeadZone();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}