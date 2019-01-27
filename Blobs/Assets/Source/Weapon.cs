using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Legend
{
    public enum AimingMode
    {
        Orthogonal,
        Mouse,
        Forward
    }

    public class Weapon : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public Vector3 ProjectileOffset = new Vector3(0, 0.5f, 0);
        //public float ProjectileDelay = 0.1f;

        //public int ClipSize = 8;
        //public float ReloadDelay = 1f;
        //public AudioClip ReloadSound;
        //public float ReloadSoundVolume = 1;
        //public int CurrentClipCount;
        public AimingMode Mode;

        public float DamageAmount = 0.48f;
        public float TimeBetweenBullets = 0.26f;
        public float ImpulseForce = 4.5f;
        public float Range = 20f;
        public float Homing;

        public AudioClip FireSound;
        public float FireSoundVolume = 0.8f;
        public float FireSoundVolumeRange = 0.4f;
        public float FireSoundPitch = 0.8f;
        public float FireSoundPitchRange = 0.4f;

        [NonSerialized]
        public bool IsShooting;

        float nextShotTime;
        PlayerVisual visual;
        Rigidbody2D rigid;
        Damageable health;

        void Start()
        {
            visual = GetComponentInChildren<PlayerVisual>();
            rigid = GetComponent<Rigidbody2D>();
            health = GetComponent<Damageable>();
            //CurrentClipCount = ClipSize;
        }

        public void Update()
        {
            if (Player.Instance.Health.IsDead)
                return;

            //if (Input.GetKeyDown(KeyCode.R) && CurrentClipCount != ClipSize)
            //{
            //    Reload();
            //    return false;
            //}

            IsShooting = false;

            var horizontalAxis = Input.GetAxis("Attack Horizontal");
            var verticalAxis = Input.GetAxis("Attack Vertical");

            switch (Mode)
            {
                case AimingMode.Orthogonal:
                    //print(horizontalAxis);
                    //print(verticalAxis);
                    if (horizontalAxis < 0f)
                    {
                        IsShooting = true;
                        if (Time.time > nextShotTime)
                            Shoot(Direction.Left);
                    }
                    if (horizontalAxis > 0f)
                    {
                        IsShooting = true;
                        if (Time.time > nextShotTime)
                            Shoot(Direction.Right);
                    }
                    if (verticalAxis > 0f)
                    {
                        IsShooting = true;
                        if (Time.time > nextShotTime)
                            Shoot(Direction.Up);
                    }
                    if (verticalAxis < 0f)
                    {
                        IsShooting = true;
                        if (Time.time > nextShotTime)
                            Shoot(Direction.Down);
                    }
                    break;
                case AimingMode.Mouse:
                    if (Input.GetButton("Fire1") && Time.time > nextShotTime)
                    {
                        IsShooting = true;
                        var pos = Input.mousePosition;
                        pos.z = transform.position.z - Camera.main.transform.position.z;
                        pos = Camera.main.ScreenToWorldPoint(pos);
                        var rotation = Quaternion.FromToRotation(Vector3.up, pos - transform.position);
                        Shoot(transform.position, rotation);
                        //visual.SetTemporaryAnimation(CharacterAnimation.Shoot, 0.25f);
                    }

                    if ((horizontalAxis != 0 || verticalAxis != 0) && Time.time > nextShotTime)
                    {
                        var rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(horizontalAxis, verticalAxis));
                        Shoot(transform.position, rotation);
                    }
                    break;
                case AimingMode.Forward:
                    if (Input.GetButton("Fire1") && Time.time > nextShotTime)
                    {
                        IsShooting = true;
                        Shoot(transform.position, transform.rotation);
                        visual.SetTemporaryAnimation(CharacterAnimation.Shoot, 0.25f);
                    }
                    break;
            }
        }

        public static Quaternion DirectionRotation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Quaternion.identity;
                case Direction.Left:
                    return Quaternion.Euler(0, 0, 90);
                case Direction.Down:
                    return Quaternion.Euler(0, 0, 180);
                case Direction.Right:
                    return Quaternion.Euler(0, 0, 270);
                default:
                    throw new InvalidOperationException(direction + " can not be rotated");
            }
        }

        public void Shoot(Direction direction)
        {
            //if (player.Carrying != null)
            //{
            //    visual.SetTemporaryAnimation(CharacterAnimation.Shoot, 0.25f);
            //    player.Throw(player.CarryPosition.position, player.Movement.Rigid.velocity, direction);
            //    return;
            //}

            //if (visual == null)
            //    visual = GetComponentInChildren<PlayerVisual>();

            //visual.SetTemporaryAnimation(CharacterAnimation.Shoot, 0.25f);

            var position = Vector3.zero;
            switch (direction)
            {
                case Direction.Left:
                    position = transform.position + ProjectileOffset;
                    break;
                case Direction.Right:
                    position = transform.position + ProjectileOffset;
                    break;
                case Direction.Up:
                    position = transform.position + ProjectileOffset;
                    break;
                case Direction.Down:
                    position = transform.position + ProjectileOffset;
                    break;
            }

            Shoot(position, DirectionRotation(direction));
        }

        void Shoot(Vector3 position, Quaternion rotation)
        {
            //CurrentClipCount--;
            nextShotTime = Time.time + TimeBetweenBullets;

            var damage = DamageAmount;
            damage *= 1f - (UnityEngine.Random.value * 0.25f);

            if (ProjectilePrefab != null) // null check here for connecting while someone is shooting
            {
                if (rigid == null)
                    rigid = GetComponent<Rigidbody2D>();

                PlayClipAt(FireSound, transform.position, FireSoundVolume + FireSoundVolumeRange * UnityEngine.Random.value, FireSoundPitch + FireSoundPitchRange * UnityEngine.Random.value);

                var projectileObject = Projectile.Create(ProjectilePrefab, position, rotation, health, damage, ImpulseForce, Range, rigid == null ? Vector2.zero : (this.rigid.velocity * 0.5f));

                //projectileObject.GetComponent<Projectile>().Pierce = true;

                projectileObject.GetComponent<Projectile>().Homing += Homing;
            }
        }

        AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volume = 1f, float pitch = 1f)
        {
            if (clip == null)
                return null;

            var temp = new GameObject("TempAudio");
            temp.transform.position = pos;
            var source = temp.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            Destroy(temp, clip.length);
            return source;
        }

        //bool isReloading;

        //public void Reload()
        //{
        //    if (isReloading)
        //        return;

        //    isReloading = true;
        //    nextShotTime = Time.time + ReloadDelay;

        //    Player.Instance.Visual.SetTemporaryAnimation(CharacterAnimation.SpellChant, ReloadDelay / 2);
        //    this.PlaySound(ReloadSound, ReloadSoundVolume);

        //    this.Delay(() =>
        //    {
        //        CurrentClipCount = ClipSize;
        //        isReloading = false;
        //    }, ReloadDelay);
        //}
    }
}