using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : SingletonMonoBehavior<GroundManager>
{
    [SerializeField] float rotatelerpValue = 0.05f;
    Transform player;
    [SerializeField] float moveDelay = 0.3f;
    [SerializeField] Vector2Int playerPos; // 플레이어 위치
    [SerializeField] Vector2Int goalPos;   // 클릭한 위치 (이동목표)
    [SerializeField]
    Dictionary<Vector2Int, BlockType> map
        = new Dictionary<Vector2Int, BlockType>(); // 블록 맵 지정하기
    [SerializeField] BlockType passableValues = BlockType.Walkable | BlockType.Water; // TilType을 int로 받기

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    Coroutine findPathCoHandle;
    void FindPath(Vector2Int goalPos)
    {
        StopCo(findPathCoHandle);
        findPathCoHandle = StartCoroutine(FindPathCo(goalPos));
    }
    IEnumerator FindPathCo(Vector2Int goalPos)
    {
        //passableValues = new List<int>();
        //passableValues.Add((int)BlockType.Walkable);

        // 자식의 모든 BlockInfo 찾자
        var blockInfos = GetComponentsInChildren<BlockInfo>();

        // 맵을 채워넣자
        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;
            Vector2Int intPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
            map[intPos] = item.blockType;
        }
        playerPos.x = Mathf.RoundToInt(player.position.x);
        playerPos.y = Mathf.RoundToInt(player.position.z);

        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            Player.selectedPlayer.PlayAnimation("Run");
            FollowTarget.Instance.SetTarget(Player.selectedPlayer.transform);
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                StartCoroutine(PlayerLookAtLerp(playerNewPos));
                player.DOMove(playerNewPos, moveDelay).SetEase(Ease.Linear);
                yield return new WaitForSeconds(moveDelay);
            }
            Player.selectedPlayer.PlayAnimation("Idle");
        }
        FollowTarget.Instance.SetTarget(null);
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
