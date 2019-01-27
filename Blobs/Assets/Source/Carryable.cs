using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Legend
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Carryable : Activateable
    {
        public bool BreakOnThrow;

        Rigidbody2D rigid;
        Transform originalParent;
        Player carrier;
        Player owner;
        bool thrown;
        float thrownTime;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            originalParent = transform.parent;
            ActivationText = "Pick Up";
        }

        bool doneBeingThrown;
        void LateUpdate()
        {
            if (thrown && rigid.velocity.sqrMagnitude < 0.002f && Time.time > thrownTime + 0.1f)
            {
                // delay setting thrown false a frame because a collision will stop the movement
                if (!doneBeingThrown)
                {
                    doneBeingThrown = true;
                }
                else
                {
                    doneBeingThrown = false;
                    thrown = false;

                    //var sync = GetComponent<Smooth.SmoothSync>();
                    //sync.enabled = true;
                    //if (hasAuthority)
                    //    sync.teleport();

                    if (owner != null)
                        owner.gameObject.IgnoreCollisionsWith(gameObject, false);
                }
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //print("Carryable OnCollisionEnter from " + name + " to " + collision.collider.name);
            if (BreakOnThrow && thrown)
            {
                //var target = collision.collider.GetComponentInChildren<Damageable>();
                //if (target != null && !target.IsPlayer)
                //    target.Damage(owner.DamageAmount * 2, collision.contacts[0].point, owner.gameObject);

                var damageable = GetComponent<Damageable>();
                if (damageable != null && !damageable.IsPlayer)
                    damageable.Damage(damageable.Health, collision.contacts[0].point, owner.gameObject);

                //var explodable = GetComponent<Explodeable>();
                //if (explodable != null)
                //    explodable.Explode(collision.contacts[0].point);
            }
        }

        public override bool CanActivate(Player source)
        {
            return enabled &&
                source.Carrying == null &&
                carrier == null &&
                Utility.HasLineOfSight(source.transform.position, transform);// && Player.Instance.Movement.MovementType == MovementType.Normal;
        }

        protected override void DoActivate(Player source)
        {
            source.Carry(gameObject);
        }

        void OnDisable() // do ondisable here since destroy is delayed 5 sec
        {
            //            print("OnDisable");
            if (carrier != null && carrier.Carrying == this)
            {
                carrier.Carrying = null;
                HUD.Instance.ClearCommandOptions();
                carrier = null;
            }
        }

        public void Carry(Player source)
        {
            //print("Carry");

            if (source.Carrying != null || carrier != null)
                return;

            carrier = source;
            owner = source;

            source.Carrying = this;
            Unselect();
            rigid.simulated = false;
            //var sync = GetComponent<Smooth.SmoothSync>();
            ////sync.positionLerpSpeed = 0;
            //sync.enabled = false;
            transform.SetParent(source.CarryPosition);

            this.DisableColliders();

            if (source == Player.Instance)
            {
                HUD.Instance.ClearCommandOptions();
                HUD.Instance.AddCommandOption(InputAction.Bomb, "Drop");
                HUD.Instance.AddCommandOption(InputAction.Activate, "Throw");
            }

            GetComponent<SortingGroup>().sortingOrder = 99;

            //iTween.RotateTo(gameObject,
            //    iTween.Hash(
            //        "rotation", Vector3.zero,
            //        "islocal", true,
            //        "time", 3f
            //    )
            //);
            iTween.MoveTo(gameObject,
                iTween.Hash(
                    "position", Vector3.zero,
                    "islocal", true,
                    "time", 1f
                )
            );
        }

        const float throwForce = 500;

        public void Throw(Vector3 from, Vector3 velocity, Direction direction)
        {
            //print("throw");
            thrown = true;
            thrownTime = Time.time;

            //gameObject.IgnoreCollisionsWith(Player.Instance.gameObject);

            var carrier = this.carrier; // copy value because it gets cleared by drop
            Drop(true);
            transform.position = from;
            rigid.velocity = velocity;
            //GetComponent<Smooth.SmoothSync>().enabled = false;
            if (carrier != null)
            {
                carrier.gameObject.IgnoreCollisionsWith(gameObject);

                switch (direction)
                {
                    case Direction.Up:
                        transform.position += new Vector3(0, 0);
                        rigid.AddForce((Vector3.forward + Vector3.up) * throwForce);
                        break;
                    case Direction.Down:
                        transform.position += new Vector3(0, -0.75f);
                        rigid.AddForce((Vector3.forward + Vector3.down) * throwForce);
                        break;
                    case Direction.Left:
                        transform.position += new Vector3(-0.5f, -.5f);
                        rigid.AddForce((Vector3.forward + Vector3.left) * throwForce);
                        break;
                    case Direction.Right:
                        transform.position += new Vector3(0.5f, -.5f);
                        rigid.AddForce((Vector3.forward + Vector3.right) * throwForce);
                        break;
                }
            }
            //GetComponent<Smooth.SmoothSync>().enabled = true;
        }

        public void Drop(bool isThrown = false)
        {
            //print("drop");
            this.EnableColliders();

            if (Player.Instance == carrier)
                HUD.Instance.ClearCommandOptions();

            iTween.Stop(gameObject);
            rigid.simulated = true;
            //rigid.velocity = carrier.GetComponent<Rigidbody2D>().velocity;

            carrier.Carrying = null;
            carrier = null;

            this.Delay(() => GetComponent<SortingGroup>().sortingOrder = 0, 0.2f);

            //rigid.AddForce(Player.Instance.transform.rotation * (Vector3.forward) * 300);
            transform.SetParent(originalParent);

            if (!isThrown)
                BroadcastMessage("OnDrop", SendMessageOptions.DontRequireReceiver);

            //var sync = GetComponent<Smooth.SmoothSync>();
            //if (!thrown)
            //{
            //    sync.enabled = true;
            //}
            //else
            //{
            //    this.Delay(() =>
            //    {
            //        sync.enabled = true;
            //        sync.teleport();
            //    }, 1f);
            //}
            ////sync.positionLerpSpeed = 1;
            //if (hasAuthority)
            //{
            //    //sync.teleport();
            //    sync.forceStateSendNextFixedUpdate();
            //}
        }
    }
}