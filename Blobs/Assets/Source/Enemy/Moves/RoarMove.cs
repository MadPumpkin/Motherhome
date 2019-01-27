using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class RoarMove : Move
    {
        public RoarMove()
        {
            Frequency = MoveFrequency.Once;
            IsDurationAnimationLength = true;
            AnimationTrigger = EnemyAnimation.Appear;
            //if (World.Instance != null)
            //    StartSounds = World.Instance.SpawnRumbles;
        }
    }
}