using System;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    [Serializable]
    public struct Damage
    {
        public float Amount;
        public DamageType Type;
        public GameObject Source;
        public Vector3 Position;
    }
}