using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] Vector2Int playerPos; // 플레이어 위치
    [SerializeField] Vector2Int goalPos;   // 클릭한 위치 (이동목표)
    [SerializeField] Dictionary<Vector2Int, int> map; // 블록 맵 지정하기
    [SerializeField] List<int> passableValues; // TilType을 int로 받기
    //enum TileType
    //{
    //    Walkable, // 갈 수 있는 지역
    //    Wall, // 갈 수 없는 지역
    //}
    void Start()
    {
        passableValues = new List<int>();
        passableValues.Add((int)BlockType.Walkable);

        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
    }

}
