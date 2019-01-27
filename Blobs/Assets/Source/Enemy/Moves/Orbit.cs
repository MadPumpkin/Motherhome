using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Legend
{
    public class Orbit : Move
    {
        public GameObject Prefab;
        public float OrbitRadius = 2.0f;
        public float OrbitSpeed = 45f;
        public int OrbitalCount = 8;
        public float OrbitalRadiusGrowthSpeed;
        public Transform Tip;

        float currentAngle;
        float orbitRadius;
        List<GameObject> orbitals = new List<GameObject>();

        public override void DoMove()
        {
            if (Tip == null)
                Tip = transform;

            orbitRadius = OrbitRadius;
            for (int i = 0; i < OrbitalCount; i++)
            {
                var orbital = Projectile.Create(Prefab, Vector3.zero, Quaternion.identity, Enemy.Health);
                orbitals.Add(orbital);
            }

            MoveUpdate();
        }

        public override void MoveUpdate()
        {
            orbitRadius += OrbitalRadiusGrowthSpeed * Time.deltaTime;
            var offsetVector = (Vector2.one * orbitRadius);
            currentAngle += OrbitSpeed * Time.deltaTime;
            var orbNumber = 0;
            foreach (var orbital in orbitals)
            {
                if (orbital != null)
                {
                    var angle = currentAngle + (360f / orbitals.Count) * orbNumber;
                    var rotation = Quaternion.Euler(0, 0, angle);
                    orbital.transform.position = Tip.position + (rotation * offsetVector);
                    orbital.transform.rotation = Quaternion.Euler(0, 0, angle + 30 + 180);
                }
                orbNumber++;
            }
        }

        public override void OnEnd()
        {
            foreach (var orbital in orbitals)
            {
                if (orbital != null)
                    Destroy(orbital);
            }
            orbitals.Clear();
        }

        void OnDie()
        {
            OnEnd();
        }
    }
}