using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    //[RequireComponent(typeof(Rigidbody))] // OnControllerColliderHit doesn't seem to get called if no rigid is present
    [RequireComponent(typeof(Collider2D))]
    public class TouchDamage : MonoBehaviour
    {
        public float Amount = 0.5f;
        public TargetType Targets = TargetType.Players;
        public bool IsFamiliar = false;
        public float Cooldown = 0.1f;
        public float MinimumAge = 0;
        public bool OnEnterOnly;
        public DamageType DamageType = DamageType.Physical;
        public float KnockbackVelocity;
        public float TakeDamageWhenDealDamage;

        [NonSerialized]
        public GameObject DamageSource;

        float nextHit;

        float startTime;

        void Awake()
        {
            startTime = Time.time;
            DamageSource = gameObject;
            var damageable = GetComponentInParent<Damageable>();
            if (damageable != null)
                DamageSource = damageable.gameObject;

            // check to make sure there is a trigger collider on us.
            //foreach (var collider in GetComponents<Collider>())
            //{
            //    if (collider.isTrigger)
            //        return;
            //}
            //throw new Exception("TouchDamage requires a trigger collider to be present or else a player moving into it will not trigger damage.");
        }

        //void OnCollisionStay(Collision collision)
        //{
        //    print("TouchDamage collision with " + collision.collider.name);
        //}

        void OnTriggerEnter2D(Collider2D other)
        {
            //print("TouchDamage OnTriggerEnter from " + name + " to " + other.name);

            if (other.isTrigger)
                return;

            var damageable = other.gameObject.GetComponent<Damageable>();
            if (damageable != null)
            {
                CollideWith(other, other.bounds.ClosestPoint(transform.position), damageable);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            //print("TouchDamage OnTriggerStay from " + name + " to " + other.name);

            if (other.isTrigger)
                return;

            if (!OnEnterOnly)
            {
                CollideWith(other, other.bounds.ClosestPoint(transform.position), other.gameObject.GetComponent<Damageable>());
            }
        }

        //called when we move this enemy and it collides with a non-trigger collider.

        //void OnControllerColliderHit(ControllerColliderHit hit)
        //{
        //    print("TouchDamage OnControllerColliderHit from " + name + " to " + hit.collider.name);
        //    CollideWith(hit.collider, hit.point, hit.collider.GetComponent<Damageable>());
        //}

        void OnCollisionEnter2D(Collision2D collision)
        {
            //print("TouchDamage OnCollisionEnter from " + name + " to " + collision.collider.name);
            if (collision.contacts.Length > 0)
                CollideWith(collision.collider, collision.contacts[0].point, collision.collider.gameObject.GetComponent<Damageable>());
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            //print("TouchDamage OnCollisionStay from " + name + " to " + collision.collider.name);
            if (collision.contacts.Length > 0 && !OnEnterOnly)
                CollideWith(collision.collider, collision.contacts[0].point, collision.collider.gameObject.GetComponent<Damageable>());
        }

        void CollideWith(Collider2D target, Vector3 position, Damageable damageable)
        {
            if (Time.time - startTime < MinimumAge ||
                damageable == null ||
                damageable.gameObject == gameObject ||
                Amount <= 0 ||
                !enabled ||
                Time.time < nextHit ||
                damageable.IsInvulnerable)
            {
                return;
            }

            //print("TouchDamage CollideWith " + target.name + " at " + position.ToString());

            if (!damageable.IsType(Targets))
                return;

            nextHit = Time.time + Cooldown;

            if (!damageable.IsInvulnerable)
            {
                damageable.Damage(Amount, position, DamageSource, DamageType);
                if (TakeDamageWhenDealDamage > 0)
                {
                    var selfDamageable = GetComponentInChildren<Damageable>();
                    if (selfDamageable != null)
                    {
                        selfDamageable.Damage(TakeDamageWhenDealDamage, position, gameObject, DamageType.Unblockable);
                    }
                }

                if (!damageable.IsDead && damageable.IsPlayer)
                {
                    var player = damageable.GetComponent<Player>();

                    if (KnockbackVelocity > 0 && player != null)
                        player.Movement.Knockback(position, KnockbackVelocity);
                }
            }
        }
    }
}