using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Legend
{
    public class Activateable : MonoBehaviour
    {
        public bool IsActivatedOnContact;
        //public bool IsActivatableWhenClose = true;

        public string ActivationText { get; set; }

        NetworkIdentity net;

        void Awake()
        {
            net = GetComponent<NetworkIdentity>();
        }

        public virtual bool CanActivate(Player source)
        {
            return true;
        }

        public void Activate(Player source)
        {
            Unselect();
            DoActivate(source);
        }

        protected virtual void DoActivate(Player source)
        {
        }

        public virtual void Select(Player player)
        {
            if (player.Selected != null)
                player.Selected.Unselect();
            player.Selected = this;
            //HUD.Instance.SetCommandOption(InputAction.Activate, ActivationText);
        }

        public virtual void Unselect()
        {
            if (Player.Instance != null && Player.Instance.Selected == this)
            {
                Player.Instance.Selected = null;
                //HUD.Instance.ClearCommandOptions();
            }
        }

        void OnDestroy()
        {
            if (Player.Instance != null && Player.Instance.Selected == this)
            {
                Unselect();
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            CheckCollision(other.collider);
        }

        void OnCollisionStay2D(Collision2D other)
        {
            CheckCollision(other.collider);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            CheckCollision(other);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            CheckCollision(other);
        }

        public void CheckCollision(Collider2D other)
        {
            //print(name + " hit " + other.name);

            if (!IsActivatedOnContact || other.isTrigger)
                return;

            var target = other.GetComponentInChildren<Player>();
            if (target != null)
            {
                Activate(target);
            }
        }
    }
}