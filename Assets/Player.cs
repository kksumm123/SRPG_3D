using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
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

    public bool CanAttackTarget(Actor actor)
    {
        // 같은 팀이면 공격 X
        if (actor.)
            return false;

        return true;
    }


    IEnumerator PlayerLookAtLerp(Vector3 playerNewPos)
    {
        var endTime = Time.time + moveDelay;
        while (endTime > Time.time)
        {
            transform.forward = Vector3.Slerp(transform.forward
                    , (playerNewPos - transform.position).normalized, rotatelerpValue);
            yield return null;
        }
    }
    internal bool OnMoveable(Vector3 position, int maxDistance)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;
        var path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else if (path.Count > maxDistance + 1)
            Debug.Log("이동모태 !");
        else
            return true;

        return false;
    }
    public bool ShowAttackableArea()
    {
        bool existEnemy = false;

        Vector2Int currentPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.blockInfoMap;

        foreach (var item in attackablePoints)
        {
            Vector2Int pos = item + currentPos; //아이템의 월드 지역 위치
            // item 블록 위에 적이 있는지
            if (map.ContainsKey(pos))
            {
                if (IsEnemyExist(map[pos]))
                {
                    map[pos].ToChangeColor(Color.red);
                    existEnemy = true;
                }
            }
        }

        return existEnemy;
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