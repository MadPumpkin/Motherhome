using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legend
{
    public class Vanish : Move
    {
        public override void DoMove()
        {
            base.DoMove();
            Enemy.LootCount = 0;
            Enemy.Health.OnDiePrefab = new GameObject[0];
            Enemy.Health.OnDieSound = new AudioClip[0];
            Enemy.DeathSounds = new AudioClip[0];
            Enemy.Health.Die(new Damage() { Source = Enemy.gameObject });
        }
    }
}