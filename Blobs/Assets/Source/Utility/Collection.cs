using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public class Collection : Counter
    {
        public List<int> CollectedIds = new List<int>();

        public bool HasCollected(int id)
        {
            return CollectedIds.Contains(id);
        }

        public bool Collect(int id)
        {
            if (HasCollected(id))
                return false;

            CollectedIds.Add(id);
            Increment();
            return true;
        }

        public override bool Decrement(float amount)
        {
            return false;
        }

        public override CounterData Save()
        {
            var result = base.Save();
            result.CollectedIds = CollectedIds;
            return result;
        }
    }
}

