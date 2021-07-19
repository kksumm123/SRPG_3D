using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] Vector2Int playerPos; // 플레이어 위치
    [SerializeField] Vector2Int goalPos;   // 클릭한 위치 (이동목표)
    [SerializeField] Dictionary<Vector2Int, int> map = new Dictionary<Vector2Int, int>(); // 블록 맵 지정하기
    [SerializeField] List<int> passableValues; // TilType을 int로 받기
    //enum TileType
    //{
    //    Walkable, // 갈 수 있는 지역
    //    Wall, // 갈 수 없는 지역
    //}

    public Transform player;
    public Transform goal;
    [ContextMenu("길찾기 테스트")]
    void Start()
    {
        StartCoroutine(FindPathCo());
    }

    IEnumerator FindPathCo()
    { 
        passableValues = new List<int>();
        passableValues.Add((int)BlockType.Walkable);

        // 자식의 모든 BlockInfo 찾자
        var blockInfos = GetComponentsInChildren<BlockInfo>();

        // 맵을 채워넣자
        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;
            Vector2Int intPos = new Vector2Int((int)pos.x, (int)pos.z);
            map[intPos] = (int)item.blockType;
        }
        playerPos.x = (int)player.position.x;
        playerPos.y = (int)player.position.z;

        goalPos.x = (int)goal.position.x; 
        goalPos.y = (int)goal.position.z;

        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                player.position = playerNewPos;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
