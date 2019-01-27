using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public enum PickupType
    {
        Ring,
        Star,
        Strawberry,
        Arrow,
        Heart,
        Pellet
    }

    public class Pickup : Activateable
    {
        const float pickupDistance = 4f;
        const float pickupDelay = 1f;
        public const float MaxDropProbability = 0.8f;

        public AudioClip PickupSound;
        public GameObject SpawnOnPickup;

        public PickupType Type;

        public SpriteRenderer Icon;
        public int Price;
        public float AdditionalPricePerLevel;
        public bool IsShopItem;
        public bool IsAutoPickup = true;

        [NonSerialized]
        public GameObject ReservedPlayer;
        [NonSerialized]
        public Text PriceLabel;

        Text playerLabel;
        GameObject itemLabel;
        bool taken;
        float started = float.MaxValue;
        bool notReserved;

        public int ActualPrice(Player buyer)
        {
            var result = Price + (int)(AdditionalPricePerLevel * (Level.Instance.LevelNumber - 1));
            return result;
        }

        public static GameObject SpawnPickup(Transform parent, Vector3 position, bool excludeChests = true)
        {
            var prefab = Level.Instance.Pickups.RandomOrDefault();
            if (excludeChests)
            {
                while (prefab.name.Contains("Chest"))
                    prefab = Level.Instance.Pickups.RandomOrDefault();
            }

            //print("SpawnPickup " + prefab.name);
            return Spawn(parent, position, prefab, null);
        }

        public static GameObject Spawn(Transform parent, Vector3 position, GameObject prefab, GameObject reservedPlayer = null, bool notReserved = false)
        {
            if (prefab == null)
                prefab = Level.Instance.Pickups.RandomOrDefault();

            position = Utility.FindEmptySpot(position);

            var result = Instantiate(prefab, position, Quaternion.identity, parent);
            var pickup = result.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.ReservedPlayer = reservedPlayer;
                pickup.notReserved = notReserved;
            }

            return result;
        }

        public static float DropRateMultiplier()
        {
            var result = 3f;

            //if (GameServer.Instance.Mode == GameMode.Hard)
            //    result *= 0.5f;

            return result;
        }

        public void Start()
        {
            started = Time.time;

            this.IsActivatedOnContact = IsAutoPickup;

            if (Player.Instance != null && Player.Instance.IsPlaying)
                this.Delay(() => this.PlaySound(Level.Instance.ItemSpawned), 0.1f);
        }

        float priceLabelUpdateTime;

        void Update()
        {
            if (IsShopItem)
            {
                var carryable = GetComponent<Carryable>();
                if (carryable != null && carryable.enabled)
                    carryable.enabled = false;

                if (PriceLabel == null)
                {
                    itemLabel = this.Spawn(Level.Instance.PriceLabelPrefab, transform);
                    PriceLabel = itemLabel.GetComponentInChildren<Text>();
                }

                if (Player.Instance != null)
                {
                    if (Time.time > priceLabelUpdateTime)
                    {
                        priceLabelUpdateTime = Time.time + 5f;
                        PriceLabel.text = ActualPrice(Player.Instance).ToString();
                    }

                    //if (Player.Instance.Coins < ActualPrice(Player.Instance))
                    //    PriceLabel.color = Utility.DarkRed;
                    //else
                    //    PriceLabel.color = Utility.Green;
                }
            }
            else
            {
                //if (!notReserved && playerLabel == null)
                //{
                //    var label = this.Spawn(Level.Instance.PriceLabelPrefab, transform);
                //    label.transform.SetParent(transform);
                //    playerLabel = label.GetComponentInChildren<Text>();

                //    if (ReservedPlayer == null)
                //    {
                //        playerLabel.text = "?";
                //    }
                //    else
                //    {
                //        playerLabel.text = ReservedPlayer.name;
                //    }
                //}

                //if (IsAutoPickup && started < Time.time - pickupDelay)
                //{
                //    foreach (var hand in Player.Instance.Hands)
                //    {
                //        if (hand != null && CanCollect(hand) && (hand.transform.position - transform.position).sqrMagnitude < pickupDistance * pickupDistance && Utility.HasLineOfSight(hand.transform.position, transform))
                //        {
                //            //if (!Utility.HasLineOfSight(hand.transform.position, transform))
                //            //    Debug.Break();
                //            DoPickup(hand);
                //            break;
                //        }
                //    }
                //}
            }
        }

        protected override void DoActivate(Player source)
        {
            //print("DoActivate");
            if (CanCollect(source))
                DoPickup(source.gameObject);
        }

        // Not needed due to parenting
        //void OnDestroy()
        //{
        //    if (itemLabel != null)
        //        Destroy(itemLabel);
        //}

        public void DoPickup(GameObject source)
        {
            if (!taken)
            {
                taken = true;

                this.DisableColliders();

                ApplyItem(source);
            }
        }

        bool CanCollect(Player source)
        {
            if (started + pickupDelay > Time.time)
                return false;

            //if (IsShopItem && source.Coins < ActualPrice(source))
            //    return false;

            switch (Type)
            {
                case PickupType.Heart:
                    return source.Health.Health < source.Health.MaxHealth;
                default:
                    return true;
            }
        }

        public void ApplyItem(GameObject targetObject)
        {
            if (targetObject == null)
                return;

            var target = targetObject.GetComponent<Player>();

            if (SpawnOnPickup != null)
                Instantiate(SpawnOnPickup, transform.position, transform.rotation);

            if (IsShopItem)
            {
                AudioSource.PlayClipAtPoint(Level.Instance.BuyItem, target.transform.position);
                //target.Coins -= ActualPrice(target);
                Destroy(itemLabel);
            }

            foreach (var mod in Level.Mods)
            {
                mod.OnPickup(this);
            }

            switch (Type)
            {
                case PickupType.Heart:
                    target.Health.Heal(1);
                    Destroy(gameObject);
                    break;
                case PickupType.Pellet:
                    target.Health.Heal(0.1f);
                    Destroy(gameObject);
                    break;
                default:
                    var follow = gameObject.AddComponent<FollowTarget>();

                    if (Player.Instance.Items.Count > 0)
                        follow.Target = Player.Instance.Items.Last().transform;
                    else
                        follow.Target = Player.Instance.transform;
                    Player.Instance.Items.Add(this);
                    enabled = false;
                    break;
            }

            if (PickupSound != null)
                AudioSource.PlayClipAtPoint(PickupSound, transform.position);
        }
    }
}