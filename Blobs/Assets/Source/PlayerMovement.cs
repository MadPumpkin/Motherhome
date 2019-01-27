using UnityEngine;
using System.Collections;
using System;
using Legend;

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
    None = 4
}

public enum CharacterAnimation
{
    Stand,
    CarryStand,
    Ready,
    Walk,
    CarryWalk,
    Hurt,
    Run,
    Nod,
    Damaged,
    Jump,
    ShakeHead,
    Swing,
    Fall,
    Wave,
    Shoot,
    Sit,
    Shocked,
    SpellChant,
    Sad,
    Happy,
    Dead,
}

public class PlayerMovement : MonoBehaviour
{
    [NonSerialized]
    internal bool MovementEnabled = true;
    internal bool MovementReallyEnabled = true;

    public bool CanMove { get { return MovementEnabled && MovementReallyEnabled /* && Menu.CurrentMenu == null */ /*&& HUD.Instance.DialogWindow.gameObject.activeSelf == false*/; } }
    public ParticleSystem splashParticle;

    float stunnedUntil;

    PlayerVisual visual;
    Weapon weapon;
    [NonSerialized]
    public Rigidbody2D Rigid;

    void Start()
    {
        visual = GetComponentInChildren<PlayerVisual>();
        weapon = GetComponentInChildren<Weapon>();
        Rigid = GetComponent<Rigidbody2D>();
    }

    public void Stun(float length = 2f)
    {
        stunnedUntil = Time.time + length;
        visual.SetTemporaryAnimation(CharacterAnimation.Damaged, length);
        //print(stunnedUntil);
    }

    internal void Knockback(Vector3 source, float amount = 100)
    {
        if (Rigid != null)
            Rigid.AddForce((transform.position - source).normalized * amount);
    }

    public void OnRespawn()
    {
        MovementEnabled = true;
    }

    public void OnDie(Damage damage)
    {
        visual.SetAnimation(CharacterAnimation.Dead);
    }

    public void Splash()
    {
        splashParticle.Play();
    }
}