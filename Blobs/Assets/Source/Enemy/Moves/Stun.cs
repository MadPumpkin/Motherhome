using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    [RequireComponent(typeof(Enemy))]
    public class Stun : Move
    {
        public Stun()
        {
            AnimationTrigger = EnemyAnimation.Hurt;
            Frequency = MoveFrequency.Once;
            DurationMinimum = 2f;
            DurationMaximum = 2f;
            IsDurationAnimationLength = false;
        }
    }
}