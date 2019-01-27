using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Legend
{
    public enum SpawnLocation
    {
        AroundEnemy,
        RandomInRoom,
        RoomCorners,
        AroundTarget,
        AlongWalls
    }

    public class SpawnPrefab : Move
    {
        [Header("Spawning")]
        public GameObject ToBeSpawned;
        public GameObject[] ToBeSpawnedList;
        public int SpawnCount = 1;
        [Tooltip("Set range to 0 to pick random position in room.")]
        public float Range = 2;
        public float SpawnDelay = 1;
        public bool RequireEmptySpot;
        public float MinimumDistanceBetweenSpawns;
        public float DelayBetweenSpawns = 0.15f;
        public int SpawnLimit = 3;
        public SpawnLocation SpawnLocation;

        public Transform Tip;

        int currentSpawnCount;

        void Start()
        {
            if (Tip == null)
                Tip = transform;
        }

        List<Vector3> spawnedPositions = new List<Vector3>();
        float totalSpawnDelay;
        public override void DoMove()
        {
            //print("Spawn Move");
            this.Delay(() => DoSpawn(), SpawnDelay);
        }

        protected override bool CanStart()
        {
            return SpawnLimit <= 0 || currentSpawnCount < SpawnLimit;
        }

        void DoSpawn()
        {
            spawnedPositions.Clear();
            totalSpawnDelay = 0;

            var count = 0;
            for (int i = 0; i < SpawnCount; i++)
            {
                if (SpawnLimit > 0 && currentSpawnCount >= SpawnLimit)
                    return;

                while (!TrySpawn())
                {
                    count++;
                    if (count > 100)
                        return;
                }
            }

            //Debug.Break();
        }

        bool TrySpawn()
        {
            //print("TrySpawn " + SpawnLocation);

            var rotation = Quaternion.identity;
            var position = Vector3.zero;
            switch (SpawnLocation)
            {
                case SpawnLocation.RandomInRoom:
                    //var i = 0;
                    //do
                    //{
                    //    position = Enemy.Room.RandomPosition();
                    //    i++;
                    //}
                    //while (Physics2D.OverlapBox(position, new Vector3(0.5f, 0.5f), 0, World.Instance.PickupPlacementCheckMask) != null && i < 30);
                    break;
                case SpawnLocation.AroundEnemy:
                    //position = Range == 0 ?
                    //    Enemy.Room.RandomPosition() :
                    //    Tip.position + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * Range);
                    break;
                case SpawnLocation.RoomCorners:

                    //var corners = new List<Vector3>() {
                    //    Enemy.Room.transform.position + new Vector3(Enemy.Room.Bounds.extents.x - 0.5f, Enemy.Room.Bounds.extents.y - 0.5f),
                    //    Enemy.Room.transform.position + new Vector3(-Enemy.Room.Bounds.extents.x + 1.0f, Enemy.Room.Bounds.extents.y - 0.5f),
                    //    Enemy.Room.transform.position + new Vector3(-Enemy.Room.Bounds.extents.x + 1.0f, -Enemy.Room.Bounds.extents.y + 0.5f),
                    //    Enemy.Room.transform.position + new Vector3(Enemy.Room.Bounds.extents.x - 0.5f, -Enemy.Room.Bounds.extents.y + 0.5f),
                    //    };

                    //while (corners.Count > 0)
                    //{
                    //    position = corners.RandomOrDefault();

                    //    if (Utility.IsEmptySpot(position))
                    //        break;

                    //    corners.Remove(position);
                    //}

                    //if (corners.Count == 0)
                    //    return true;

                    //Debug.Break();

                    break;
                case SpawnLocation.AroundTarget:
                    if (Enemy.Target != null)
                        position = Enemy.Target.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * Range);
                    //else
                    //    position = Enemy.Room.RandomPosition();
                    break;

                case SpawnLocation.AlongWalls:

                    //if (Utility.Flip())
                    //{
                    //    position = Enemy.Room.transform.position + new Vector3(UnityEngine.Random.Range(Enemy.Room.Bounds.min.x, Enemy.Room.Bounds.max.x), Enemy.Room.Bounds.extents.y - 0.5f);
                    //    rotation = Quaternion.Euler(0, 0, 180);
                    //}
                    //else
                    //{
                    //    position = Enemy.Room.transform.position + new Vector3(UnityEngine.Random.Range(Enemy.Room.Bounds.min.x, Enemy.Room.Bounds.max.x), -Enemy.Room.Bounds.extents.y + 0.5f);
                    //}

                    break;
            }

            //Debug.DrawLine(transform.position, spawnLocation);

            if (RequireEmptySpot && !Utility.IsEmptySpot(position))
                return false;

            var minDistanceSquared = MinimumDistanceBetweenSpawns * MinimumDistanceBetweenSpawns;
            if (MinimumDistanceBetweenSpawns > 0 && spawnedPositions.Where((l) => (l - position).sqrMagnitude < minDistanceSquared).FirstOrDefault() != default(Vector3))
                return false;

            //print("Spawning at " + currentSpawnCount);
            spawnedPositions.Add(position);

            currentSpawnCount++;

            this.Delay(() =>
            {
                if (Enemy.Health.IsDead)
                    return;

                var prefab = ToBeSpawned;
                if (prefab == null)
                    prefab = ToBeSpawnedList.RandomOrDefault();

                var spawned = Instantiate(prefab, position, rotation, transform.parent);
                if (spawned != null)
                {
                    //spawned.AddComponent<SpawnMove>();

                    var enemy = spawned.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.OnDestroyEvent += () => currentSpawnCount--;
                        enemy.GetComponent<Damageable>().Stun();
                        //Enemy.Room.EnemyAdded(enemy);
                    }

                    //if (spawned.GetComponent<Bomb>() != null)
                    //    currentSpawnCount--;

                    NetworkServer.Spawn(spawned);
                }
            }, totalSpawnDelay);

            totalSpawnDelay += DelayBetweenSpawns;
            return true;
        }
    }
}