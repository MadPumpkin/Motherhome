using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float MoveSpeed = 6f;
    public float FollowDistance = 1f;
    public Transform Target;
    public bool MatchRotation = false;

    float followDistanceSquared;

    void Start()
    {
        followDistanceSquared = FollowDistance * FollowDistance;
    }

    void Update()
    {
        if (Target == null)
            return;

        if (MatchRotation)
            transform.rotation = Target.rotation;

        var difference = transform.position - Target.position;
        if (difference.sqrMagnitude > followDistanceSquared)
        {
            var targetPosition = Target.position + (difference.normalized * FollowDistance);
            if ((transform.position - targetPosition).sqrMagnitude > 0.1f)
                transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
        }
    }
}