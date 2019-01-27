using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public class DelayAnimation : MonoBehaviour
    {
        void Start()
        {
            var animator = GetComponentInChildren<Animator>();
            animator.Update(Random.value);
        }
    }
}