using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : SingletonMonoBehavior<FollowTarget>
{
    Transform target;
    [SerializeField] Vector3 offset;

    public void SetTarget(Transform _target)
    {
        this.target = _target;
    }

    void Update()
    {
        if (target == null)
            return;

        var newPow = target.position + offset;
        newPow.y = transform.position.y;
        transform.position = newPow;
    }
}