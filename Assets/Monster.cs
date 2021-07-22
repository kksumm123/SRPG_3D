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
}
public class Monster : Actor
{
    Animator animator;

    void Start()
    {
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
    }
}