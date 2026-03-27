using UnityEngine;

[RequireComponent(typeof(BoxCollider))] // 確保該物件一定有 BoxCollider
[RequireComponent(typeof(LineRenderer))] // 自動加入 LineRenderer
public class ShowHitbox : MonoBehaviour
{
    private BoxCollider boxCollider;
    private LineRenderer lineRenderer;

    // 用於儲存 Cube 的 8 個頂點座標
    private Vector3[] vertices = new Vector3[8];

    [Header("Debug Settings")]
    [SerializeField] private float lineWidth = 0.05f; // 線條粗細
    [SerializeField] private bool isPlayerHitbox;
    [SerializeField] private bool isPlayerSkillHitbox;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        lineRenderer = GetComponent<LineRenderer>();

        // 初始化 LineRenderer 設定
        if (isPlayerHitbox)
        {
            lineRenderer.material = DebugManager.Instance.playerHitbox;
        }
        else if (isPlayerSkillHitbox)
        {
            lineRenderer.material = DebugManager.Instance.playerSkillHitbox;
        }
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 16; // 畫出一個完整的 Cube 邊框需要 16 個點
        lineRenderer.useWorldSpace = true; // 使用世界座標，這樣旋轉時線條才正確
        
        // 確保 LineRenderer 不會擋住射線檢測
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
    }

    void Update()
    {
        // 如果關閉顯示，或是物件被禁用，就隱藏線條
        if (!DebugManager.Instance.showHitbox || !boxCollider.enabled)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        DrawBox();
    }

    // 計算並繪製 BoxCollider 的邊框
    void DrawBox()
    {
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size / 2f; // 取得半徑

        // 計算局部空間 (Local Space) 的 8 個頂點
        vertices[0] = center + new Vector3(-size.x, -size.y, -size.z); // 左下後
        vertices[1] = center + new Vector3(size.x, -size.y, -size.z);  // 右下後
        vertices[2] = center + new Vector3(size.x, size.y, -size.z);   // 右上後
        vertices[3] = center + new Vector3(-size.x, size.y, -size.z);  // 左上後
        
        vertices[4] = center + new Vector3(-size.x, -size.y, size.z);  // 左下前
        vertices[5] = center + new Vector3(size.x, -size.y, size.z);   // 右下前
        vertices[6] = center + new Vector3(size.x, size.y, size.z);    // 右上前
        vertices[7] = center + new Vector3(-size.x, size.y, size.z);   // 左上前

        // 將頂點轉換為世界空間 (World Space)
        for (int i = 0; i < 8; i++)
        {
            vertices[i] = transform.TransformPoint(vertices[i]);
        }

        // 設定 LineRenderer 的點序，連成一個 Cube
        // 底面
        lineRenderer.SetPosition(0, vertices[0]);
        lineRenderer.SetPosition(1, vertices[1]);
        lineRenderer.SetPosition(2, vertices[5]);
        lineRenderer.SetPosition(3, vertices[4]);
        lineRenderer.SetPosition(4, vertices[0]); // 回到起點

        // 垂直支柱 1
        lineRenderer.SetPosition(5, vertices[3]);
        
        // 頂面
        lineRenderer.SetPosition(6, vertices[2]);
        lineRenderer.SetPosition(7, vertices[6]);
        lineRenderer.SetPosition(8, vertices[7]);
        lineRenderer.SetPosition(9, vertices[3]); // 回到起點

        // 垂直支柱 2 (連回底面)
        lineRenderer.SetPosition(10, vertices[7]);
        lineRenderer.SetPosition(11, vertices[4]);
        
        // 垂直支柱 3 (連回底面)
        lineRenderer.SetPosition(12, vertices[5]);
        lineRenderer.SetPosition(13, vertices[6]);
        
        // 垂直支柱 4 (連回底面)
        lineRenderer.SetPosition(14, vertices[2]);
        lineRenderer.SetPosition(15, vertices[1]);
    }
}