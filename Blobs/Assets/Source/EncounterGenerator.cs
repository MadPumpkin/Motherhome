using Legend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Encounter
{
    public string Name;

    public GameObject[] Enemies;
    public int EnemyCountMin = 1;
    public int EnemyCountMaxExclusive = 10;

    public GameObject[] Loot;
    public int LootCountMin = 1;
    public int LootCountMaxExclusive = 4;

    public int CosmeticCountMin = 1;
    public int CosmeticCountMaxExclusive = 4;

    public float MinDistance = 20;
    public float MaxDistance = 50;

    public float Radius = 10;

    public void Spawn()
    {
        var location = (UnityEngine.Random.insideUnitCircle * (MaxDistance - MinDistance))
            + (UnityEngine.Random.insideUnitCircle.normalized * MinDistance);

        var enemyCount = UnityEngine.Random.Range(EnemyCountMin, EnemyCountMaxExclusive);
        for (int i = 0; i < enemyCount; i++)
            GameObject.Instantiate(Enemies.RandomOrDefault(), location + UnityEngine.Random.insideUnitCircle * Radius, Quaternion.identity);

        var lootCount = UnityEngine.Random.Range(LootCountMin, LootCountMaxExclusive);
        for (int i = 0; i < lootCount; i++)
            GameObject.Instantiate(Loot.RandomOrDefault(), location + UnityEngine.Random.insideUnitCircle * Radius, Quaternion.identity);

        Debug.Log(Name + " spawned at " + location + " Enemy: " + enemyCount + " Loot: " + lootCount);
    }
}

public class EncounterGenerator : MonoBehaviour
{
    public Encounter[] RandomEncounters;
    public Encounter[] NonRandomEncounters;

    public int InitialCountMin = 5;
    public int InitialCountMaxExclusive = 10;

    public float RandomEncounterInterval = 30f;
    float nextRandomEncounter;

    void Start()
    {
        var encounterCount = UnityEngine.Random.Range(InitialCountMin, InitialCountMaxExclusive);
        for (int i = 0; i < encounterCount; i++)
            SpawnRandomEncounter();

        foreach (var encounter in NonRandomEncounters)
            encounter.Spawn();

        nextRandomEncounter = Time.time + RandomEncounterInterval;
    }

    public void Update()
    {
        if (Time.time > nextRandomEncounter)
        {
            SpawnRandomEncounter();
            nextRandomEncounter = Time.time + RandomEncounterInterval;
        }
    }

    public void SpawnRandomEncounter()
    {
        RandomEncounters.RandomOrDefault().Spawn();
    }
}
