using Legend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public List<Pickup> Items = new List<Pickup>();
    public GameObject[] SpawnPrefabs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Trigger Splash Animation if Player enters this trigger
        if(collision.gameObject == Player.Instance.gameObject)
        {
            SpawnFamiliars();
            foreach (var familiar in Player.Instance.Familiars)
            {
                familiar.GetComponent<Damageable>().Respawn();
            }
            Player.Instance.HealFull();
            Player.Instance.Movement.Splash();
        }
    }

    public void SpawnFamiliars()
    {
        Items.AddRange(Player.Instance.Items);
        foreach (var item in Items)
        {
            Destroy(item.GetComponent<FollowTarget>());
            // todo: float towards center
        }
        Player.Instance.Items.Clear();

        while (Items.Count >= 2)
        {
            Spawn(Items[0].Type, Items[1].Type);
            Destroy(Items[0].gameObject);
            Destroy(Items[1].gameObject);
            Items.RemoveRange(0, 2);
        }
    }

    public void Spawn(PickupType first, PickupType second)
    {
        if ((int)first > (int)second)
        {
            var temp = first;
            first = second;
            second = temp;
        }

        var index = 0;
        switch (first)
        {
            case PickupType.Ring:
                switch (second)
                {
                    case PickupType.Ring:
                        index = 0;
                        break;
                    case PickupType.Star:
                        index = 1;
                        break;
                    case PickupType.Strawberry:
                        index = 2;                    
                        break;
                    case PickupType.Arrow:
                        index = 3;
                        break;
                }
                break;
            case PickupType.Star:
                switch (second)
                {
                    case PickupType.Star:
                        index = 4;
                        break;
                    case PickupType.Strawberry:
                        index = 5;
                        break;
                    case PickupType.Arrow:
                        index = 6;
                        break;
                }
                break;
            case PickupType.Strawberry:
                switch (second)
                {
                    case PickupType.Strawberry:
                        index = 7;
                        break;
                    case PickupType.Arrow:
                        index = 8;
                        break;
                }
                break;
            case PickupType.Arrow:
                index = 9;
                break;
        }

        print(first + " " + second + " " + index);

        var familiarObj = Instantiate(SpawnPrefabs[index], Player.Instance.transform.position, Player.Instance.transform.rotation);
        var familiar = familiarObj.GetComponent<Familiar>();
        Player.Instance.Familiars.Add(familiar);
        var follower = familiarObj.GetComponent<FollowTarget>();
        follower.Target = Player.Instance.GetAvailableFormationPoint();
    }
}
