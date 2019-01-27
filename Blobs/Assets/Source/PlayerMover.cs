using Legend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationDirection
{
    None = 0,
    Left,
    Right
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    public float Speed = 3;
    public float DamagePerSecondWhileMove = 3;
    public ParticleSystem MoveParticles;

    Rigidbody2D body;
    bool moving;
    float horizontal, vertical;
    float speed = 20f;
    float rotationSpeed = 0.1f;
    float nearTargetRotationCoefficient = 0.5f;
    float angularDrag = 10f;

    Rigidbody2D rigid;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.angularDrag = angularDrag;
        body.mass = 2;
        MoveParticles.Stop();
    }

    void FixedUpdate()
    {
        if (Player.Instance.Health.IsDead)
        {
            MoveParticles.Stop();
            return;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        var targetVector = new Vector2(horizontal, vertical);
        var targetHeading = Vector2.SignedAngle(Vector2.up, targetVector);

        //print(targetVector + " " + targetHeading);

        moving = false;
        if (vertical != 0 || horizontal != 0)
            moving = true;

        bool accelerateSwiftly = false;

        if (moving)
        {
            if (!MoveParticles.isPlaying)
                MoveParticles.Play();

            if (DamagePerSecondWhileMove > 0)
                Player.Instance.Health.Damage(DamagePerSecondWhileMove * Time.deltaTime, Vector3.zero, gameObject, DamageType.Movement);

            accelerateSwiftly = TorqueRotate(targetHeading);
            if (accelerateSwiftly)
            {
                Accelerate(speed);
            }
            else
            {
                Accelerate(speed / 4);
            }
        }
        else
        {
            MoveParticles.Stop();
        }

        body.rotation = ClampRotation(body.rotation);
    }

    public RotationDirection GetRotateDirection(float targetRotation)
    {
        if (body.rotation < 0)
            body.rotation += 360;

        if (targetRotation < 0)
            targetRotation += 360;

        if (body.rotation == 360)
            body.rotation = 0;

        if (targetRotation == 360)
            targetRotation = 0;

        if (body.rotation == targetRotation)
            return RotationDirection.None;

        float angleLeft = (360 - body.rotation) + targetRotation;
        float angleRight = body.rotation - targetRotation;

        float rotationDifference = targetRotation - body.rotation;
        float inverseRotationDifference = (360 - targetRotation) + body.rotation;

        if (body.rotation < targetRotation)
        {
            if (targetRotation > 0)
            {
                angleLeft = rotationDifference;
                angleRight = inverseRotationDifference;
            }
            else
            {
                angleLeft = inverseRotationDifference;
                angleRight = rotationDifference;
            }
        }

        float angleRotation = ((angleLeft <= angleRight) ? angleLeft : (angleRight * -1));
        if (angleRotation == 0)
        {
            return RotationDirection.None;
        }
        else
        {
            return angleRotation >= 0 ? RotationDirection.Left : RotationDirection.Right;
        }
    }

    public float ClampRotation(float angle)
    {
        float result = angle - Mathf.CeilToInt(angle / 360f) * 360f;
        if (result < 0)
            result += 360f;
        return result;
    }

    public Vector2 RotationToVector(float degree)
    {
        return new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    }

    public bool TorqueRotate(float targetRotation)
    {
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, targetRotation) * Vector3.up);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, body.rotation) * Vector3.up, Color.blue);

        float angleDifference = Vector2.Angle(RotationToVector(body.rotation), RotationToVector(targetRotation));
        float rotationCoefficient = angleDifference * nearTargetRotationCoefficient;

        RotationDirection direction = GetRotateDirection(targetRotation);
        switch (direction)
        {
            case RotationDirection.Left:
                body.AddTorque(rotationSpeed * rotationCoefficient * Time.fixedDeltaTime, ForceMode2D.Impulse);
                break;
            case RotationDirection.Right:
                body.AddTorque(-rotationSpeed * rotationCoefficient * Time.fixedDeltaTime, ForceMode2D.Impulse);
                break;
        }

        return angleDifference < 45f;
    }

    public void Accelerate(float thrust)
    {
        body.AddForce(transform.up * thrust * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

}
