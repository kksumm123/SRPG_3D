using DG.Tweening;
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

    public bool completeMove;
    public bool completeAct;
    public bool CompleteTurn { get => completeMove && completeAct; }

    // 공격 범위를 모아두자
    public List<Vector2Int> attackableLocalPoints = new List<Vector2Int>();
    protected void Awake()
    {
        var attackPoints = GetComponentsInChildren<AttackPoint>(true);

        // 앞쪽 공격 포인트
        foreach (var item in attackPoints)
            attackableLocalPoints.Add((item.transform.position -transform.position).ToVector2Int());

        // 오른쪽 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 뒤 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 왼쪽 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 다시 앞을 보도록 
        transform.Rotate(0, 90, 0);

        animator = GetComponentInChildren<Animator>();
    }

    public void TakeHit(int power)
    {
        //맞은 데미지를 표시하자
        GameObject damageTextGo = (GameObject)Instantiate(Resources.Load("DamageText"), transform.position, Quaternion.identity, transform);
        // 데미지 오브젝트를 적당한 위치로 수정
        //damageTextGo.transform.position = new Vector3(0, 2, 0);
        damageTextGo.transform.localPosition = new Vector3(0, 2, 0);
        damageTextGo.GetComponent<TextMeshPro>().text = power.ToString();
        Destroy(damageTextGo, 2);

        //맞은 데미지 표시
        hp -= power;
        animator.Play("TakeHit");
    }
    public virtual BlockType GetBlockType()
    {
        Debug.LogError("자식에서 GetBlockType() 오버라이드 해야함");
        return BlockType.None;
    }

    protected bool IsInAttackArea(Vector3 enemyPosition)
    { // 타겟 위치가 공격 가능한 지역인지 확인
        Vector2Int enemyPositionVector2 = enemyPosition.ToVector2Int();
        Vector2Int currentPos = transform.position.ToVector2Int();

        // 공격 가능한 지역에 적이 있는지
        foreach (var item in attackableLocalPoints)
        {
            Vector2Int pos = item + currentPos; //아이템의 월드 지역 위치
            if (pos == enemyPositionVector2)
                return true;
        }
        return false;
    }
    [SerializeField] protected BlockType passableValues = BlockType.Walkable | BlockType.Water;
    [SerializeField] float moveDelay = 0.3f;
    protected IEnumerator FindPathCo(Vector2Int destPos)
    {
        Vector2Int myPos = transform.position.ToVector2Int();
        Vector3 myPosVec3 = transform.position;
        var map = GroundManager.Instance.blockInfoMap;
        var path = PathFinding2D.find4(myPos, destPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else
        {
            // 원래 위치에서 플레이어 정보 삭제
            GroundManager.Instance.RemoveBlockInfo(myPosVec3, GetBlockType());
            PlayAnimation("Walk");
            FollowTarget.Instance.SetTarget(transform);
            path.RemoveAt(0);

            //몬스터 일때는 마지막 지점을 삭제해야한다
            if (ActorType == ActorTypeEnum.Monster)
                path.RemoveAt(path.Count - 1);


            // 최대 이동거리만큼 이동하자.
            if (path.Count > moveDistance) //3
                path.RemoveRange(moveDistance, path.Count - moveDistance);

            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, myPosVec3.y, item.y);
                StartCoroutine(LookAtLerp(playerNewPos));
                transform.DOMove(playerNewPos, moveDelay).SetEase(Ease.Linear);
                yield return new WaitForSeconds(moveDelay);
            }
        }
        PlayAnimation("Idle");
        FollowTarget.Instance.SetTarget(null);
        // 이동한 위치에 플레이어 정보 추가 
        GroundManager.Instance.AddBlockInfo(transform.position, GetBlockType(), this);

        completeMove = true;
        OnCompleteMove();
    }

    protected virtual void OnCompleteMove()
    {
    }
    protected Animator animator;
    public void PlayAnimation(string stateName)
    {
        animator.Play(stateName);
    }
    [SerializeField] float rotatelerpValue = 0.05f;
    IEnumerator LookAtLerp(Vector3 targetPos)
    {
        var endTime = Time.time + moveDelay;
        while (endTime > Time.time)
        {
            transform.forward = Vector3.Slerp(transform.forward
                    , (targetPos - transform.position).normalized, rotatelerpValue);
            yield return null;
        }
    }

    protected float attackTime = 1;
    protected IEnumerator AttackToTargetCo(Actor attackTarget)
    {
        // 타겟 방향 보기
        transform.LookAt(attackTarget.transform);

        animator.Play("Attack");
        attackTarget.TakeHit(power);
        yield return new WaitForSeconds(attackTime);

        completeAct = true;
    }
}