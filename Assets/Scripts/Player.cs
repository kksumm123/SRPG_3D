using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public static List<Player> Players = new List<Player>();
    new void Awake()
    {
        base.Awake();
        Players.Add(this);
    }
    private void OnDestroy()
    {
        Players.Remove(this);
    }
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Player; }
    public static Player SelectedPlayer;

    void Start()
    {
        //SelectedPlayer = this;
        animator = GetComponentInChildren<Animator>();
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
        FollowTarget.Instance.SetTarget(transform);
    }

    Coroutine findPathCoHandle;
    void FindPath(Vector2Int goalPos)
    {
        StopCo(findPathCoHandle);
        findPathCoHandle = StartCoroutine(FindPathCo(goalPos));
    }

    public bool CanAttackTarget(Actor enemy)
    {
        // 같은 팀이면 공격 X
        if (enemy.ActorType != ActorTypeEnum.Monster)
            return false;

        // 공격 가능한 범위안에 있는지 확인
        if (IsInAttackArea(enemy.transform.position) == false)
            return false;

        return true;
    }

    public void AttackToTarget(Actor actor)
    {
        ClearEnemyExistPoint();
        StartCoroutine(AttackToTartgetCo_(actor));
    }

    private IEnumerator AttackToTartgetCo_(Actor actor)
    {
        yield return AttackToTargetCo(actor);
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    internal bool OnMoveable(Vector3 position, int maxDistance)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;
        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0 || path.Count > maxDistance + 1)
            return false;

        return true;
    }

    public void ClearEnemyExistPoint()
    {
        enemyExistPoint.ForEach(x => x.ToChangeOriginColor());
        enemyExistPoint.Clear();
    }
    protected override void OnCompleteMove()
    {
        bool existAttackTarget = ShowAttackableArea();
        if (existAttackTarget)
            StageManager.GameState = GameStateType.SelectToAttackTarget;
        else
            StageManager.GameState = GameStateType.SelectPlayer;
    }

    public List<BlockInfo> enemyExistPoint = new List<BlockInfo>();
    public bool ShowAttackableArea()
    {
        Vector2Int currentPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;

        foreach (var item in attackableLocalPoints)
        {
            Vector2Int pos = item + currentPos; //아이템의 월드 지역 위치
            // item 블록 위에 적이 있는지
            if (map.ContainsKey(pos))
            {
                if (IsEnemyExist(map[pos]))
                {
                    enemyExistPoint.Add(map[pos]);
                }
            }
        }

        enemyExistPoint.ForEach(x => x.ToChangeColor(Color.red));
        return enemyExistPoint.Count > 0;
    }

    private bool IsEnemyExist(BlockInfo blockInfo)
    {
        if (blockInfo.blockType.HasFlag(BlockType.Monster) == false)
            return false;
        Debug.Assert(blockInfo.actor != null, "블록의 액터는 있어야 해");

        return true;
    }

    public void MoveToPosition(Vector3 position)
    {
        Vector2Int findPos = position.ToVector2Int();
        FindPath(findPos);
    }
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    public override BlockType GetBlockType()
    {
        return BlockType.Player;
    }
}