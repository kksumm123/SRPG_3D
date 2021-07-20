using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Flags]
public enum BlockType
{
    None = 0,
    Walkable = 1 << 0,
    Water = 1 << 1,
    Player = 1 << 2,
    Monster = 1 << 3,
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
        Player.SelectedPlayer.OnTouch(transform.position);
    }
    string debugTextPrefabString = "DebugTextPrefab";
    GameObject debutTextGos;

    public void UpdateDebugInfo()
    {
        if(debutTextGos == null)
        {
            GameObject textMeshGo = Instantiate((GameObject)Resources.Load(debugTextPrefabString), transform);
            debutTextGos = textMeshGo;
            textMeshGo.transform.localPosition = Vector3.zero;
        }

        StringBuilder debugText = new StringBuilder();
        ContaingText(debugText, BlockType.Water);
        ContaingText(debugText, BlockType.Player);
        ContaingText(debugText, BlockType.Monster);

        GetComponentInChildren<TextMesh>().text = debugText.ToString();
    }
    void ContaingText(StringBuilder sb, BlockType walkable)
    {
        if (blockType.HasFlag(walkable))
            sb.AppendLine(walkable.ToString());
    }
}