using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class SpawnMove : Move
    {
        public bool IsFadeIn;

        public SpawnMove()
        {
            //Frequency = MoveFrequency.Start;
            ////IsDurationAnimationLength = true;
            //AnimationTrigger = EnemyAnimation.Appear;
            //if (World.Instance != null)
            //    StartSounds = World.Instance.SpawnRumbles;
        }

        public override void DoMove()
        {
            if (IsFadeIn)
            {
                //print("Start fade");
                iTween.FadeFrom(gameObject, 0, this.DurationMinimum);
            }
            base.DoMove();
        }
    }
}