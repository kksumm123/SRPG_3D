using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] Vector2Int playerPos; // �÷��̾� ��ġ
    [SerializeField] Vector2Int goalPos;   // Ŭ���� ��ġ (�̵���ǥ)
    [SerializeField] Dictionary<Vector2Int, int> map; // ��� �� �����ϱ�
    [SerializeField] List<int> passableValues; // TilType�� int�� �ޱ�
    //enum TileType
    //{
    //    Walkable, // �� �� �ִ� ����
    //    Wall, // �� �� ���� ����
    //}
    void Start()
    {
        passableValues = new List<int>();
        passableValues.Add((int)BlockType.Walkable);

        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
    }

}
