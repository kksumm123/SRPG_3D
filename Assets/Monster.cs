using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatusType
{
    Normal,
    Sleep,
    Die,
}
public class Actor : MonoBehaviour
{
    public string nickName;
    public string iconName;
    public float hp = 20;
    public float maxHP = 20;
    public float mp = 0;
    public float maxMP = 0;
    public StatusType status;

    public int moveDistance = 5;

    // 공격 범위를 모아두자
    public List<Vector2Int> attackablePoints = new List<Vector2Int>();
    private void Awake()
    {
        var attackPoints = GetComponents<AttackPoint>();

        foreach (var item in attackPoints)
            attackablePoints.Add(item.transform.localPosition.ToVector2Int());

        // 
    }
}
public class Monster : Actor
{
    Animator animator;

    void Start()
    {
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
    }
}