using UnityEngine;
using System.Collections;

public class MapRotator : MonoBehaviour
{
    // 旋转动画的持续时间（秒）
    public float rotationDuration = 1f;
    
    // 标记当前是否正在旋转，防止重复启动协程
    private bool isRotating = false;

    /// <summary>
    /// 开始旋转地图。旋转过程中将暂停其他行动，旋转完毕后恢复时间缩放。
    /// </summary>
    public IEnumerator RotateMap()
    {
        if (isRotating)
            yield break;
        isRotating = true;
        
        // 地图旋转过程中通过暂停时间来暂停其他所有行为
        // 记录当前时间缩放，并将 Time.timeScale 设置为 0
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // 记录初始旋转角度和目标旋转角度（旋转180度）
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 0f, 180f);

        float elapsed = 0f;
        // 用 unscaledDeltaTime 驱动协程（即使时间缩放为0也能更新）
        while (elapsed < rotationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // 确保旋转到目标角度
        transform.rotation = targetRotation;

        // 恢复时间缩放，使地图中的其他行为继续运行
        Time.timeScale = originalTimeScale;
        isRotating = false;
    }
}
