using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapMover : MonoBehaviour
{
    void Start()
    {
        if (Application.isPlaying == true)
            Destroy(this); // 컴포넌트만 부시기
    }
    void Update()
    {
        transform.position = transform.position.ToVector3Snap();
    }
}
