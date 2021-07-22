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
    Renderer m_Renderer;
    Color m_MouseOverColor = Color.red;
    Color m_OriginalColor;
    private void Awake()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        m_OriginalColor = m_Renderer.material.color;
    }
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
        // 작으면 캐릭터 이동
        if (Vector3.Distance(mouseDownPosition, mouseUpPosition) > clickDistance)
            return;
        if (actor && actor == Player.SelectedPlayer)
        { // 클릭한 위치에 actor가 있고, 플레이어가 있다면
            //영역표시
            // 첫번째 이동으로 갈 수 있는 곳을 추가
            ShowMoveDistance(actor.moveDistance);
        }
        Player.SelectedPlayer.OnTouch(transform.position);
    }

    void ShowMoveDistance(int moveDistance)
    {
        //var blocks = Physics.OverlapSphere(transform.position, actor.moveDistance);
        var rotate = Quaternion.Euler(0, 45, 0);
        Vector3 halfExtents = (moveDistance / Mathf.Sqrt(2)) * 0.99f * Vector3.one;
        var blocks = Physics.OverlapBox(transform.position, halfExtents, rotate);
        foreach (var item in blocks)
        {
            if (Player.SelectedPlayer.OnMoveable(item.transform.position, moveDistance))
                item.GetComponent<BlockInfo>()?.ToChangeRedColor();
        }
    }

    string debugTextPrefabString = "DebugTextPrefab";
    GameObject debutTextGos;
    internal Actor actor;

    public void UpdateDebugInfo()
    {
        if (debutTextGos == null)
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


    private void OnMouseOver()
    {
        if (actor)
            ActorStateUI.Instance.Show(actor);
    }

    public void ToChangeRedColor()
    {
        m_Renderer.material.color = m_MouseOverColor;
    }

    private void OnMouseExit()
    {
        if (actor)
            ActorStateUI.Instance.Close();
    }
}