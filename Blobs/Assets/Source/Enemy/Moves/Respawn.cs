using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legend
{
    public class Respawn : Move
    {
        public float RespawnTime = 3f;
        public float DeadHealth = 2f;
        public AudioClip[] OnReallyDieSound;

        bool isActive;
        float currentDeadHealth;

        public void Start()
        {
            Enemy.Health.TakesDamageEvenWhenDead = true;
        }

        public override void DoMove()
        {
            if (isActive)
                return;

            //print("DoMove");

            base.DoMove();

            currentDeadHealth = DeadHealth;
            isActive = true;

            //this.EnableColliders(false);

            this.Delay(() =>
            {
                if (isActive)
                {
                    Enemy.Respawn();
                    isActive = false;
                }
            }, RespawnTime);
        }

        public void OnHit(Damage damage)
        {
            //print("OnHit for " + damage.Amount);
            if (isActive)
            {
                currentDeadHealth -= damage.Amount;
                //print(currentDeadHealth);
                if (currentDeadHealth <= 0)
                {
                    isActive = false;
                    this.PlaySound(OnReallyDieSound);
                    Enemy.Health.FadeAndDestroy();
                }
            }
        }
    }
}