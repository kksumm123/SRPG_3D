using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Actor
{
    public static List<Player> Players = new List<Player>();
    public int ID;
    public SaveInt exp, level;
    public SaveString comment;
    new void Awake()
    {
        base.Awake();
        Players.Add(this);
    }

    void InitLevelData()
    {
        exp = new SaveInt("exp" + ID, 0);
        level = new SaveInt("level" + ID, 1);
        comment = new SaveString("comment" + ID);
        var data = GlobalData.Instance.playerDataMap[level.Value];
        maxExp = data.maxExp;
        hp = data.hp;
        mp = data.mp;
    }

    [ContextMenu("저장 테스트")]
    void TestSave()
    {
        exp.Value += 1;
        comment.Value += 'a';
    }
    new private void OnDestroy()
    {
        base.OnDestroy();
        Players.Remove(this);
    }
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Player; }
    public static Player SelectedPlayer;

    void Start()
    {
        //SelectedPlayer = this;
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
        FollowTarget.Instance.SetTarget(transform);
        InitLevelData();

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

    public void AttackToTarget(Monster monster)
    {
        ClearEnemyExistPoint();
        StartCoroutine(AttackToTartgetCo_(monster));
    }

    private IEnumerator AttackToTartgetCo_(Monster monster)
    {
        yield return AttackToTargetCo(monster);
        if (monster.status == StatusType.Die)
        {
            AddExp(monster.rewardExp);
        }
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    //int exp, level;
    int maxExp;
    void AddExp(int rewardExp)
    {
        // 경험치 추가
        exp.Value += rewardExp;

        // 경험치가 최대 경험치보다 클 경우 레벨 증가

        // 래밸 중가할 경우 hp, mp 회복, hp, mp 증가
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
    protected override void OnDie()
    {
        // 플레이어가 죽은 경우
        // 모든 플레이어가 죽었는지 파악, 다 죽었으면 GameOver
        if (Players.Where(x => x.status != StatusType.Die).Count() == 0)
        {
            CenterNotifyUI.Instance.Show("게임 오버");
        }
    }
}