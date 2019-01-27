using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class Spin : MonoBehaviour
    {
        public Vector3 Amount;
        public bool SetRotationToIdentityOnDestroy;

        void Update()
        {
            transform.Rotate(Amount * Time.deltaTime, Space.Self);
        }

        void OnDestroy()
        {
            if (SetRotationToIdentityOnDestroy)
                transform.localRotation = Quaternion.identity;
        }
    }
}