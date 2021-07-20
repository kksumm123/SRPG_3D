﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GroundManager : SingletonMonoBehavior<GroundManager>
{
    [SerializeField] float rotatelerpValue = 0.05f;
    Transform player;
    [SerializeField] float moveDelay = 0.3f;
    [SerializeField] Vector2Int playerPos; // 플레이어 위치
    [SerializeField] Vector2Int goalPos;   // 클릭한 위치 (이동목표)
    public Dictionary<Vector2Int, BlockType> map
        = new Dictionary<Vector2Int, BlockType>(); // 블록 맵 지정하기, A*에서 사용
    public Dictionary<Vector2Int, BlockInfo> blockInfoMap
        = new Dictionary<Vector2Int, BlockInfo>();
    [SerializeField] BlockType passableValues = BlockType.Walkable | BlockType.Water; // 비트연산, int1 | int2 = 3 => 01 | 10 = 11

    [SerializeField] bool useDebugMode = true;
    List<GameObject> debugTexts = new List<GameObject>();

    new void Awake()
    {
        base.Awake();
        // 자식의 모든 BlockInfo 찾자
        var blockInfos = GetComponentsInChildren<BlockInfo>();

        debugTexts.ForEach(x => Destroy(x));
        debugTexts.Clear();

        // 맵을 채워넣자
        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;
            Vector2Int intPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
            map[intPos] = item.blockType;

            if (useDebugMode)
                item.UpdateDebugInfo();
            blockInfoMap[intPos] = item;
        }
    }

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

        playerPos.x = Mathf.RoundToInt(player.position.x);
        playerPos.y = Mathf.RoundToInt(player.position.z);

        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            // 원래 위치에서 플레이어 정보 삭제
            RemoveBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player);
            Player.SelectedPlayer.PlayAnimation("Run");
            FollowTarget.Instance.SetTarget(Player.SelectedPlayer.transform);
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
        AddBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player);
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

    public void AddBlockInfo(Vector3 position, BlockType addBlockType)
    {
        Vector2Int pos =
            new Vector2Int(Mathf.RoundToInt(position.x)
                         , Mathf.RoundToInt(position.z));
        if (map.ContainsKey(pos) == false)
            Debug.Log($"{pos} 위치에 맵이 없다.");

        //map[pos] = map[pos] | addBlockType;
        map[pos] |= addBlockType;
        blockInfoMap[pos].blockType |= addBlockType;
        if (useDebugMode)
            blockInfoMap[pos].UpdateDebugInfo();
    }
    private void RemoveBlockInfo(Vector3 position, BlockType removeBlockType)
    {
        Vector2Int pos =
            new Vector2Int(Mathf.RoundToInt(position.x)
                         , Mathf.RoundToInt(position.z));
        if (map.ContainsKey(pos) == false)
            Debug.Log($"{pos} 위치에 맵이 없다.");

        //map[pos] = map[pos] | addBlockType;
        map[pos] &= ~removeBlockType;
        blockInfoMap[pos].blockType &= ~removeBlockType;
        if (useDebugMode)
            blockInfoMap[pos].UpdateDebugInfo();
    }
}
