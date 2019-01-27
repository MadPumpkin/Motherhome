using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Legend
{
    public class LeaveTrail : MonoBehaviour
    {

        //public string TrailOfObjects;
        //public float lerpValue;


        //EnemyMovement enemyMovement;
        //Vector3 spawningPosition;
        //float distance = 0.0f;
        //int segmentsToCreate;
        


        //// Use this for initialization
        //void Start()
        //{
        //    enemyMovement = GetComponentInParent<EnemyMovement>();
        //}

        //void Update()
        //{
        //    if(enemyMovement.CanTeleport == false)
        //    {
        //        InstantiateSegments();
        //    }
        //}

        //public void InstantiateSegments()
        //{
        //    segmentsToCreate = Mathf.RoundToInt(Vector3.Distance( enemyMovement.previousPosition, enemyMovement.nextPosition) / 0.5f);
        //    distance = 1 / segmentsToCreate;
        //    lerpValue = distance;

        //    for (int i = 0; i < segmentsToCreate; i++)
        //    {
        //        lerpValue += distance;

        //        spawningPosition = Vector3.Lerp(enemyMovement.previousPosition, enemyMovement.nextPosition, lerpValue);

        //        (PhotonNetwork.Instantiate(TrailOfObjects, spawningPosition, Quaternion.identity, 0) as GameObject).GetComponent<Projectile>();
        //    }
        //}
    }
}