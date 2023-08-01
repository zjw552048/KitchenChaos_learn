using UnityEngine;

public class LoadingUI : MonoBehaviour {
    private bool firstUpdate = true;

    private void Update() {
        if (!firstUpdate) {
            return;
        }

        firstUpdate = false;

        SceneLoader.LoadingSceneCallbacks();
    }
}