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
    Animator animator;
    [SerializeField] float rotatelerpValue = 0.05f;
    [SerializeField] float moveDelay = 0.3f;
    void Start()
    {
        //SelectedPlayer = this;
        animator = GetComponentInChildren<Animator>();
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
        FollowTarget.Instance.SetTarget(transform);
    }
    public void PlayAnimation(string stateName)
    {
        animator.Play(stateName);
    }

    Coroutine findPathCoHandle;
    void FindPath(Vector2Int goalPos)
    {
        StopCo(findPathCoHandle);
        findPathCoHandle = StartCoroutine(FindPathCo(goalPos));
    }
    [SerializeField] BlockType passableValues = BlockType.Walkable | BlockType.Water;
    IEnumerator FindPathCo(Vector2Int goalPos)
    {
        Vector2Int playerPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;
        var path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            // 원래 위치에서 플레이어 정보 삭제
            GroundManager.Instance
                .RemoveBlockInfo(transform.position, BlockType.Player);
            PlayAnimation("Run");
            FollowTarget.Instance.SetTarget(transform);
            path.RemoveAt(0);
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                StartCoroutine(PlayerLookAtLerp(playerNewPos));
                transform.DOMove(playerNewPos, moveDelay).SetEase(Ease.Linear);
                yield return new WaitForSeconds(moveDelay);
            }
            completeMove = true;
        }
        Player.SelectedPlayer.PlayAnimation("Idle");
        FollowTarget.Instance.SetTarget(null);
        // 이동한 위치에 플레이어 정보 추가 
        GroundManager.Instance
            .AddBlockInfo(transform.position, BlockType.Player, this);

        bool existAttackTarget = ShowAttackableArea();
        if (existAttackTarget)
            StageManager.GameState = GameStateType.SelectToAttackTarget;
        else
            StageManager.GameState = GameStateType.SelectPlayer;
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

    bool IsInAttackArea(Vector3 enemyPosition)
    { // 타겟 위치가 공격 가능한 지역인지 확인
        Vector2Int enemyPositionVector2 = enemyPosition.ToVector2Int();
        Vector2Int currentPos = transform.position.ToVector2Int();

        // 공격 가능한 지역에 적이 있는지
        foreach (var item in attackableLocalPoints)
        {
            Vector2Int pos = item + currentPos; //아이템의 월드 지역 위치
            if (pos == enemyPositionVector2)
                return true;
        }
        return false;
    }

    public void AttackToTarget(Actor actor)
    {
        StartCoroutine(AttackToTargetCo(actor));
        ClearEnemyExistPoint();
    }

    public float attackTime = 1;
    private IEnumerator AttackToTargetCo(Actor attackTarget)
    {
        // 타겟 방향 보기
        transform.LookAt(attackTarget.transform);

        animator.Play("Attack");
        attackTarget.TakeHit(power);
        yield return new WaitForSeconds(attackTime);

        completeAct = true;
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    IEnumerator PlayerLookAtLerp(Vector3 targetPos)
    {
        var endTime = Time.time + moveDelay;
        while (endTime > Time.time)
        {
            transform.forward = Vector3.Slerp(transform.forward
                    , (targetPos - transform.position).normalized, rotatelerpValue);
            yield return null;
        }
    }
    internal bool OnMoveable(Vector3 position, int maxDistance)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;
        var path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0 || path.Count > maxDistance + 1)
            return false;

        return true;
    }

    public void ClearEnemyExistPoint()
    {
        enemyExistPoint.ForEach(x => x.ToChangeOriginColor());
        enemyExistPoint.Clear();
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
}