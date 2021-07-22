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
    public static List<BlockInfo> highlightedMoveableArea = new List<BlockInfo>();

    Renderer m_Renderer;
    Color moveableColor = Color.blue;
    Color m_OriginalColor;
    private void Awake()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        m_OriginalColor = m_Renderer.material.color;
    }
    public BlockType blockType;
    Vector3 mouseDownPosition;
    public float clickDistance = 1;
    bool isChangedColor = false;
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

        switch (StageManager.GameState)
        {
            case GameStateType.SelectPlayer:
                SelectPlayer();
                break;
            case GameStateType.SelectBlockOrAttackTarget:
                SelectBlockOrAttackTarget();
                break;
            case GameStateType.SelectToAttackTarget:
                SelectToAttackTarget();
                break;
            case GameStateType.AttackTartget:
                AttackTartget();
                break;

            case GameStateType.NotInit:
            case GameStateType.IngPlayerMove:
            case GameStateType.MonsterTurn:
                Debug.Log($"블럭을 클릭할 수 없는 상태 : {StageManager.GameState}");
                break;
        }

        if (actor && actor == Player.SelectedPlayer)
        { // 클릭한 위치에 actor가 있고, 플레이어가 있다면
          //영역표시
          // 첫번째 이동으로 갈 수 있는 곳을 추가
            ShowMoveDistance(actor.moveDistance);
        }
        if (transform.position != Player.SelectedPlayer.transform.position)
            Player.SelectedPlayer.MoveToPosition(transform.position);

    }
    void SelectPlayer()
    {
        if (actor == null)
            return;
        if (actor.GetType() == typeof(Player))
        {
            //Player.SelectedPlayer = actor as Player;
            Player.SelectedPlayer = (Player)actor;

            // 이동가능한 영역 표시
            ShowMoveDistance(Player.SelectedPlayer.moveDistance);

            // 현재 위치에서 공격 가능한 영역 표시
            Player.SelectedPlayer.ShowAttackableArea();
            StageManager.GameState = GameStateType.SelectBlockOrAttackTarget;
        }
    }
    private void SelectBlockOrAttackTarget()
    {// 공격 대상이 있다면 공격 하자 (Actor == 몬스터 라면
        if (actor)
        {
            if (Player.SelectedPlayer.CanAttackTarget(actor))
                Player.SelectedPlayer.AttackToTarget(actor);
        }
        else
        {
            if (highlightedMoveableArea.Contains(this))
            {
                Player.SelectedPlayer.MoveToPosition(transform.position);
                ClearMoveableArea();
                StageManager.GameState = GameStateType.IngPlayerMove;
            }
        }
    }

    private void SelectToAttackTarget()
    {
    }

    private void AttackTartget()
    {
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
            {
                var itemBlocks = item.GetComponent<BlockInfo>();
                if (itemBlocks != null)
                {
                    itemBlocks.ToChangeBlueColor();
                    highlightedMoveableArea.Add(itemBlocks);
                }
            }
        }
    }
    void ClearMoveableArea()
    {
        highlightedMoveableArea.ForEach(x => x.ToChangeOriginColor());
        highlightedMoveableArea.Clear();
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
    public void ContaingText(StringBuilder sb, BlockType walkable)
    {
        if (blockType.HasFlag(walkable))
            sb.AppendLine(walkable.ToString());
    }


    private void OnMouseOver()
    {
        if (actor)
            ActorStateUI.Instance.Show(actor);
    }

    internal void ToChangeColor(Color color)
    {
        m_Renderer.material.color = color;
    }
    public void ToChangeBlueColor()
    {
        m_Renderer.material.color = moveableColor;
    }
    void ToChangeOriginColor()
    {
        m_Renderer.material.color = m_OriginalColor;
    }

    private void OnMouseExit()
    {
        if (actor)
            ActorStateUI.Instance.Close();
    }
}