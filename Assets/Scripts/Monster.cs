using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Player enemyPlayer = GetNearestPlayer();

        // 공격 가능한 위치에 있다면 바로 공격 
        if (IsAttackablePosition(enemyPlayer.transform.position))
        {
            // 공격
            yield return AttackToTargetCo(enemyPlayer);
        }
        else
        {
            //Player쪽 이동
            yield return MoveToPositionCo(enemyPlayer.transform.position);

            //공격할 수 있으면 공격
            yield return AttackToTargetCo(enemyPlayer);
        }
        yield return null;
    }

    private Player GetNearestPlayer()
    {
        var myPos = transform.position;
        //플레이어 리스트를 거리순으로 정렬 후 맨 앞에 있는 정보 가져옴
        var nearestPlayer = Player.Players
            .Where(x => x.status != StatusType.Die)
            .OrderBy(x => Vector3.Distance(x.transform.position, myPos))
            .Single();
        return nearestPlayer;
    }
}