using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour {
    private void Awake() {
        // 执行清理静态字段逻辑
        Player.ResetStaticData();
    }
}