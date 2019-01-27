using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlobMover : MonoBehaviour
{
    public float Speed = 3;

    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var moving = false;
        if (Input.GetKey(KeyCode.W))
        {
            moving = true;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            moving = true;
            transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moving = true;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moving = true;
            transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
        }

        if (moving)
        {
            rigid.AddRelativeForce(new Vector2(0, Speed));
        }
    }
}
