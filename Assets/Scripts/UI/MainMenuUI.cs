using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button multiplayerBtn;
    [SerializeField] private Button singlePlayerBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private GameObject logo;

    private void Awake() {
        // 屏幕常亮
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start() {
        Time.timeScale = 1f;
        multiplayerBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.SinglePlayerMode = false;
            SceneLoader.LoadScene(SceneLoader.SceneName.LobbyScene);
        });
        singlePlayerBtn.onClick.AddListener(() => {
            MultiplayerNetworkManager.SinglePlayerMode = true;
            SceneLoader.LoadScene(SceneLoader.SceneName.LobbyScene);
        });

        quitBtn.onClick.AddListener(Application.Quit);
    }

    #region 补丁逻辑，未知原因造成首次进入场景时，UI元素的高度不正确，位置异常。

    private bool hasFixUiPosition;

    private void TryFixUIPosition() {
        if (hasFixUiPosition) {
            return;
        }

        var canvasTransform = (RectTransform) transform.parent;
        if (!(Math.Abs(canvasTransform.rect.height - 1080) < 1)) {
            return;
        }

        Debug.LogWarning("success fix ui position!");
        hasFixUiPosition = true;

        var multiplayerBtnTransform = (RectTransform) multiplayerBtn.transform;
        multiplayerBtnTransform.anchoredPosition = new Vector2(105, 430);
        var singlePlayerBtnTransform = (RectTransform) singlePlayerBtn.transform;
        singlePlayerBtnTransform.anchoredPosition = new Vector2(105, 240);
        var quitBtnTransform = (RectTransform) quitBtn.transform;
        quitBtnTransform.anchoredPosition = new Vector2(105, 70);
        var logoTransform = (RectTransform) logo.transform;
        logoTransform.anchoredPosition = new Vector2(0, -50);
    }

    private void Update() {
        TryFixUIPosition();
    }

    #endregion
}