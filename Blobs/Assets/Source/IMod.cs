using System;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public interface IMod
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        int Version { get; }

        void OnLoad(string modLocation);
        void OnPlayerSpawn(Player player);
        void OnPickup(Pickup pickup);
        OnPlayerHitResult OnPlayerHit(Damage damage);
        void OnEnemyDamaged(Damageable damageable, GameObject source);
        void OnEnemyDie(Enemy enemy);
    }
}