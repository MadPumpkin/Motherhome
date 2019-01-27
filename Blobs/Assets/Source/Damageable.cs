using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public class Damageable : MonoBehaviour
    {
        public static List<Damageable> Damageables = new List<Damageable>();

        public float MaxHealth = 10f;

        [Tooltip("Set to less than 0 to not be used (IE for player)")]
        public float InitialHealth = -1f;
        public float AdditionalHealthPerLevel;
        public float RegenPerSecond;
        public bool IsDefaultInvulnerable;
        public bool IsImmuneToTouchDamage;
        public bool IsImmuneToExplosionDamage;
        public bool IsPlayer;
        public Damageable PassThruTo;
        public float InvulnerableTime;
        public string InvulnerableLayer;

        [Header("Shield")]
        public float MaxShield = 0f;
        public float ShieldRegenPerSecond;
        public float ShieldCooldownBeforeRegen = 10;

        [Header("Events")]
        public AudioClip[] OnSpawnSound;
        public AudioClip[] OnHitSound;
        public GameObject OnHitPrefab;
        public GameObject[] OnHitPrefabAtLocalPosition;
        public AudioClip[] OnDieSound;
        public GameObject[] OnDiePrefab;
        public AudioClip[] OnInvulnerableHitSound;
        public bool IsDestroyOnDie;
        public float DestroyOnDieDelay = 0.25f;

        [Header("GUI")]
        public Slider Slider;
        public Text HealthLabel;
        public Text MaxHealthLabel;

        public Image BarImage;
        public Image BarImageBackground;

        public Sprite[] HeartSprites;
        public GridLayoutGroup Hearts;

        [NonSerialized]
        public float DamageTaken = 0;
        [NonSerialized]
        public bool TakesDamageEvenWhenDead;

        public bool IsDead { get { return Health <= 0; } }

        float health = 1f;
        public float Health
        {
            get { return health; }
            set
            {
                health = value;
                UpdateVisual();
            }
        }

        float shield = 0f;
        public float Shield
        {
            get { return shield; }
            set
            {
                shield = value;
                //Debug.Log("Player shield set to " + value);
                UpdateVisual();
            }
        }

        public float CurrentHealthAsPercentage { get { return health / MaxHealth; } }

        [NonSerialized]
        public GameObject LastDamageSource;

        bool isStarted;
        float lastDamageTime;
        int invulnerableLayer;
        int originalLayer;

        void Awake()
        {
            originalLayer = gameObject.layer;

            if (String.IsNullOrEmpty(InvulnerableLayer))
                invulnerableLayer = originalLayer;
            else
                invulnerableLayer = LayerMask.NameToLayer(InvulnerableLayer);

            // Do not initalize to full health here or players will recieve full health on level transition
            //Health = MaxHealth;

            if (InitialHealth > 0)
                Health = InitialHealth;

            Damageables.Add(this);

            isStarted = true;
            lastDamageTime = Time.time;
            if (BarImage != null)
            {
                initialBarImageWidth = BarImage.rectTransform.sizeDelta.x;
            }
        }

        void OnDestroy()
        {
            Damageables.Remove(this);
        }

        float lastMaxHealth;

        void Update()
        {
            if (RegenPerSecond > 0 && Health < MaxHealth && !IsDead)
                Heal(MaxHealth * RegenPerSecond * Time.deltaTime);

            if (ShieldRegenPerSecond > 0 && Shield < MaxShield && !IsDead && lastDamageTime < Time.time - ShieldCooldownBeforeRegen)
                AddShield(MaxShield * ShieldRegenPerSecond * Time.deltaTime);

            if (lastMaxHealth != MaxHealth)
            {
                lastMaxHealth = MaxHealth;
                UpdateVisual();
            }
        }

        float initialBarImageWidth = 100f;

        void UpdateSlider(float value)
        {
            Slider.value = value;
        }

        public void UpdateVisual()
        {
            if (Slider != null)
            {
                iTween.ValueTo(Slider.gameObject, iTween.Hash(
                     "from", Slider.value,
                     "to", CurrentHealthAsPercentage,
                     "time", 0.5f,
                     "onupdatetarget", gameObject,
                     "onupdate", "UpdateSlider",
                     "easetype", iTween.EaseType.easeOutQuad,
                     "ignoretimescale", true
                     )
                 );
                if (HealthLabel != null)
                    HealthLabel.GetComponent<Spinner>().SpinTo((long)Health, -1);
                if (MaxHealthLabel != null)
                    MaxHealthLabel.text = MaxHealth.ToString("N0");
            }

            if (BarImage != null)
            {
                var currentHealthPercentage = CurrentHealthAsPercentage;
                if (currentHealthPercentage >= 1)
                {
                    BarImage.enabled = false;
                    BarImageBackground.enabled = false;
                }
                else
                {
                    BarImage.enabled = true;
                    BarImageBackground.enabled = true;
                    BarImage.rectTransform.sizeDelta = new Vector2(initialBarImageWidth * CurrentHealthAsPercentage, BarImage.rectTransform.sizeDelta.y);
                }
            }

            if (Hearts != null)
            {
                var count = 0;
                var shieldCount = 0;
                foreach (Transform heart in Hearts.transform)
                {
                    count++;
                    var image = heart.GetComponent<Image>();
                    if (count > MaxHealth)
                    {
                        if (shieldCount < Shield)
                        {
                            shieldCount++;
                            image.enabled = true;
                            if (Shield >= shieldCount)
                            {
                                image.sprite = HeartSprites[5];
                            }
                            else if (Shield > shieldCount - 1)
                            {
                                image.sprite = HeartSprites[4];
                            }
                            else
                            {
                                image.enabled = false;
                            }
                        }
                        else
                            image.enabled = false;
                    }
                    else
                    {
                        image.enabled = true;

                        if (health >= count)
                        {
                            image.sprite = HeartSprites[2];
                        }
                        else if (health > count - 1)
                        {
                            image.sprite = HeartSprites[1];
                        }
                        else
                        {
                            image.sprite = HeartSprites[0];
                        }
                    }
                }
            }
        }

        public void Respawn()
        {
            if (AdditionalHealthPerLevel > 0)
                MaxHealth += AdditionalHealthPerLevel * Level.Instance.LevelNumber;

            Health = MaxHealth;
            Shield = 0;
            DamageTaken = 0;
            this.PlaySound(OnSpawnSound);
        }

        public void Heal(float amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);

            //HUD.Instance.ClearCommandOptions();
        }

        public void AddShield(float amount = 1f)
        {
            Shield = Math.Min(MaxShield, amount + Shield);
        }

        public bool IsInvulnerable { get { return Time.time < invulnerableUntil; } }
        double invulnerableUntil;

        public void Invulnerable(float amount)
        {
            //print("Invulnerable");
            if (amount > 0)
            {
                gameObject.layer = invulnerableLayer;

                this.Delay(() => { gameObject.layer = originalLayer; }, amount);

                invulnerableUntil = Time.time + amount;
                if (!isBlinking)
                    BroadcastMessage("Blink", amount, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                invulnerableUntil = Time.time;
            }

            //print(invulnerableUntil);
        }

        float vulnerableUntil;

        public void Vulnerable(float amount)
        {
            if (amount > 0)
            {
                vulnerableUntil = Time.time + amount;
            }
            else
            {
                vulnerableUntil = 0;
            }
        }

        public bool CanDamage(Damageable target)
        {
            if (target == this)
                return false;

            if (target.IsPlayer != IsPlayer)
                return true;

            return IsPlayer;
        }

        public void Damage(float amount, Vector3 position, GameObject source, DamageType type = DamageType.Projectile)
        {
            Damage(new Damage() { Amount = amount, Position = position, Source = source, Type = type });
        }

        //[Command] public void CmdExplode(Vector3 position, float damage) { RpcExplode(position, damage); }
        //[ClientRpc]
        //void RpcExplode(Vector3 position, float damage)
        //{
        //    Explosion.Create(position, gameObject, damage);
        //}

        public void Damage(Damage damage)
        {
            //print("damage");

            if (IsImmuneToExplosionDamage && damage.Type == DamageType.Explosion)
                return;

            if (IsImmuneToTouchDamage && damage.Type == DamageType.Physical)
                return;

            if (PassThruTo != null)
            {
                PassThruTo.Damage(damage);
            }
            else
            {
                if ((!IsDead || TakesDamageEvenWhenDead) && isStarted)
                {
                    if (damage.Amount <= 0)
                        return;

                    //print(IsInvulnerable);
                    //print(invulnerableUntil);

                    if (damage.Type != DamageType.Unblockable && (IsInvulnerable || (IsDefaultInvulnerable && Time.time > vulnerableUntil)))
                    {
                        //print(name + " damage avoided because invln");
                        return;
                    }

                    if (IsPlayer)
                    {
                        switch (damage.Type)
                        {
                            case DamageType.Unblockable:
                            case DamageType.Movement:
                                break;
                            //case DamageType.Explosion:
                            case DamageType.Spikes:
                                damage.Amount = 1;
                                break;
                            default:
                                //if (GameServer.Instance.Mode == GameMode.Hard)
                                //    damage.Amount = 1f;
                                //else
                                damage.Amount = 0.5f;
                                break;
                        }

                        // TODO: RPC from here on
                        var player = GetComponent<Player>();

                        foreach (var mod in Level.Mods)
                        {
                            if (mod.OnPlayerHit(damage) == OnPlayerHitResult.IgnoreDamage && damage.Type != DamageType.Unblockable)
                                return;
                        }
                    }

                    RpcDamage(damage);

                    //print("Health " + Health);
                }
            }
        }

        void RpcDamage(Damage damage)
        {
            if (damage.Type != DamageType.Movement)
            {
                if (damage.Type != DamageType.Unblockable && (IsInvulnerable || (IsDefaultInvulnerable && Time.time > vulnerableUntil)))
                {
                    //print(name + " damage avoided because invln");
                    if (OnInvulnerableHitSound != null && OnInvulnerableHitSound.Length > 0)
                    {
                        this.PlaySound(OnInvulnerableHitSound);
                    }
                    else
                    {
                        if (!IsPlayer)
                            Player.PlayErrorSound();
                    }
                    return;
                }

                if (IsPlayer)
                {
                    var player = GetComponent<Player>();
                    if (player != null)
                        player.Movement.Knockback(damage.Position);
                }

                this.Spawn(OnHitPrefab, transform.position, transform);

                foreach (var prefab in OnHitPrefabAtLocalPosition)
                {
                    this.Spawn(prefab, damage.Position, transform.parent);
                }

                this.PlaySound(OnHitSound);
                HitBlink();

#if !UNITY_EDITOR
            if (Player.Instance != null && damage.Source == Player.Instance.gameObject)
#endif
                //CombatText.Add(damage.Amount.ToString("N0"), Color.white, damage.Position);
            }

            DamageTaken += damage.Amount;
            LastDamageSource = damage.Source;
            lastDamageTime = Time.time;
            BroadcastMessage("OnHit", damage, SendMessageOptions.DontRequireReceiver);

            if (!IsDead) // this is here so that the colliders don't get disabled again after shooting a downed skeleton.
            {
                //print("hit: " + name + " for " + amount);
                var amount = damage.Amount;

                if (Shield > 0)
                {
                    var shieldAmount = Math.Min(Shield, amount);
                    Shield -= shieldAmount;
                    amount -= shieldAmount;
                }

                if (amount > 0)
                {
                    Health -= amount;
                }

                if (IsDead)
                    Die(damage);
                else
                    Invulnerable(InvulnerableTime);

                if (damage.Source != null)
                {
                    var playerSource = damage.Source.GetComponent<Player>();
                    //if (playerSource != null)
                    //    playerSource.DamageDealt(gameObject, damage.Amount);
                }
            }
        }

        bool isBlinking;

        public void HitBlink()
        {
            if (IsPlayer || !CanBlink)
                return;

            float amount = 0.25f;
            if (InvulnerableTime != 0)
                amount = InvulnerableTime;

            if (!isBlinking)
            {
                //print("HitBlink " + visuals.Count().ToString());
                isBlinking = true;
                blinkingCoroutine = StartCoroutine(DoBlink(amount));
            }
        }

        Coroutine blinkingCoroutine;
        Renderer[] validVisuals;
        Material[] originalMaterials;

        public bool CanBlink = true;

        IEnumerator DoBlink(float amount = 1)
        {
            //print(name + " DoBlink started for " + amount);

            var visuals = GetComponentsInChildren<SpriteRenderer>();

            var blinkUntil = Time.time + amount;
            validVisuals = (from v in visuals where v != null && v.sharedMaterial != null select v).ToArray();
            originalMaterials = (from v in validVisuals select v.sharedMaterial).ToArray();
            //if (visuals.Count() == 10)
            //    print("DoBlink " + validVisuals.Count().ToString());
            while (Time.time < blinkUntil)
            {
                foreach (var visual in validVisuals)
                {
                    if (visual != null) // check this here since the object may get destroyed mid blink
                    {
                        visual.sharedMaterial = World.Instance.RedHitMaterial;
                    }
                }
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < validVisuals.Length; i++)
                {
                    if (originalMaterials[i] != null && validVisuals[i] != null) // check this here since the object may get destroyed mid blink
                    {
                        validVisuals[i].sharedMaterial = originalMaterials[i];
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            //print("done");

            isBlinking = false;
            blinkingCoroutine = null;
        }

        public void StopBlinking()
        {
            if (isBlinking)
            {
                if (blinkingCoroutine != null)
                {
                    StopCoroutine(blinkingCoroutine);
                    blinkingCoroutine = null;
                }

                for (int i = 0; i < validVisuals.Length; i++)
                {
                    if (originalMaterials[i] != null && validVisuals[i] != null) // To fix: The object of type 'SpriteRenderer' has been destroyed but you are still trying to access it.
                        validVisuals[i].sharedMaterial = originalMaterials[i];
                }

                isBlinking = false;
            }
        }

        public void Stun(float length = 2f)
        {
            if (this.IsPlayer)
            {
                var player = GetComponent<Player>();
                if (player != null)
                    player.Movement.Stun(length);
            }
            else
            {
                var enemy = GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Movement.Stun(length);
            }
        }

        public void Fear(float length = 2f)
        {
            if (this.IsPlayer)
            {
                //Nothing yet
            }
            else
            {
                var enemy = GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Movement.Fear(length);
            }
        }

        public void Die(Damage damage)
        {
            Health = 0f;
            BroadcastMessage("OnDie", damage, SendMessageOptions.DontRequireReceiver);

            this.PlaySound(OnDieSound);

            foreach (var prefab in OnDiePrefab)
            {
                if (prefab != null)
                    Instantiate(prefab, transform.position, transform.rotation, transform.parent);
            }

            if (IsDestroyOnDie)
                FadeAndDestroy();
        }

        public void FadeAndDestroy()
        {
            if (DestroyOnDieDelay > 0)
            {
                StopBlinking();
                iTween.FadeTo(gameObject, 0, DestroyOnDieDelay);
            }

            Destroy(gameObject, DestroyOnDieDelay);
        }

        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    if (stream.isWriting)
        //    {
        //        // We own this player: send the others our data
        //        stream.SendNext(this.Health);
        //        stream.SendNext(this.MaxHealth);
        //    }
        //    else
        //    {
        //        // Network player, receive data
        //        this.Health = (float)stream.ReceiveNext();
        //        this.MaxHealth = (float)stream.ReceiveNext();
        //    }
        //}

        internal bool IsType(TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.All:
                    return true;
                case TargetType.Enemies:
                    return !IsPlayer;
                case TargetType.Players:
                    return IsPlayer;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return String.Format("{0:N0}/{1}", Health, MaxHealth);
        }
    }
}