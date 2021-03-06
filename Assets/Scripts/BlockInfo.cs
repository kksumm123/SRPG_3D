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
    Item = 1 << 4,
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
            case GameStateType.SelectedPlayerMoveOrAct:
                SelectedPlayerMoveOrAct();
                break;
            case GameStateType.SelectToAttackTarget:
                SelectToAttackTarget();
                break;
            //case GameStateType.AttackTartget:
            //    AttackTartget();
            //    break;

            case GameStateType.NotInit:
            case GameStateType.IngPlayerMove:
            case GameStateType.MonsterTurn:
                Debug.Log($"블럭을 클릭할 수 없는 상태 : {StageManager.GameState}");
                break;
        }

        //if (actor && actor == Player.SelectedPlayer)
        //{ // 클릭한 위치에 actor가 있고, 플레이어가 있다면
        //  //영역표시
        //  // 첫번째 이동으로 갈 수 있는 곳을 추가
        //    ShowMoveDistance(actor.moveDistance);
        //}
        //if (transform.position != Player.SelectedPlayer.transform.position)
        //    Player.SelectedPlayer.MoveToPosition(transform.position);

    }
    void SelectPlayer()
    {
        if (actor == null)
            return;
        if (actor.GetType() == typeof(Player))
        {
            //Player.SelectedPlayer = actor as Player;

            Player player = (Player)actor;
            if (player.CompleteTurn)
            {
                CenterNotifyUI.Instance.Show("모든 행동이 끝난 플레이어 입니다.");
                return;
            }

            Player.SelectedPlayer = player;

            // 이동가능한 영역 표시
            if (player.completeMove == false)
                ShowMoveableBlocks(Player.SelectedPlayer.moveDistance);
            else
                CenterNotifyUI.Instance.Show("이미 이동했습니다.");

            // 현재 위치에서 공격 가능한 영역 표시
            if (player.completeAct == false)
                Player.SelectedPlayer.ShowAttackableArea();
            else
                CenterNotifyUI.Instance.Show("이미 공격했습니다.");

            StageManager.GameState = GameStateType.SelectedPlayerMoveOrAct;
        }
    }
    private void SelectedPlayerMoveOrAct()
    {// 공격 대상이 있다면 공격 하자 (Actor == 몬스터 라면
        if (actor)
        {
            if (Player.SelectedPlayer.CanAttackTarget(actor))
            {
                ClearMoveableArea();
                Player.SelectedPlayer.AttackToTarget((Monster)actor);
            }
            else
            {
                NotifyUI.Instance.Show("여기는 공격 모태");
            }
        }
        else
        {
            // 플레이어 이동 블럭 클릭
            if (highlightedMoveableArea.Contains(this))
            {
                Player.SelectedPlayer.ClearEnemyExistPoint();
                Player.SelectedPlayer.MoveToPosition(transform.position);
                ClearMoveableArea();
                StageManager.GameState = GameStateType.IngPlayerMove;
            }
        }
    }

    // 이동후에 공격할 타겟일 수 있는 블럭 선택
    // 블럭에 공격할 타겟이 있고 공격 가능한 타겟이면 공격
    private void SelectToAttackTarget()
    {
        if (Player.SelectedPlayer.enemyExistPoint.Contains(this))
        {
            if (Player.SelectedPlayer.CanAttackTarget(actor))
            {
                Player.SelectedPlayer.AttackToTarget((Monster)actor);
            }
        }
    }
    void ShowMoveableBlocks(int moveDistance)
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
    public static void ClearMoveableArea()
    {
        highlightedMoveableArea.ForEach(x => x.ToChangeOriginColor());
        highlightedMoveableArea.Clear();
    }
    string debugTextPrefabString = "DebugTextPrefab";
    GameObject debutTextGos;
    internal Actor actor;
    internal int dropItemID;
    internal GameObject dropItemGo;

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
    public void ToChangeOriginColor()
    {
        m_Renderer.material.color = m_OriginalColor;
    }

    private void OnMouseExit()
    {
        if (actor)
            ActorStateUI.Instance.Close();
    }
}