using Legend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public AudioListener Ears;
    public Transform FollowTarget;
    public Vector3 Offset = new Vector3(0, 0, -10);

    AudioListener cameraListener;
    Camera cam;

    void Awake()
    {
        Instance = this;

        cameraListener = GetComponent<AudioListener>();
        cameraListener.enabled = false;
        cam = GetComponent<Camera>();
        //Ears = cameraListener;
    }

    void Update()
    {
        transform.position = FollowTarget.position + Offset;

        if (Input.GetKey(KeyCode.Tab))
        {
            cam.orthographicSize = 50;
        }
        else
        {
            if (cam.orthographicSize == 50)
                Player.Instance.UpdateScale();
        }
    }
}
