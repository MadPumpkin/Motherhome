using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public enum BallisticTargetMode
    {
        Direct,
        BallisticAtPlayer,
    }

    public class Shoot : Move
    {
        [Header("Attacking")]
        public GameObject Projectile;
        [Tooltip("Accuracy deviation in degrees (0 = spot on)")]
        public float ProjectileSpread = 0.0f;
        public AudioClip FireSound;
        public float FireDelay = 0.5f;
        public Transform Tip;
        public float InitialDistanceFromTip;
        public bool FireAtPlayer;
        public bool ShootOrthogonal;
        public int RepeatCount = 1;
        public float RepeatDelay = 0.1f;
        public float RepeatOffset = 0;
        public float StartOffset;

        public int SpawnLimit = 0;

        int currentSpawnCount;

        //public BallisticTargetMode BallisticTargetMode;
        //[Range(20f, 70f)]
        //public float BallisticAngle;
        //[Tooltip("Distance in meters from the player that the ballistic shot will be aimed at.")]
        //public float BallisticSpread;

        // single shot
        // multiple shot
        // spread shot
        // blast shot
        // wave

        public Shoot()
        {
            Frequency = MoveFrequency.Repeat;
        }

        protected override bool CanStart()
        {
            if (SpawnLimit <= 0)
                return true;

            return currentSpawnCount < SpawnLimit;
        }


        public override void DoMove()
        {
            UpdateRotation();

            //print("Shoot domove");
            for (int i = 0; i < RepeatCount; i++)
            {
                if (currentSpawnCount > SpawnLimit)
                    return;

                var interation = i; // copy to escape lambda variable

                this.Delay(() =>
                {
                    DoShot(interation);
                }, FireDelay + (i * RepeatDelay));
            }
            //Debug.Break();
        }

        void DoShot(int interation)
        {
            if (Enemy.Health.IsDead)
                return;

            if (FireAtPlayer && Enemy.Target == null)
                return;

            this.PlaySound(FireSound);

            //print(transform.rotation.eulerAngles);
            //print(Tip.rotation.eulerAngles);

            var rotation = UpdateRotation();

            if (ShootOrthogonal)
            {
                if (Enemy.Target != null)
                {
                    var target = Enemy.Target.transform.position - transform.position;
                    //if (enemyMovement.AlwaysFaceCenter)
                    //{
                    //    target = Enemy.Room.transform.position - transform.position;
                    //}

                    if (Mathf.Abs(target.y) > Mathf.Abs(target.x))
                    {
                        target = new Vector3(0, target.y);
                        rotation = Quaternion.FromToRotation(Vector3.up, target);
                    }
                    else
                    {
                        target = new Vector3(target.x, 0);
                        rotation = Quaternion.FromToRotation(Vector3.up, target);
                    }
                }
            }

            var angle = StartOffset + UnityEngine.Random.Range(-ProjectileSpread, ProjectileSpread) + (RepeatOffset * interation);
            rotation *= Quaternion.Euler(0, 0, angle);

            var position = Tip.position;
            if (InitialDistanceFromTip > 0)
            {
                position += rotation * new Vector3(0, InitialDistanceFromTip);
                if (Physics2D.OverlapCircle(position, 0.1f))
                    return;
            }

            var result = Legend.Projectile.Create(Projectile, position, rotation, Enemy.Health);
            result.gameObject.IgnoreCollisionsWith(gameObject);

            var enemy = result.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDestroyEvent += () => currentSpawnCount--;
                currentSpawnCount++;
                Destroy(result);
            }

            //var projectile = (PhotonNetwork.Instantiate(Projectile, Tip.position, rotation, 0) as GameObject).GetComponent<Projectile>();
            //if (projectile != null)
            //{
            //    projectile.Source = Enemy.Health;

            //    //if (FireAtPlayer)
            //    //    projectile.transform.LookAt(Enemy.Target.transform);

            //    ////print("Projectile Pre Rotate: " + projectile.transform.rotation.eulerAngles);

            //    var angle = StartOffset + UnityEngine.Random.Range(-ProjectileSpread, ProjectileSpread) + (RepeatOffset * interation);
            //    projectile.transform.Rotate(0, 0, angle);

            //    //switch (BallisticTargetMode)
            //    //{
            //    //    case BallisticTargetMode.BallisticAtPlayer:
            //    //        if (Enemy.Target != null)
            //    //        {
            //    //            var targetPosition = Enemy.Target.transform.position;
            //    //            if (BallisticSpread != 0)
            //    //            {
            //    //                var spread = UnityEngine.Random.insideUnitCircle * BallisticSpread;
            //    //                targetPosition += new Vector3(spread.x, 0, spread.y);
            //    //            }

            //    //            projectile.LaunchAt(targetPosition, BallisticAngle);
            //    //            var rigid = projectile.GetComponent<Rigidbody>();
            //    //            if (rigid != null)
            //    //                rigid.velocity = Quaternion.Euler(0, 0, angle) * rigid.velocity;
            //    //        }
            //    //        //Debug.DrawLine(targetPosition, projectile.transform.position, Color.magenta);
            //    //        //TrajectorySimulation.DebugPath(projectile.GetComponent<Rigidbody>());
            //    //        //Debug.Break();
            //    //        break;
            //    //}

            //    //print("Projectile: " + projectile.transform.rotation.eulerAngles);
            //    //Debug.DrawRay(Tip.position, Tip.rotation * Vector3.forward);
            //    //Debug.Break();

            //    projectile.gameObject.IgnoreCollisionsWith(gameObject);

            //    //var projectileCollider = projectile.GetComponent<Collider>();
            //    //foreach (var collider in GetComponentsInChildren<Collider>())
            //    //{
            //    //    Physics.IgnoreCollision(projectileCollider, collider);
            //    //}
            //}
        }

        Quaternion UpdateRotation()
        {
            var result = Tip.rotation;
            if (FireAtPlayer && Enemy.Target != null)
            {
                var targetPosition = Enemy.Target.transform.position - transform.position;
                targetPosition = new Vector3(targetPosition.x, targetPosition.y);
                result = Quaternion.FromToRotation(Vector3.up, targetPosition);

                if (Mathf.Abs(targetPosition.y) > Mathf.Abs(targetPosition.x))
                {
                    if (targetPosition.y < 0)
                    {
                        visual.Facing = Direction.Down;
                        visual.Animation = EnemyAnimation.Attack;
                    }
                    else
                    {
                        visual.Facing = Direction.Up;
                        visual.Animation = EnemyAnimation.Attack;
                    }
                }
                else
                {
                    if (targetPosition.x < 0)
                    {
                        visual.Facing = Direction.Left;
                        visual.Animation = EnemyAnimation.Attack;
                    }
                    else
                    {
                        visual.Facing = Direction.Right;
                        visual.Animation = EnemyAnimation.Attack;
                    }
                }
            }

            return result;
        }
    }
}