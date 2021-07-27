using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Monster : Actor
{
    public static List<Monster> Monsters = new List<Monster>();
    new protected void Awake()
    {
        base.Awake();
        Monsters.Add(this);
    }
    protected void OnDestroy()
    {
        Monsters.Remove(this);
    }
    Animator animator;
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Monster; }

    void Start()
    {
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
        animator = GetComponentInChildren<Animator>();
    }

    public override void TakeHit(int power)
    {
        //맞은 데미지를 표시하자
        GameObject damageTextGo = (GameObject)Instantiate(Resources.Load("DamageText"), transform);
        // 데미지 오브젝트를 적당한 위치로 수정
        //damageTextGo.transform.position = new Vector3(0, 2, 0);
        damageTextGo.transform.localPosition = new Vector3(0, 2, 0);
        damageTextGo.GetComponent<TextMeshPro>().text = power.ToString();
        Destroy(damageTextGo, 2);

        hp -= power;
        animator.Play("TakeHit");
    }

    internal IEnumerator AutoAttackCo()
    {
        // 가장 가까이에 있는 player 탐색

        //Player쪽 이동

        //공격할 수 있으면 공격
        yield return null;
    }
}