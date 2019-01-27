using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public enum MoveFrequency
    {
        Once,
        Repeat,
        Never,
        Start,
        OnHit,
        OnDeath
    }

    [RequireComponent(typeof(Enemy))]
    public class Move : MonoBehaviour
    {
        public EnemyAnimation AnimationTrigger;
        public float AnimationTriggerDelay;
        public bool IsMovingAnimation;
        public int MoveRepeatCount = 1;
	    public bool RequiresLineOfSite = true;
	    public bool RequiresClearPath = false;
        [Header("Timing")]
        public MoveFrequency Frequency = MoveFrequency.Repeat;
        public float DelayMinimum;
        public float DelayMaximum;
        public float DurationMinimum;
        public float DurationMaximum;
        public bool IsDurationAnimationLength = true;
        public float MaxTargetDistance;

        [Header("Audio")]
        public AudioClip[] StartSounds;
        public AudioClip[] DelayedSounds;
        public float DelayedSoundsDelay;
        public AudioClip[] EndSounds;

        [Header("Effects")]
        public GameObject SpawnOnStart;
        public Vector3 SpawnOnStartOffset;
        public float SpawnOnStartDelay;
        public bool IsInvulnerable;
        public bool IsVulnerable;
        public Move OnComplete;

        [NonSerialized]
        public Vector3 Movement;
        [NonSerialized]
        public Enemy Enemy;
        //[NonSerialized]
        //public CharacterController2D controller;
        [NonSerialized]
        public int Id; // used for sending current move over wire

        protected float MoveStartTime;

        protected float timeToMove;
        protected EnemyVisual visual;
        protected EnemyMovement enemyMovement;
        bool isPlaying;
        int count = -1;

        void Awake()
        {
            Enemy = GetComponent<Enemy>();
            visual = GetComponentInChildren<EnemyVisual>();
            //controller = GetComponentInParent<CharacterController2D>();
            enemyMovement = GetComponentInParent<EnemyMovement>();

            //name = AnimationTrigger;

            if (Frequency == MoveFrequency.OnHit)
                Enemy.OnHitEvent = OnHitEvent;

            if (Frequency == MoveFrequency.OnDeath)
                Enemy.OnDieEvent += OnDieEvent;
        }

        void OnEnable()
        {
            //print("setting move start time");
            ResetDelay();
        }

        private void ResetDelay()
        {
            timeToMove = Time.time + UnityEngine.Random.Range(DelayMinimum, DelayMaximum);
        }

        void OnHitEvent()
        {
            if (Enemy.CurrentMove == null)
                StartMove();
        }

        void OnDieEvent()
        {
            StartMove();
        }

        protected virtual bool CanStart()
        {
            return true;
        }

	    protected virtual void Update()
        {
            if (Enemy.Health.IsDead)
            {
                //enabled = false;
                return;
            }

            if (isPlaying)
                MoveUpdate();

            if (Frequency == MoveFrequency.Never || Frequency == MoveFrequency.Start || Frequency == MoveFrequency.OnHit || Frequency == MoveFrequency.OnDeath)
                return;

            //if (RequiresClearPath)
            //{
            //    if (Enemy.Target == null || !Enemy.HasClearPath(Enemy.Target.transform.position, Enemy.Target.transform))
            //    {
            //        //ResetDelay();
            //        //return;
            //    }
            //}

            if (timeToMove < Time.time && Enemy.CurrentMove == null)
            {
                if (!CanStart())
                    return;

                if (RequiresClearPath)
                {
                    if (Enemy.Target == null || !Enemy.HasClearPath(Enemy.Target.transform.position, Enemy.Target.transform))
                    {
                        //ResetDelay();
                        return;
                    }
                }

                if (RequiresLineOfSite)
                {
                    var mask = World.Instance.LOSCheckMask;
                    if (Enemy.Target == null || !transform.position.HasLineOfSight(Enemy.Target.transform, mask))
                    {
                        //ResetDelay();
                        return;
                    }
                }

				if (RequiresLineOfSite && MaxTargetDistance > 0)
                {
                    if (Enemy.Target == null)
                        return;

                    var distanceSquared = (transform.position - Enemy.Target.transform.position).sqrMagnitude;
                    //Level.Instance.Player.HUD.BossNameLabel.text = distanceSquared.ToString();
                    if (distanceSquared > MaxTargetDistance * MaxTargetDistance)
                        return;
                }

                StartMove();
            }

            //if (isPlaying && IsDurationAnimationLength && Enemy.CurrentMove == this)
            //{
            //    var currentState = anim.GetCurrentAnimatorStateInfo(0);
            //    if (!currentState.IsName(AnimationTrigger))
            //    {
            //        //Do something if this particular state is palying
            //        EndMove();
            //    }
            //}
        }

        public void StartMove()
        {
            if (this == null) // can happen when called from Legend.Enemy.OnPhotonSerializeView
                return;

            if (Enemy.Health.IsDead && Frequency != MoveFrequency.OnDeath)
            {
                //enabled = false;
                return;
            }

            if (count == -1)
            {
                count = MoveRepeatCount - 1;
            }

            //print("Starting move " + ToString());

            timeToMove = float.MaxValue;
            Movement = Vector3.zero;
            Enemy.CurrentMove = this;
            MoveStartTime = Time.time;

            this.PlaySound(StartSounds);

            if (SpawnOnStart != null)
            {
                this.Delay(() => {
                    //if (this == null)
                    //    throw new NullReferenceException("Null after delay");
                    this.Spawn(SpawnOnStart, transform.position + SpawnOnStartOffset, transform.parent);
                }, SpawnOnStartDelay);
            }

            if (DelayedSounds != null && DelayedSounds.Length > 0)
                this.Delay(() => this.PlaySound(DelayedSounds), DelayedSoundsDelay);

            if (AnimationTrigger != EnemyAnimation.Idle)
            {
                this.Delay(() =>
                {
                    //print(AnimationTrigger);
                    visual.SetTemporaryAnimation(AnimationTrigger, float.MaxValue);
                }, AnimationTriggerDelay);
            }

            if (IsInvulnerable)
                Enemy.Health.Invulnerable(float.PositiveInfinity);
            else
                Enemy.Health.Invulnerable(0);

            if (IsVulnerable)
                Enemy.Health.Vulnerable(float.PositiveInfinity);

            if (!IsDurationAnimationLength)
                this.Delay(() => EndMove(), UnityEngine.Random.Range(DurationMinimum, DurationMaximum));

            isPlaying = true;

            DoMove();
        }

        public virtual void DoMove()
        {
        }

        public virtual void MoveUpdate()
        {
        }

        public void EndMove()
        {
            visual.ClearTemporaryAnimation();

            //print("endMove " + name);
            //if (Enemy.Health.IsDead && Frequency != MoveFrequency.OnDeath)
            //    return;

            if (count > 0)
            {
                count--;
                StartMove();
                return;
            }
            count = -1;            

            OnEnd();

            isPlaying = false;

            if (Enemy.CurrentMove == this)
            {
                Enemy.CurrentMove = null;

                if (IsInvulnerable)
                    Enemy.Health.Invulnerable(0);

                if (IsVulnerable)
                    Enemy.Health.Vulnerable(0);
            }

            this.PlaySound(EndSounds);

            if (OnComplete != null)
                OnComplete.StartMove();

            if (Frequency == MoveFrequency.Repeat)
            {
                ResetDelay();
            }
            else if (Frequency == MoveFrequency.Once || Frequency == MoveFrequency.Start)
            {
                Destroy(this);
            }
        }

        public virtual void OnEnd()
        {
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2:N2}", base.ToString(), AnimationTrigger, Time.time - MoveStartTime);
        }
    }
}