using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legend;
using System;

namespace Legend
{
    public enum EnemyMovementType
    {
        Stationary,
        HorizontalPacing,
        VerticalPacing,
        DiagonalBounce,
        Wander,
        ZigZag,
        MoveTowardsTarget,
        MaintainDistance,
        Curved,
        PatrolWaypoints,
        Jumping,
        RandomOrthogonalDirection,
    }

    public class EnemyMovement : MonoBehaviour
    {
        public const float WanderRadius = 3.0f;

        [Header("Movement")]
        public float MoveSpeed = 0.0f;
        public float RestTime = 0.0f;
        public float PacingDistance = 0.0f;
        public EnemyMovementType Type;
        public EnemyMovementType MissingTargetMovement = EnemyMovementType.Wander;
        public EnemyMovementType AlternateMovement;
        public float AlternateInterval = 2f;
        public float AlternatePercentage = 0;
        public List<Transform> positions = new List<Transform>();
        public bool AlwaysFaceCenter;
        public bool IsFlying;

        Enemy enemy;
        EnemyVisual visual;
        Rigidbody2D rigid;

        int currentWaypoint = 0;
        Vector3 targetPosition;
        Vector3 targetDirection;
        float stunnedUntil;
        Vector3 startingPosition;
        bool isResting;
        float restUntil;
        EnemyMovementType currentMovement;
        bool isAlternateMovement;
        float nextAlternateMovementCheck;
        bool isFeared = false;
        float fearTime;

        bool IsAtTargetPosition { get { return (transform.position - targetPosition).magnitude < 0.03f; } }

        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            enemy = GetComponentInParent<Enemy>();
            visual = GetComponentInParent<EnemyVisual>();

            startingPosition = transform.position;
            Update();
            GetNextTargetPosition();

            nextAlternateMovementCheck = Time.time + (AlternatePercentage * UnityEngine.Random.value);

            if (MoveSpeed > 0)
                MoveSpeed += UnityEngine.Random.value * 0.2f;

            if (AlwaysFaceCenter)
                FaceCenter();
        }

        void Update()
        {
            if (AlternatePercentage > 0 && Time.time > nextAlternateMovementCheck)
            {
                isAlternateMovement = UnityEngine.Random.value < AlternatePercentage;
                nextAlternateMovementCheck = Time.time + AlternateInterval;
            }

            //if (enemy.Target != null)
            //    enemy.Room.FindPath(transform.position, enemy.Target.transform.position);
            if(fearTime < Time.time && isFeared)
            {
                isFeared = false;
                currentMovement = Type;
                MoveSpeed = -1 * MoveSpeed;
            }

            if (enemy.Target == null)
            {
                currentMovement = MissingTargetMovement;
            }
            else
            {
                currentMovement = isAlternateMovement ? AlternateMovement : Type;
            }

            if (enemy.Health.IsDead
                || currentMovement == EnemyMovementType.Stationary)
            {
                //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 10);
                visual.IsMoving = false;
                return;
            }

            var movement = Vector3.zero;
            if (enemy.CurrentMove != null && !enemy.CurrentMove.IsMovingAnimation)
            {
                movement = enemy.CurrentMove.Movement;
            }
            else
            {
                if (currentMovement == EnemyMovementType.MoveTowardsTarget || currentMovement == EnemyMovementType.MaintainDistance || IsAtTargetPosition)
                    GetNextTargetPosition();

                if (targetDirection != Vector3.zero)
                    movement = Quaternion.LookRotation(targetDirection) * (Vector3.forward * MoveSpeed);
            }

            if (stunnedUntil < Time.time && movement != Vector3.zero)
            {
                visual.IsMoving = true;
                rigid.MovePosition(transform.position + (movement * Time.deltaTime));

                //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 10);

                if (!IsAtTargetPosition) // to prevent spazing when target position reached
                {
                    targetDirection.z = 0;
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, -targetDirection);
                }
                else
                {
                    visual.IsMoving = true;
                }

                //print(enemyVisual.Facing);
                //if (Controller.collisionState.HasCollision)
                //    GetNextTargetPosition();
            }
        }

        void GetNextTargetPosition()
        {
            //print(name + " GetNextTargetPosition " + currentMovement);
            if (isResting)
            {
                if (Time.time < restUntil)
                    return;
                isResting = false;
            }
            else
            {
                if (RestTime != 0)
                {
                    isResting = true;
                    restUntil = Time.time + RestTime;
                    return;
                }
            }

            switch (currentMovement)
            {
                case EnemyMovementType.PatrolWaypoints:
                    targetPosition = positions[currentWaypoint % positions.Count].position;
                    currentWaypoint++;
                    targetDirection = targetPosition - transform.position;
                    break;
                case EnemyMovementType.HorizontalPacing:
                    if (transform.position == startingPosition)
                    {
                        targetPosition = startingPosition;
                        targetPosition.x += PacingDistance;
                    }
                    else if (transform.position == targetPosition)
                    {
                        targetPosition = startingPosition;
                        targetPosition.x -= PacingDistance;
                    }
                    targetDirection = targetPosition - transform.position;
                    break;
                case EnemyMovementType.VerticalPacing:
                    if (transform.position == startingPosition)
                    {
                        targetPosition = startingPosition;
                        targetPosition.y += PacingDistance;
                    }
                    else if (transform.position == targetPosition)
                    {
                        targetPosition = startingPosition;
                        targetPosition.y -= PacingDistance;
                    }
                    targetDirection = targetPosition - transform.position;
                    break;
                case EnemyMovementType.MoveTowardsTarget:
                    if (enemy.Target != null)
                    {
                        if (enemy.HasClearPath(enemy.Target.transform.position, enemy.Target.transform))
                        {
                            targetPosition = enemy.Target.transform.position;
                        }
                        else
                        {
                            // select the furthest point we have clear path to
                            //foreach (var position in enemy.Room.FindPath(transform.position, enemy.Target.transform.position))
                            //{
                            //    if (enemy.HasClearPath(position, enemy.Target.transform))
                            //        targetPosition = position;
                            //    else
                            //        break;
                            //}
                        }
                        //Debug.DrawLine(transform.position, targetPosition, Color.magenta);
                    }
                    targetDirection = targetPosition - transform.position;
                    break;
                case EnemyMovementType.Wander:
                    targetPosition = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * WanderRadius);
                    targetDirection = targetPosition - transform.position;
                    break;
                case EnemyMovementType.DiagonalBounce:
                    if (Mathf.Abs(targetDirection.x) != 1 || Mathf.Abs(targetDirection.y) != 1)
                    {
                        targetDirection = new Vector3(1, 1);
                    }
                    else
                    {
                        //if (Controller.collisionState.above)
                        //    targetDirection = new Vector3(targetDirection.x, -1);
                        //else if (Controller.collisionState.below)
                        //    targetDirection = new Vector3(targetDirection.x, 1);
                        //else if (Controller.collisionState.left)
                        //    targetDirection = new Vector3(1, targetDirection.y);
                        //else
                        //    targetDirection = new Vector3(-1, targetDirection.y);
                    }
                    break;
            }
        }

        public void Stun(float length)
        {
            stunnedUntil = Time.time + length;
            //print(stunnedUntil);
        }

        public void Fear(float length)
        {
            fearTime = Time.time + length;
            MoveSpeed = -1f * MoveSpeed;
            isFeared = true;
        }

        Vector3 RandomOrthogonalDirection()
        {
            switch (UnityEngine.Random.Range(0, 4 /* exclusive */))
            {
                case 0:
                    return Vector3.up;
                case 1:
                    return Vector3.right;
                case 2:
                    return Vector3.down;
                case 3:
                    return Vector3.left;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void FaceCenter()
        {
            //if (enemy.Room == null)
            //    return;

            //if (Mathf.Abs(enemy.Room.transform.position.y - transform.position.y) > Mathf.Abs(enemy.Room.transform.position.x - transform.position.x))
            //{
            //    if (enemy.Room.transform.position.y - transform.position.y < 0)
            //        visual.Facing = Direction.Down;
            //    else
            //        visual.Facing = Direction.Up;
            //}
            //else
            //{
            //    if (enemy.Room.transform.position.x - transform.position.x < 0)
            //        visual.Facing = Direction.Left;
            //    else
            //        visual.Facing = Direction.Right;
            //}
        }
    }
}