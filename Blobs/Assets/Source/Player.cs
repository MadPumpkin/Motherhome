using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Text;
using Moments;

namespace Legend
{
    public class Player : Activateable
    {
        public static Player Instance;

        public Transform CarryPosition;
        public AudioClip[] ReviveSounds;
        public Transform[] FormationPoints;

        [NonSerialized]
        public PlayerVisual Visual;
        [NonSerialized]
        public Damageable Health;
        [NonSerialized]
        public PlayerMovement Movement;
        [NonSerialized]
        public Activateable Selected;
        [NonSerialized]
        public Carryable Carrying;
        [NonSerialized]
        public Weapon Weapon;
        [NonSerialized]
        public bool IsPlaying = true;
        [NonSerialized]
        public List<Pickup> Items = new List<Pickup>();
        [NonSerialized]
        public List<Familiar> Familiars = new List<Familiar>();

        void Awake()
        {
            Instance = this;

            Visual = GetComponentInChildren<PlayerVisual>();
            Movement = GetComponentInChildren<PlayerMovement>();
            Health = GetComponent<Damageable>();

            //print(CharacterClassType);

            Weapon = GetComponentInChildren<Weapon>();

            Respawn();
        }

        List<Transform> usedPoints = new List<Transform>();

        internal Transform GetAvailableFormationPoint()
        {
            foreach (var point in FormationPoints)
            {
                if (!usedPoints.Contains(point))
                {
                    usedPoints.Add(point);
                    return point;
                }
            }

            return null;
        }

        public void ReleaseFormationPoint(Transform point)
        {
            usedPoints.Remove(point);
        }

        public void Respawn()
        {
            print("Respawn");

            transform.position = Vector3.zero;

            IsPlaying = true;

            foreach (var item in Items)
            {
                Destroy(item.gameObject);
            }
            Items.Clear();

            if (Carrying != null)
            {
                Destroy(Carrying.gameObject);
                Carrying = null;
            }

            Health.Respawn();
            UpdateScale();

            foreach (var mod in Level.Mods)
            {
                mod.OnPlayerSpawn(this);
            }

            Visual.SetTempAnimation(CharacterAnimation.Happy, .75f, Direction.Down);

            this.EnableColliders();
            Movement.OnRespawn();
        }

        public void HealFull()
        {
            this.PlaySound(ReviveSounds);

            Health.Respawn();
            UpdateScale();
        }

        public void OnHit(Damage damage)
        {
            //if (isLocalPlayer)
            Visual.SetTemporaryAnimation(CharacterAnimation.Hurt, Health.InvulnerableTime);

            UpdateScale();
        }

        public void UpdateScale()
        {
            var scale = (Health.CurrentHealthAsPercentage * 1f) + 0.5f;
            transform.localScale = new Vector3(scale, scale, 1f);

            Camera.main.orthographicSize = scale * 5f;
        }

        public void OnDie(Damage damage)
        {
            //print("OnDie");

            //this.DisableColliders();
            //Movement.DisableControl();

            if (Carrying != null)
                Carrying.Drop();

            //if (damage.Source != null)
            //{
            //    HUD.Instance.ShowMessage(String.Format("{0} was knocked down by {1}."._(), name, damage.Source.name), Utility.Red);
            //}

            this.Delay(Respawn, 5f);

            //if (Inventory.Decrement("Coins", 5))
            //{
            //    var coin = PhotonNetwork.Instantiate("Placeables/Coin", transform.position, Quaternion.identity, 0);
            //    coin.IgnoreCollisionsWith(gameObject);
            //}

            //if (GameMatch.Instance != null && GameMatch.Instance.Contest != null)
            //{
            //    var sourcePlayer = damage.Source.GetComponent<Player>();
            //    if (!GameMatch.Instance.Contest.OnPlayerDeath(this, sourcePlayer))
            //        return; // don't respawn if told not to
            //}

        }

        internal void TeleportTo(Transform target)
        {
            TeleportTo(target.position.x, target.position.y);
        }

        internal void TeleportTo(float x, float y)
        {
            const float teleportLenth = .25f;
            iTween.CameraFadeAdd();
            iTween.CameraFadeTo(1, teleportLenth);
            this.Delay(() =>
            {
                transform.position = new Vector3(x, y, transform.position.z);
                iTween.CameraFadeTo(0, teleportLenth);
            }, teleportLenth);
        }

        public void Update()
        {
            //AttackRate++;
            //print(AttackRate + " " + TimeBetweenBullets);

            //HUD.Instance.ShowMessage("Update" + isLocalPlayer.ToString());

            //HUD.Instance.ShowMessage("PauseMenu.Instance.IsShown " + PauseMenu.Instance.IsShown);
            //print(PauseMenu.Instance.IsShown);
            //if (!PauseMenu.Instance.IsShown && Input != null)
            //{
            //    if (Input.GetButtonDown("Pause"))
            //        PauseMenu.Instance.Show();

                //HUD.Instance.ShowMessage("Health.IsDead " + Health.IsDead);

                if (!Health.IsDead)
                {
                    ProcessInputs();
                }

            //}
        }

        internal void CmdDamage(GameObject target, Damage damage)
        {
            if (target != null)
            {
                var damageable = target.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.Damage(damage);
                }
            }
        }

        internal static void PlayErrorSound()
        {
            if (Instance != null)
                Instance.PlaySound(Level.Instance.Error);
            else
                Level.Instance.PlaySound(Level.Instance.Error);
        }

        void ProcessInputs()
        {
            //HUD.Instance.ShowMessage("ProcessInputs " + hasAuthority.ToString());

            if (Carrying != null)
            {
                if (Input.GetButtonDown("Activate"))
                {
                    Throw(CarryPosition.position, Movement.Rigid.velocity, Direction.Down);
                }
                else
                {
                    if (Input.GetButtonDown("Place Bomb"))
                        Drop();
                }
            }
            else
            {
                var closest = GetClosestActivatable();
                if (closest != Selected)
                {
                    if (closest == null)
                        Selected.Unselect();
                    else
                        closest.Select(this);
                }

                if (Selected != null && Input.GetButtonDown("Activate"))
                {
                    Activate(Selected.gameObject);
                }
            }

            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    if (Settings.MusicVolume == 0)
            //        Settings.MusicVolume = 0.5f;
            //    else
            //        Settings.MusicVolume = 0;
            //}

            if (Input.GetButtonDown("Record GIF"))
                Record();
        }

        public void Record()
        {
            var recorder = GameObject.FindObjectOfType<Recorder>();
            recorder.SaveFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games"), Level.GameName);
            Directory.CreateDirectory(recorder.SaveFolder);
            recorder.Record();

            //AudioSource.PlayClipAtPoint(RecordSound, transform.position);
            print("recording");

            recorder.Delay(() =>
            {
                //AudioSource.PlayClipAtPoint(RecordSound, transform.position);
                print("saving");
                var filename = GenerateFileName();
                recorder.Save(filename);

                HUD.Instance.ShowMessage(String.Format("Saving Animated GIF to {0}", filename)._());
            }, 6f);
        }

        string GenerateFileName()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            return Legend.Level.GameName + " " + timestamp;
        }

        Activateable GetClosestActivatable()
        {
            Activateable result = null;
            var closestDistanceSqr = Mathf.Infinity;
            foreach (var collider in Physics2D.OverlapCircleAll(transform.position, 1.5f))
            {
                foreach (var activatable in collider.GetComponentsInChildren<Activateable>())
                {
                    if (activatable != null && activatable != this && !activatable.IsActivatedOnContact && activatable.CanActivate(this))
                    {
                        var offsetToTarget = activatable.transform.position - transform.position;
                        var dSqrToTarget = offsetToTarget.sqrMagnitude;
                        if (dSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = dSqrToTarget;
                            result = activatable;
                        }
                    }
                }
            }
            return result;
        }

        public void Activate(GameObject target)
        {
            if (target != null)
            {
                var activatable = target.GetComponent<Activateable>();
                activatable.Activate(this);
            }
        }

        public void Carry(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var carryable = gameObject.GetComponent<Carryable>();
                if (carryable != null)
                    carryable.Carry(this);
            }
        }

        public void Throw(Vector3 from, Vector3 velocity, Direction direction)
        {
            if (Carrying != null)
                Carrying.Throw(from, velocity, direction);
        }

        public void Drop()
        {
            if (Carrying != null)
                Carrying.Drop();
        }

        public override bool CanActivate(Player source)
        {
            return Health.IsDead;
        }

        protected override void DoActivate(Player source)
        {
            if (source.Health.IsDead || !Health.IsDead) // to avoid rez exploit
                return;

            source.Health.Damage(0.5f, transform.position, gameObject, DamageType.Unblockable);
            Health.Heal(0.5f);
            Health.Invulnerable(2);
        }
    }
}