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
    public float hp;
    public float mp;
    public StatusType status;
}
public class Monster : Actor
{
    Animator animator;

    void Start()
    {
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
    }
}