using UnityEngine;

namespace Legend
{
    public class SpawnFourPrefabsAndSetUniqueDirectionsOnThemMove : Move
    {
        [Header("Spawning")]
        public GameObject ToBeSpawned;
        public float Range = 2;
		
        public override void DoMove()
        {
            //var spawned = TrySpawn(transform.position + new Vector3(Range, 0, Range));
            //if (spawned != null)
            //    spawned.GetComponent<Enemy>().MoveDirection = new Vector3(1, 0, 1);

            //spawned = TrySpawn(transform.position + new Vector3(-Range, 0, Range));
            //if (spawned != null)
            //    spawned.GetComponent<Enemy>().MoveDirection = new Vector3(-1, 0, 1);

            //spawned = TrySpawn(transform.position + new Vector3(Range, 0, -Range));
            //if (spawned != null)
            //    spawned.GetComponent<Enemy>().MoveDirection = new Vector3(1, 0, -1);

            //spawned = TrySpawn(transform.position + new Vector3(-Range, 0, -Range));
            //if (spawned != null)
            //    spawned.GetComponent<Enemy>().MoveDirection = new Vector3(-1, 0, -1);
        }

        GameObject TrySpawn(Vector3 position)
        {
            if (!Physics.CheckSphere(position, 0.2f))
            {
                var spawned = Instantiate(ToBeSpawned, position, transform.rotation);
                spawned.transform.SetParent(transform.parent);
                return spawned;
            }
            return null;
        }
    }
}