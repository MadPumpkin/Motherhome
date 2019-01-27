using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class HighJump : Move
    {
        [Header("High Jump")]
        public float JumpDelay = 0.5f;
        public float Height = 10;
        public float HangTime = 2;

        //bool isFlying;

        public override void DoMove()
        {
            var targetPosition = Enemy.Target.transform.position;
            targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

            //isFlying = Enemy.Movement.IsFlying;
            //this.Delay(() =>
            //{
            //    Enemy.Movement.IsFlying = true;

            //    iTween.MoveTo(gameObject, iTween.Hash(
            //        "position", transform.position + new Vector3(0, Height, 0),
            //        "time", HangTime / 2,
            //        "easetype", iTween.EaseType.easeInSine
            //    ));
            //}, JumpDelay);

            //this.Delay(() =>
            //{
            //    iTween.MoveTo(gameObject, iTween.Hash(
            //        "position", targetPosition,
            //        "time", HangTime / 2,
            //        "easetype", iTween.EaseType.easeOutSine
            //    ));
            //}, JumpDelay + (HangTime / 2));
        }

        public override void OnEnd()
        {
            //Enemy.Movement.IsFlying = isFlying;
        }
    }
}