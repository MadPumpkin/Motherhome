using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legend
{
    public class Destroy : MonoBehaviour
    {
        public bool IsEnabled = true;
        public float Delay;
        public bool DetachOnAwake;
        public float StopAllParticlesAt;
        public bool IsFadeOut;
        public float FadeOutLength = 0.5f;
        public bool IsFloatUp;
        public string SetObjectNameTo;

        void OnEnable()
        {
            if (IsEnabled)
            {
                if (StopAllParticlesAt != 0)
                {
                    this.Delay(() =>
                    {
                        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
                            particle.Stop();
                    }, StopAllParticlesAt);
                }

                if (IsFloatUp)
                    iTween.MoveTo(gameObject, transform.position + new Vector3(0, 0.5f, 0), Delay);

                if (IsFadeOut)
                {
                    this.Delay(() =>
                        iTween.FadeTo(gameObject, 0, FadeOutLength),
                        Delay - FadeOutLength);
                }

                Destroy(gameObject, Delay);
            }
            else
                Destroy(this);
        }

        void Start()
        {
            if (!String.IsNullOrEmpty(SetObjectNameTo))
                name = SetObjectNameTo;

            if (DetachOnAwake)
                transform.SetParent(null);
        }
    }
}