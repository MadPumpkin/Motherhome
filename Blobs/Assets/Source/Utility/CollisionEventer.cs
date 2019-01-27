using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public class CollisionEventer : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            DoCheckCollision(other.collider);
        }

        void OnCollisionStay2D(Collision2D other)
        {
            DoCheckCollision(other.collider);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            DoCheckCollision(other);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            DoCheckCollision(other);
        }

        void DoCheckCollision(Collider2D other)
        {
            this.SendMessageUpwards("CheckCollision", other);
        }
    }
}

