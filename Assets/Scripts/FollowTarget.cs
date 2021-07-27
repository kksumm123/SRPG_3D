using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : SingletonMonoBehavior<FollowTarget>
{
    Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 0, -5);

    public void SetTarget(Transform _target)
    {
        this.target = _target;
    }

    void Update()
    {
        if (target == null)
            return;

        var newPow = target.position + offset;
        newPow.x = transform.position.x;
        newPow.y = transform.position.y;
        transform.position = newPow;
    }
}