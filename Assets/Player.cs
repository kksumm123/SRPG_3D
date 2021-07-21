using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public static Player SelectedPlayer;
    Animator animator;
    Transform player;
    [SerializeField] float rotatelerpValue = 0.05f;
    [SerializeField] float moveDelay = 0.3f;
    void Start()
    {
        SelectedPlayer = this;
        animator = GetComponentInChildren<Animator>();
        player = transform;
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
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
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.z));
        playerPos.x = Mathf.RoundToInt(player.position.x);
        playerPos.y = Mathf.RoundToInt(player.position.z);
        var map = GroundManager.Instance.map;
        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            // 원래 위치에서 플레이어 정보 삭제
            GroundManager.Instance.RemoveBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player);
            Player.SelectedPlayer.PlayAnimation("Run");
            FollowTarget.Instance.SetTarget(Player.SelectedPlayer.transform);
            path.RemoveAt(0);
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                StartCoroutine(PlayerLookAtLerp(playerNewPos));
                player.DOMove(playerNewPos, moveDelay).SetEase(Ease.Linear);
                yield return new WaitForSeconds(moveDelay);
            }
        }
        Player.SelectedPlayer.PlayAnimation("Idle");
        FollowTarget.Instance.SetTarget(null);
        // 이동한 위치에 플레이어 정보 추가 
        GroundManager.Instance.AddBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player, this);
    }

    IEnumerator PlayerLookAtLerp(Vector3 playerNewPos)
    {
        var endTime = Time.time + moveDelay;
        while (endTime > Time.time)
        {
            player.forward = Vector3.Slerp(player.forward
                    , (playerNewPos - player.position).normalized, rotatelerpValue);
            yield return null;
        }
    }

    public void OnTouch(Vector3 position)
    {
        Vector2Int findPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        FindPath(findPos);
    }
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
}