using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    [RequireComponent(typeof(Enemy))]
    public class Charge : Move
    {
        public float ChargeDelay = 0.2f;
        public float ChargeTimeLength;
        public float ChargeSpeed = 20f;
        public float StunTime = 1.0f;
        public bool UpdateWithRotation;
        public GameObject SpawnOnCollide;

        public float ExplosionDelay;

        Vector3 targetDirection;
        Quaternion targetAngle;

        public override void DoMove()
        {
            //controller.onControllerCollidedEvent += OnColliderHit;

            Aim();

            if (!UpdateWithRotation)
            {
                this.Delay(() =>
                {
                    Aim();
                    Movement = targetAngle * (Vector3.forward * ChargeSpeed); 
                    //print("movement " + Movement);
                }, ChargeDelay);
            }

            //if (ExplosionDelay != 0)
            //    this.Delay(() => Explosion.Create(transform.position, Enemy.gameObject), ExplosionDelay);
        }

        void Aim()
        {
            if (Enemy.Target != null)
            {
                targetDirection = Enemy.Target.transform.position - transform.position;
            }

            if (targetDirection != Vector3.zero)
            {
                //print(targetDirection);
                targetAngle = Quaternion.LookRotation(targetDirection);
                //print(targetAngle.eulerAngles);
            }
        }

        //void OnTouchDamage(Player player)
        //{
        //    print("attempting punt");

        //    // For this to be triggered the player layer must be turned on on the CharacterController2d
        //    if (player != null)
        //    {
        //        //if (player.Health.IsInvulnerable && !player.Health.IsJustTurnedInvulnerable)
        //        //{
        //        //    print("player is invuln");
        //        //}
        //        //else
        //        //{
        //            var adjacentRoom = player.Room.GetAdjacentRoom(visual.Facing);
        //            if (adjacentRoom != null && (adjacentRoom.Type == RoomType.Normal || adjacentRoom.Type == RoomType.Start))
        //            {
        //                player.Broadcast(player.name + " got punted!", Utility.Cyan);
        //                player.RemoteGoto(adjacentRoom);
        //            }
        //            else
        //            {
        //                var playerRigid = player.GetComponent<Rigidbody2D>();
        //                playerRigid.AddForce(Movement * 50);
        //            }

        //            player.Health.Damage(1, hit.point, transform, DamageType.Physical);
        //            player.Movement.Stun(StunTime);
        //        //}
        //    }
        //}

        void OnColliderHit(RaycastHit2D hit)
        {
            //print(hit.collider.name);

            this.Spawn(SpawnOnCollide, transform.parent);

            EndMove();
        }

        public override void MoveUpdate()
        {
            if (UpdateWithRotation && Time.time > MoveStartTime + ChargeDelay)
            {
                Aim();
                Movement = targetAngle * (Vector3.forward * ChargeSpeed);
            }

            if (ChargeTimeLength > 0 && Time.time > MoveStartTime + ChargeTimeLength)
                Movement = Vector3.zero;
        }

        public override void OnEnd()
        {
            base.OnEnd();

            //controller.onControllerCollidedEvent -= OnColliderHit;
        }
    }
}