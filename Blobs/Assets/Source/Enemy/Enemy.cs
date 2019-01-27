using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public enum EnemyTargetingType
    {
        ClosestWithLineOfSight,
    }

    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(EnemyVisual))]
    public class Enemy : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip[] SpawnSounds;
        public AudioClip[] HitSounds;
        public AudioClip[] FootstepSounds;
        public float FootstepInterval = 0.3f;
        public float FootstepVolume = 0.1f;
        public AudioClip[] DeathSounds;
        public AudioClip[] JumpToRoomSound;

        [Header("Loot")]
        //public String[] Loot;
        public float LootCount;
        public List<GameObject> DropListOverride;

        [Header("Behavior")]
        public bool IsExplodeOnDeath;
        public bool IsGoDarkOnDeath = true;
        public float RespawnTime;
        public EnemyTargetingType Targeting;
        public bool UseHurtAnimation = true;
        public bool IsDeathRequiredToClearRoom = true;

        [NonSerialized]
        public Damageable Health;
        [NonSerialized]
        public Damageable Target;
        [NonSerialized]
        public Action OnHitEvent;
        [NonSerialized]
        public Action OnDieEvent;
        [NonSerialized]
        public Action OnDestroyEvent;
        [NonSerialized]
        public EnemyMovement Movement;

        Move currentMove;
        public Move CurrentMove
        {
            get { return currentMove; }
            set
            {
                currentMove = value;
                CurrentMoveId = currentMove == null ? -1 : currentMove.Id;
            }
        }

        [NonSerialized]
        public int CurrentMoveId;

        AudioSource audioSource;
        float timeToTargetCheck = -targetCheckInterval;
        const float targetCheckInterval = 0.1f;
        Text callout = null;
        Move[] moves;

        public Bounds Bounds;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            Health = GetComponent<Damageable>();
            Movement = GetComponent<EnemyMovement>();

            foreach (var child in GetComponentsInChildren<Renderer>())
            {
                Bounds.Encapsulate(child.bounds);
            }

            moves = GetComponents<Move>();
            var moveId = 0;
            foreach (var move in moves)
            {
                move.Id = moveId;
                moveId++;
            }
        }

        void Start()
        {
#if UNITY_EDITOR
            //callout = Utility.AddCallout(transform, name).GetComponentInChildren<Text>();
            //callout.GetComponentInChildren<TypewriterEffect>().enabled = false;
#endif

            Respawn();
            Update(); // to ensure that room gets set
        }

        public void Respawn()
        {
            //print("respawning " + name);
            Health.Respawn();

            this.Delay(() => this.PlaySound(SpawnSounds), UnityEngine.Random.value);
            this.EnableColliders();

            foreach (var move in GetComponents<Move>())
            {
                if (move.Frequency == MoveFrequency.Start && CurrentMove != move)
                {
                    move.StartMove();
                    break;
                }
            }

            // do not clear current move here or SpawnMove will stop working.
            //CurrentMove = null;

            //  audioSource.Play();
        }

        void Update()
        {
            //print(CurrentMove);

            if (callout != null)
            {
                callout.text = (CurrentMove == null ? name : CurrentMove.ToString()) + " " + Health.ToString();
            }

            if (timeToTargetCheck < Time.time)
            {
                timeToTargetCheck = Time.time + targetCheckInterval;

                switch (Targeting)
                {
                    case EnemyTargetingType.ClosestWithLineOfSight:
                        Target = transform.position.FindClosestTarget(TargetType.Players, World.Instance.LOSCheckMask);
                        break;
                }
            }
        }

        public void OnHit(Damage damage)
        {
            //anim.SetTrigger("Hit");
            this.PlaySound(HitSounds);

            if (UseHurtAnimation)
            {
                var visual = GetComponent<EnemyVisual>();
                if (!visual.IsTemporaryAnimationPlaying)
                    visual.SetTemporaryAnimation(EnemyAnimation.Hurt, 0.25f);
            }

            if (OnHitEvent != null)
                OnHitEvent();
        }

        public void OnDie()
        {
            if (callout != null)
                Destroy(callout.transform.parent.gameObject);

            if (CurrentMove != null)
                CurrentMove.EndMove();

            //print("OnDie");
            this.PlaySound(DeathSounds);

            if (!Health.TakesDamageEvenWhenDead)
                this.DisableColliders();

            //controller.enabled = false;

            if (audioSource != null && audioSource.isPlaying && audioSource.loop)
                audioSource.Stop();

            Health.StopBlinking();

            if (IsGoDarkOnDeath)
            {
                iTween.ColorTo(gameObject,
                    iTween.Hash(
                        "color", Color.grey,
                        "time", 0.25f,
                        "easeType", iTween.EaseType.linear
                    )
                );
            }

            //if (IsExplodeOnDeath)
            //{
            //    this.Delay(() => Explosion.Create(transform.position, gameObject), 0.1f);
            //}

            if (OnDieEvent != null)
                OnDieEvent();

            DropLoot();

            if (RespawnTime != 0)
                this.Delay(Respawn, RespawnTime);
        }

        void OnDestroy()
        {
            if (OnDestroyEvent != null)
                OnDestroyEvent();
        }

        public void DropLoot()
        {
            var amount = LootCount * Pickup.DropRateMultiplier();

            var dropList = DropListOverride;
            if (dropList == null || dropList.Count == 0)
                dropList = Level.Instance.EnemyDropPickups;

            while (amount > 0)
            {
                var target = amount;
                if (amount > Pickup.MaxDropProbability)
                {
                    target = Pickup.MaxDropProbability;
                }
                amount -= Pickup.MaxDropProbability;

                var value = UnityEngine.Random.value;
                //print(value);

                if (value < target)
                {
                    var prefab = dropList.RandomOrDefault();
                    Pickup.Spawn(transform.parent, transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.1f), prefab, null);
                }
            }
        }

        public bool HasClearPath(Vector3 target, Transform excludeTarget)
        {
            return Bounds.HasClearPath(target, excludeTarget);
        }
    }
}