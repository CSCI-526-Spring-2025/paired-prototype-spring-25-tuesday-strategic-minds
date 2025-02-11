using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded = false;    // isGrounded 标识玩家是否在地面上
    public bool CanShift => isGrounded; // CanShift 为 true 才能进行 shift 操作
    private Vector2 groundContactPoint;
    private SpriteRenderer sr;

    // 控制旋转动画
    public bool isRotating { get; private set; } = false;   // 防止重复启动协程，并且地图需要 isRotating 为 false 才能旋转
    public float rotationDuration = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 基本移动
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 跳跃
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, -jumpForce);
        // }
    }


    // 这个落地状态可能不太精确，可以改进
    // 当与平台碰撞时，判断是否存在下方接触并记录接触点
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 如果接触点法线的 y 分量大于 0.5，认为是下方接触
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    groundContactPoint = contact.point;
                    // Debug.Log("Grounded! Contact point: " + groundContactPoint);
                    break;
                }
            }
        }
    }

    // 当与平台分离时，取消落地状态
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }

    // OnShift 在 ShiftManager 切换状态时被调用。
    // 除了改变颜色，还触发玩家旋转（如果满足条件）。
    public void OnShift(bool shifted)
    {
        Debug.Log("OnShift called, shifted = " + shifted);

        if (sr != null)
        {
            // 根据状态修改颜色，例如：shifted 为 true 时显示白色，false 时显示黑色
            sr.color = shifted ? Color.white : Color.black;
        }

        StartCoroutine(RotatePlayerCoroutine());

    }

    /// <summary>
    /// 协程：使玩家绕着最近一次与平台接触的点旋转 180°，实现平滑动画。
    /// </summary>
    private IEnumerator RotatePlayerCoroutine()
    {
        isRotating = true;
        
        // 为避免物理干扰，临时禁用物理模拟
        rb.simulated = false;

        // 记录初始状态
        Vector3 initialPos = transform.position;
        Quaternion initialRot = transform.rotation;
        
        // 计算玩家底部中心点作为旋转轴：
        // sr.bounds.min.y 是 Sprite 的底边，sr.bounds.center.x 是水平中心
        Vector3 pivot = new Vector3(sr.bounds.center.x, sr.bounds.min.y, transform.position.z);

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            // 计算当前插值的旋转角度（从 0° 到 180°）
            float angle = Mathf.Lerp(0f, 180f, elapsed / rotationDuration);

            // 每帧重置到初始状态，再围绕 pivot 旋转
            transform.position = initialPos;
            transform.rotation = initialRot;
            transform.RotateAround(pivot, Vector3.forward, angle);

            yield return null;
        }
        
        // 确保最终精确旋转 180°
        transform.position = initialPos;
        transform.rotation = initialRot;
        transform.RotateAround(pivot, Vector3.forward, 180f);
        
        // 恢复物理模拟
        rb.simulated = true;
        // 玩家旋转结束
        isRotating = false;
    }

}
