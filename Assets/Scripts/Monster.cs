using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum StatusType
{
    Normal,
    Sleep,
    Die,
}
public enum ActorTypeEnum
{
    NotInit,
    Player,
    Monster,
}
public class Actor : MonoBehaviour
{
    //오버라이드 해서 상속받은 애들이 각각 리턴값 정하도록
    public virtual ActorTypeEnum ActorType { get => ActorTypeEnum.NotInit; }
    public string nickName;
    public string iconName;
    public int power;
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
        var attackPoints = GetComponentsInChildren<AttackPoint>(true);

        // 앞쪽 공격 포인트
        foreach (var item in attackPoints)
            attackablePoints.Add(item.transform.position.ToVector2Int());

        // 오른쪽 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 뒤 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 왼쪽 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 다시 앞을 보도록 
        transform.Rotate(0, 90, 0);
    }

    public virtual void TakeHit(int power)
    {
        //맞은 데미지 표시
        hp -= power;
    }
}
public class Monster : Actor
{
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
}