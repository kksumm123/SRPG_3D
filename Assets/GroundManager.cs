using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : SingletonMonoBehavior<GroundManager>
{
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

    public void AddBlockInfo(Vector3 position, BlockType addBlockType, Actor actor)
    {
        Vector2Int pos =
            new Vector2Int(Mathf.RoundToInt(position.x)
                         , Mathf.RoundToInt(position.z));
        if (map.ContainsKey(pos) == false)
            Debug.Log($"{pos} 위치에 맵이 없다.");

        //map[pos] = map[pos] | addBlockType;
        map[pos] |= addBlockType;
        blockInfoMap[pos].blockType |= addBlockType;
        blockInfoMap[pos].actor = actor;
        if (useDebugMode)
            blockInfoMap[pos].UpdateDebugInfo();
    }
    public void RemoveBlockInfo(Vector3 position, BlockType removeBlockType)
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
