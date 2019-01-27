using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public enum EnemyAnimation
    {
        Idle,
        Walk,
        Hurt,
        Death,
        Attack,
        Attack1,
        Attack2,
        Damage,
        Appear,
    }

    public class EnemyVisual : MonoBehaviour
    {
        public bool HasFacing = true;
        public bool ForceAnimation;

        public bool IsMoving;
        public Direction Facing = Direction.Down;
        public EnemyAnimation Animation;

        Animator animator;
        Enemy enemy;
        EnemyAnimation lastAnimation;
        Direction lastFacing = Direction.None;
        EnemyAnimation tempAnimation;
        double tempAnimationUntil;

        public bool IsTemporaryAnimationPlaying { get { return Time.time < tempAnimationUntil; } }

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            enemy = GetComponent<Enemy>();
        }

        void Update()
        {
            UpdateAnimation();
        }

        public void SetTemporaryAnimation(EnemyAnimation animation, float length)
        {
            //print("Temp " + animation);
            tempAnimation = animation;
            tempAnimationUntil = Time.time + length;
            UpdateAnimation();
        }

        public void ClearTemporaryAnimation()
        {
            //print("Clear Temp Animation " + tempAnimation);
            tempAnimationUntil = 0;
            UpdateAnimation();
        }

        public void UpdateAnimation()
        {
            EnemyAnimation result;
            if (enemy != null && enemy.Health.IsDead)
            {
                result = EnemyAnimation.Death;
            }
            else
            {
                if (IsMoving)
                    result = EnemyAnimation.Walk;
                else
                    result = EnemyAnimation.Idle;

                if (Application.isPlaying && IsTemporaryAnimationPlaying)
                    result = tempAnimation;
            }

            if (!ForceAnimation)
                Animation = result;
            GenerateVisual();
        }

        public void GenerateVisual()
        {
            if (animator != null)
            {
                if (lastAnimation == Animation && lastFacing == Facing)
                    return;

                lastAnimation = Animation;
                lastFacing = Facing;

                var state = HasFacing
                    ? String.Format("{0} {1}", Animation.ToString().AddSpaces(), Facing)
                    : Animation.ToString().AddSpaces();

                //print(name);
                //print(state);

                //animator.Play(state, 0);
            }
        }
    }
}