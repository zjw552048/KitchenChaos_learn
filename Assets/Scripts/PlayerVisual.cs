using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    private Material material;

    private void Awake() {
        // 复制material
        material = new Material(headMeshRenderer.material);
        // 重新赋值material，以便后续修改颜色时不会影响到其他playerVisual
        headMeshRenderer.material = material;
        bodyMeshRenderer.material = material;
    }

    public void SetMaterialColor(Color color) {
        material.color = color;
    }
}