using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Legend
{
    [RequireComponent(typeof(Enemy))]
    public class ExplodeMove : Move
    {
        public GameObject ExplosionPrefab;
        public bool ExplodeOnDeath;
        //public bool DamageEnemies;

        void Start()
        {
            if (ExplodeOnDeath)
                Enemy.IsExplodeOnDeath = true;
        }

        public override void OnEnd()
        {
            //Explosion.Create(transform.position, Enemy.gameObject);

            Enemy.Health.Damage(Enemy.Health.Health, transform.position, Enemy.gameObject);
        }
    }
}