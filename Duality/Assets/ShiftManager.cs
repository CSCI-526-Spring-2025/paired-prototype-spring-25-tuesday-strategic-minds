using UnityEngine;
using System.Collections;

public class ShiftManager : MonoBehaviour
{
    // 当前反转状态，默认为 false（正常状态）
    public bool isShifted = false;

    public GameObject player;

    public MapRotator mapRotator;

    void Update()
    {
        // 按下 Shift 键时切换状态
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            // 判断当前是否可以翻转
            if (pc != null && pc.CanShift)
            {
                StartCoroutine(ToggleShiftCoroutine());
            }
            else
            {
                Debug.Log("无法 shift：玩家不处于允许状态（例如没有接触平台）");
            }
        }
    }

    private IEnumerator ToggleShiftCoroutine()
    {
        isShifted = !isShifted;
        
        // 玩家翻转协程
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.OnShift(isShifted);
            
            // 等待玩家旋转完成
            yield return new WaitWhile(() => pc.isRotating);
        }

        // ShiftableObject 的碰撞状态修改
        ShiftableObject[] shiftables = FindObjectsOfType<ShiftableObject>();
        foreach (var shiftable in shiftables)
        {
            shiftable.SetShiftState(isShifted);
        }

        // 地图翻转协程
        if (mapRotator != null)
        {
            yield return StartCoroutine(mapRotator.RotateMap());
        }
        
    }
}
