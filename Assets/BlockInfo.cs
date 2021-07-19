using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockType
{
    Walkable,
    Water,
}
public class BlockInfo : MonoBehaviour
{
    public BlockType blockType;
    Vector3 mouseDownPosition;
    public float clickDistance = 1;
    private void OnMouseDown()
    {
        mouseDownPosition = Input.mousePosition;
    }
    private void OnMouseUp()
    {
        // 다운 - 업 거리가 특정 거리보다 크면 리턴
        var mouseUpPosition = Input.mousePosition;
        if (Vector3.Distance(mouseDownPosition, mouseUpPosition) > clickDistance)
            return;
        // 작으면 캐릭터 이동
        GroundManager.Instance.OnTouch(transform.position);
    }
}
