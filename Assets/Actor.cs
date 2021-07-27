using System.Collections;
using System.Collections.Generic;
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
            attackableLocalPoints.Add(item.transform.position.ToVector2Int());

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
    }

    public virtual void TakeHit(int power)
    {
        //맞은 데미지 표시
        hp -= power;
    }
    public BlockType GetBlockType()
    {
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
}