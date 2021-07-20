using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player selectedPlayer;

    Animator animator;
    void Start()
    {
        selectedPlayer = this;
        animator = GetComponentInChildren<Animator>();
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player);
    }
    public void PlayAnimation(string stateName)
    {
        animator.Play(stateName);
    }
}
