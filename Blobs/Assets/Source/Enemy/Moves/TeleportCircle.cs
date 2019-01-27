using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Legend
{
    public class TeleportCircle : MonoBehaviour
    {
        public void Start()
        {
            name = "Teleport Circle";

            var destroy = GetComponent<Destroy>();
            destroy.enabled = false;

            var touch = GetComponent<TouchDamage>();
            touch.enabled = false;
            iTween.FadeFrom(gameObject, 0, 1.5f);
            touch.Delay(() =>
            {
                destroy.enabled = true;
                touch.enabled = true;
            }, 1.51f);
        }
    }
}