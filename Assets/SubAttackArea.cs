using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAttackArea : MonoBehaviour
{
    public float damageRatio = 1;
    public Target target = Target.EnemyOnly;
    public enum Target
    {
        EnemyOnly, // 적군 only
        AllyOnly, // 아군 only
        All, // 모두
    }
}
