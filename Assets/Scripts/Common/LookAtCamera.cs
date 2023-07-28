using System;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class LookAtCamera : MonoBehaviour {
    private enum Mode {
        LookAtCamera,
        LookAtCameraInverted,
        CameraForward,
        CameraForwardInverted,
    }

    [SerializeField] private Mode mode;

    private Transform mainCameraTransform;
    private Transform selfTransform;

    private void Awake() {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        mainCameraTransform = Camera.main.transform;
        selfTransform = transform;
    }

    private void LateUpdate() {
        switch (mode) {
            case Mode.LookAtCamera:
                transform.LookAt(mainCameraTransform);
                break;

            case Mode.LookAtCameraInverted:
                var invertedDir = selfTransform.position - mainCameraTransform.position;
                transform.LookAt(selfTransform.position + invertedDir);
                break;

            case Mode.CameraForward:
                selfTransform.forward = mainCameraTransform.forward;
                break;

            case Mode.CameraForwardInverted:
                selfTransform.forward = -mainCameraTransform.forward;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}