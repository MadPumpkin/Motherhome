using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legend
{
    public class Teleport : Move
    {
        Vector3 target;
        public float fadeTime = 0.5f;

        public override void DoMove()
        {
            var touchDamage = GetComponentInChildren<TouchDamage>();
            Enemy.Health.StopBlinking();
            Enemy.Health.CanBlink = false;
            iTween.FadeTo(gameObject, 0, fadeTime);
            this.Delay(() => { iTween.FadeTo(gameObject, 1, fadeTime); }, fadeTime);
            this.Delay(() => { Enemy.Health.CanBlink = true; }, fadeTime * 2);

            touchDamage.enabled = false;
            this.Delay(() => { touchDamage.enabled = true; }, fadeTime * 3);

            var colliderSize = Enemy.Bounds;
            //var i = 0;
            //do
            //{
            //    target = Enemy.Room.RandomPosition();
            //    i++;
            //}
            //while (Physics2D.OverlapBox(target, colliderSize.size, 0, World.Instance.PickupPlacementCheckMask) != null && i < 30);

            this.Delay(() =>
            {
                transform.position = target;
            }, fadeTime);
        }
    }
}
