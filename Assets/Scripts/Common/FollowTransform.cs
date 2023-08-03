using UnityEngine;

public class FollowTransform : MonoBehaviour {
    private Transform followTargetTransform;

    public void SetTargetTransform(Transform targetTransform) {
        followTargetTransform = targetTransform;
    }

    private void LateUpdate() {
        if (followTargetTransform == null) {
            return;
        }

        transform.position = followTargetTransform.position;
        transform.rotation = followTargetTransform.rotation;
    }
}
