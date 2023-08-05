using UnityEngine;
using UnityEngine.UI;

public class SelectCharaterUI : MonoBehaviour {
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button readyBtn;

    private void Start() {
        mainMenuBtn.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.SceneName.MainMenuScene); });
        readyBtn.onClick.AddListener(() => { SelectCharacterReadyManager.Instance.SetPlayerReady(); });
    }
}