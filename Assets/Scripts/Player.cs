using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Actor
{
    public static List<Player> Players = new List<Player>();
    public int ID;
    public int exp
    {
        get { return data.exp; }
        set { data.exp = value; }
    }
    public int level
    {
        get { return data.level; }
        set { data.level = value; }
    }
    public int maxExp;
    new protected void Awake()
    {
        base.Awake();
        Players.Add(this);
        InitLevelData();
    }

    void InitLevelData()
    {
        SetLevelData();
    }

    void SetLevelData()
    {
        if (GlobalData.Instance.playerDataMap.ContainsKey(level) == false)
            Debug.LogError("레벨 정보 없다");
        var data = GlobalData.Instance.playerDataMap[level];
        maxExp = data.maxExp;
        hp = maxHp = data.maxHp;
        mp = maxMp = data.maxMp;
    }

    new protected void OnDestroy()
    {
        base.OnDestroy();
        Players.Remove(this);
    }
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Player; }
    public static Player SelectedPlayer;

    protected void Start()
    {
        //SelectedPlayer = this;
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

        if (completeAct)
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
            if (monster.dropGroup.ratio > Random.Range(0, 1f))
                DropItem(monster.dropGroup.dropItemID, monster.transform.position);
        }
        StageManager.GameState = GameStateType.SelectPlayer;
    }
    [ContextMenu("아이템드랍")]
    void DropItemTest()
    {
        DropItem(1, transform.position);
    }
    void DropItem(int dropGroupID, Vector3 position)
    {
        var dropGroup = GlobalData.Instance.dropItemGroupDataMap[dropGroupID];

        var dropItemRatioInfo = dropGroup.dropItems.OrderByDescending(x => x.ratio * Random.Range(0, 1f)).First();
        Debug.Log(dropItemRatioInfo.ToString());

        var dropItem = GlobalData.Instance.itemDataMap[dropItemRatioInfo.dropItemID];
        Debug.Log(dropItem.ToString());

        GroundManager.Instance.AddBlockInfo(position, BlockType.Item, dropItem);
    }

    //int exp, level;
    void AddExp(int rewardExp)
    {
        // 경험치 추가
        exp += rewardExp;

        // 경험치가 최대 경험치보다 클 경우 레벨 증가
        if (exp >= maxExp)
        { // 래밸 중가할 경우 현재 hp, mp 회복, 최대 hp, mp 증가
            exp -= maxExp;
            level++;
            SetLevelData();
            CenterNotifyUI.Instance.Show($"레벨업 ! lv.{level}");
        }
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

        //도착한 지역에 아이템이 있다면 획득하자
        var intPos = transform.position.ToVector2Int();
        int itemID = GroundManager.Instance.blockInfoMap[intPos].dropItemID;
        if (itemID > 0)
        {
            // 아이템 획득하기
            AddItem(itemID);

            // 땅에 존재하는 아이템 삭제하기
            GroundManager.Instance.RemoveItem(transform.position);
        }
    }
    [System.Serializable]
    public class PlayerData
    {
        //내가 가진 아이템들
        public List<int> haveItem = new List<int>();
        public int exp;
        public int level;
    }
    public PlayerData data;
    void AddItem(int itemID)
    {
        data.haveItem.Add(itemID);
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