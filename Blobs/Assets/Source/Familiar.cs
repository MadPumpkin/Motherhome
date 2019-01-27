using UnityEngine;

namespace Legend
{
    public class Familiar : MonoBehaviour
    {
        public void OnDie()
        {
            Player.Instance.Familiars.Remove(this);
            Player.Instance.ReleaseFormationPoint(GetComponent<FollowTarget>().Target);
        }
    }
}