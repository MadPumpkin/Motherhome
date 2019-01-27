using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class DestroyNotEditor : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_EDITOR
                Destroy(this);
#else
                Destroy(gameObject);
#endif
        }
    }
}