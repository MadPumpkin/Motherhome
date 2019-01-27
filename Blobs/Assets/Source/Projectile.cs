using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Legend
{
    public enum TargetType
    {
        Players,
        Enemies,
        All
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        public float Damage = 10;
        [Tooltip("Can be set to 0 for infinite duration.")]
        public float MaxAge = 1;

        [Tooltip("This force will be applied to the projectile to give it it's initial velocity.")]
        public float ImpulseForce = 10;
        [Tooltip("A random amount of force between zero and this value will be added to the initial velocity.")]
        public float ImpulseRange = 0;
        public float Range = 15;
        public float FadeLength = 0.15f;
        public float FadeLengthOnMiss = 0.15f;
        public float RemoveDelayLength = 0.15f;

        public GameObject MuzzleFire;
        public bool ScaleVisualWithDamage = true;

        public AudioClip[] HitSound;
        public float HitSoundVolume = .5f;
        public GameObject SpawnOnHit;
        public GameObject SpawnDebrisOnHit;
        public Vector3 Acceleration;
        public bool FreezeOnHit = true;
        public bool ExplodeOnCollision;
        public bool IsFixedRotation;

        [Header("Special")]
        public float StunDuration = 0;
        public TargetType Targets;
        public float Homing;
        public bool Pierce;

        [NonSerialized]
        public Damageable Source;
        [NonSerialized]
        public bool HasHit;

        Damageable target;
        Rigidbody2D rigidBody;
        float rangeSquared;
        Vector3 startPosition;
        Animator animator;
        float startTime;

        public static GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, Damageable source, float damageAmount = 0, float force = 0, float range = 0, Vector2 velocityOffset = default(Vector2))
        {
            var result = Instantiate(prefab, position, rotation);
            var projectile = result.GetComponent<Projectile>();
            var rigid = result.GetComponent<Rigidbody2D>();

            if (projectile != null)
            {
                projectile.Source = source;
                projectile.Damage += damageAmount;
                projectile.ImpulseForce += force;
                projectile.Range += range;
            }
            //else
            //{
            //    rigid.velocity += (Vector2)(result.transform.rotation * (Vector3.up * force));
            //}

            if (rigid != null)
                rigid.velocity += velocityOffset;

            //Debug.Log(Player.Instance.name);
            //Debug.Log(Player.Instance.isLocalPlayer);
            return result;
        }

        public virtual void Start()
        {
            startTime = Time.time;
            animator = GetComponentInChildren<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();    

            if (MuzzleFire)
                Instantiate(MuzzleFire, transform.position, transform.rotation);

            ImpulseForce += UnityEngine.Random.value * ImpulseRange;

            if (ImpulseForce != 0)
                rigidBody.velocity += (Vector2)(transform.rotation * (Vector3.up * ImpulseForce));

            var actualSpeed = Quaternion.Inverse(transform.rotation) * rigidBody.velocity;
            //print(rigidBody.velocity + " is going " + actualSpeed);
            if (actualSpeed.y < 1)
            {
                // set a minimum true projectile speed so that they never move backwards
                rigidBody.velocity = (Vector2)(transform.rotation * (Vector3.up * 1));
            }

            var damageable = GetComponent<Damageable>();
            if (damageable != null)
                damageable.Respawn();

            var scale = (Damage / 1f) + 0.2f;
            if (ScaleVisualWithDamage)
                transform.localScale = new Vector3(scale, scale, 1);
            //rigidBody.mass = scale;

            if (Pierce)
            {
                foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
                    renderer.sortingOrder = 99;
            }

            if (Source != null)
                gameObject.IgnoreCollisionsWith(Source.gameObject);

            rangeSquared = Range * Range;
            startPosition = transform.position;

            if (IsFixedRotation)
                transform.rotation = Quaternion.identity;
        }

        //public void LaunchAt(Vector3 target, float ballisticAngle)
        //{
        //    // source: http://www.theappguruz.com/blog/hit-target-using-ballistic-trajectory
        //    var direction = target - transform.position; // get target direction
        //    var height = direction.y; // get height difference
        //    direction.y = 0; // retain only the horizontal direction
        //    var distance = direction.magnitude; // get horizontal distance
        //    var angle = ballisticAngle * Mathf.Deg2Rad; // convert angle to radians
        //    direction.y = distance * Mathf.Tan(angle); // set dir to the elevation angle
        //    distance += height / Mathf.Tan(angle); // correct for small height differences
        //                                           // calculate the velocity magnitude
        //    var velocity = Mathf.Sqrt(distance * -Physics.gravity.y / Mathf.Sin(2 * angle));
        //    if (!float.IsNaN(velocity))
        //    {
        //        var localVelocity = velocity * direction.normalized;
        //        // launch the cube by setting its initial velocity
        //        GetComponent<Rigidbody>().velocity = localVelocity;
        //        ImpulseForce = 0;
        //    }
        //}

        void FixedUpdate()
        {
            if (HasHit)
                return;

            if (Homing != 0)// && hasAuthority)
            {
                if (target == null)
                    target = transform.position.FindClosestTarget(Targets);

                if (target != null)
                {
                    //print("Homing to " + target.name);

                    var targetPoint = target.transform.position;

                    Debug.DrawLine(transform.position, targetPoint);

                    var direction = targetPoint - transform.position;
                    var toRotation = Quaternion.FromToRotation(Vector3.up, direction);
                    ////var toRotation = Quaternion.FromToRotation(transform.forward, direction);
                    //var toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Homing * Time.deltaTime);
                    rigidBody.velocity = (transform.rotation * (Vector3.up * ImpulseForce));
                }
            }

            if (Acceleration != Vector3.zero)
                rigidBody.velocity += (Vector2)((transform.rotation * Acceleration) * Time.fixedDeltaTime);

            var distance = (transform.position - startPosition).sqrMagnitude;
            if (distance > rangeSquared || (MaxAge != 0 && Time.time - startTime > MaxAge - FadeLength))
            {
                HasHit = true;
                StopAllCoroutines();
                this.DisableColliders();

                rigidBody.velocity = Vector3.zero;

                FadeAndDestroy();

                if (ExplodeOnCollision)
                    Explode();
            }
        }

        bool isDestroying = false;
        void FadeAndDestroy()
        {
            if (isDestroying)
                return;
            isDestroying = true;

            if (animator != null)
                animator.Play("Destroy");
            if (FadeLengthOnMiss > 0)
                iTween.FadeTo(gameObject, 0, FadeLengthOnMiss);
            Destroy(gameObject, RemoveDelayLength);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            DoCollision(collision.collider);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            DoCollision(collider);
        }

        void OnDestroy()
        {
            if (ExplodeOnCollision)
                Explode();
        }

        protected virtual void DoCollision(Collider2D collider)
        {
            if (HasHit)
                return;

            //print("projectile collision with " + collider.name + " for damage " + Damage);

            var spin = GetComponentInChildren<Spin>();
            if (spin != null)
                Destroy(spin);

            var audioClip = GetComponentInChildren<AudioSource>();
            if (audioClip != null && audioClip.isPlaying)
                audioClip.Stop();

            this.PlaySound(HitSound, HitSoundVolume);
            this.Spawn(SpawnDebrisOnHit, null);

            if (Pierce)
            {
                gameObject.IgnoreCollisionsWith(collider.gameObject);
            }
            else
            {
                HasHit = true;

                if (FreezeOnHit)
                {
                    this.DisableColliders();

                    foreach (var system in GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
                    }

                    var rigid = GetComponent<Rigidbody2D>();
                    rigid.velocity = Vector3.zero;
                    rigid.isKinematic = true;
                    rigid.simulated = false;
                    transform.parent = collider.transform;
                }

                FadeAndDestroy();
            }

            var damageable = collider.GetComponentInChildren<Damageable>();

            if (ExplodeOnCollision)
                Explode();

            var spawned = this.Spawn(SpawnOnHit, null); // used by flame
            if (spawned != null)
                spawned.transform.rotation = Quaternion.identity;

            if (damageable != null && damageable.IsType(Targets) && !damageable.IsInvulnerable)
            {
                damageable.Damage(Damage, transform.position, Source == null ? null : Source.gameObject);

                if (StunDuration > 0 && !damageable.IsDead)
                    damageable.Stun(StunDuration);
            }
        }

        void Explode()
        {
            ExplodeOnCollision = false;
            //if (Source != null && Source.hasAuthority)
            //    Source.CmdExplode(transform.position, Damage * 2.5f);
            FadeAndDestroy();
        }
    }
}