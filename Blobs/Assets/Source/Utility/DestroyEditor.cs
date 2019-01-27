using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class DestroyEditor : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_EDITOR
                Destroy(gameObject);
#else
                Destroy(this);
#endif
        }
    }
}