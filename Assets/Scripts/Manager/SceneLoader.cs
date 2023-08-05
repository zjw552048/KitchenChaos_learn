using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader {
    public enum SceneName {
        MainMenuScene,
        LoadingScene,
        GameScene,
        LobbyScene,
        CharacterSelectScene,
    }

    private static SceneName targetSceneName;

    public static void LoadScene(SceneName sceneName) {
        targetSceneName = sceneName;
        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
    }

    public static void LoadNetworkScene(SceneName sceneName) {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
    }

    public static void LoadingSceneCallbacks() {
        SceneManager.LoadScene(targetSceneName.ToString());
    }
}