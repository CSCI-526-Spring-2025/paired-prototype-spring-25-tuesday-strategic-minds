using UnityEngine;

public class ShiftableObject : MonoBehaviour
{
    private Collider2D col;
    // blackTile 为 true 表示对应的 ShiftableObject 是翻转之前的地面，在游戏中标识为黑色的TileMap物体
    public bool blackTile = true;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        // sr = GetComponent<SpriteRenderer>();
    }

    // 设置反转状态：shifted = true 表示进入反转状态
    public void SetShiftState(bool shifted)
    {
        if ((shifted && blackTile) || (!shifted && !blackTile))
        {
            // 在反转状态下，禁用碰撞
            col.enabled = false;
            // 这里可以添加shift后的特效
            // sr.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            // 恢复正常状态
            col.enabled = true;
            // sr.color = Color.white;
        }
    }
}
