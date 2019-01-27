using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    [RequireComponent(typeof(Enemy))]
    public class Boss : MonoBehaviour
    {
        public AudioClip FightMusic;

        public int OnDefeatSendToLevel;

        [NonSerialized]
        public bool IsDead;

        void Start()
        {
            if (FightMusic != null)
                MusicManager.PlayTempMusic(FightMusic);
        }

        public void OnDie()
        {
            IsDead = true;

            MusicManager.StopTempMusic();

            if (Player.Instance != null)
                Player.Instance.Delay(() => Player.Instance.PlaySound(Level.Instance.BossDefeated, 0.15f, true), 1f);

            // if we want music volume instead
            //this.Delay(() => MusicManager.PlayTempMusic(Level.Instance.BossDefeated), 2f);

            HUD.Instance.ShowMessage(String.Format("{0} was defeated!"._(), name), Utility.Lime);

            var enemy = GetComponent<Enemy>();

            //foreach (var p in GameObject.FindObjectsOfType<Projectile>())
            //{
            //    if (p.Targets == TargetType.Players || p.Targets == TargetType.All)
            //    {
            //        if (enemy.Room.ContainsPoint(p.transform.position))
            //        {
            //            p.ExplodeOnCollision = false;
            //            Destroy(p.gameObject);
            //        }
            //    }
            //}

            //foreach (var t in GameObject.FindObjectsOfType<TouchDamage>())
            //{
            //    if (t.OnlyDamagePlayer && t.GetComponent<Boss>() == null)
            //    {
            //        if (enemy.Room.ContainsPoint(t.transform.position))
            //        {
            //            Destroy(t);
            //        }
            //    }
            //}

            //if (enemy.hasAuthority)
            //{
            //    foreach (var e in GameObject.FindObjectsOfType<Enemy>())
            //    {
            //        if (e != null && e != enemy && e.GetComponent<Boss>() == null && enemy.Room.ContainsPoint(e.transform.position))
            //        {
            //            foreach (var move in e.GetComponentsInChildren<Move>())
            //                move.StopAllCoroutines(); // stop spawning & shooting

            //            e.IsExplodeOnDeath = false;
            //            e.Health.Die(new Damage() { Type = DamageType.Unblockable });
            //            if (!e.Health.IsDestroyOnDie)
            //                e.Health.FadeAndDestroy();
            //        }
            //    }

            //    if (OnDefeatSendToLevel > 0)
            //    {
            //        enemy.Room.Delay(() =>
            //        {
            //            if (!Level.IsHeadlessMode)
            //                iTween.CameraFadeTo(1, 2.5f);
            //            Level.Instance.TransitionToLevel(OnDefeatSendToLevel);
            //        }, 8f);
            //    }
            //}
        }

        void OnDestroy()
        {
            MusicManager.StopTempMusic();
        }
    }
}