using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMode : MonoBehaviour
{
    public static Transform PlayerTransform { get; private set; }
    public static bool isShootingRight { get; private set; }
    [SerializeField, Range(0,90)] private float pitchLimit;
    [SerializeField, Min(0)] private float pitchSpeed;
    [SerializeField, Range(0,180)] private float yawLimit;
    [SerializeField, Min(0)] private float yawSpeed;

    private void Awake()
    {
        if (PlayerTransform != null && PlayerTransform != transform)
        {
            Destroy(gameObject);
            return;
        }

        PlayerTransform = transform;
    }

    internal void SetLeftShoot(bool v)
    {
        
    }

    internal void SetRightShoot(bool v)
    {
        isShootingRight = v;
    }

    // Start is called before the first frame update
    void Start()
    {
        ControlHandler.InitTurret(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLookDirection(Vector2 readValue)
    {
        Vector3 rotation = transform.localEulerAngles;
        
        float pitch = rotation.x + readValue.y * yawSpeed;
        float yaw = rotation.y + readValue.x * pitchSpeed;
        
        if (pitch > pitchLimit && pitch < 180) pitch = pitchLimit;
        else if (pitch < 360 - pitchLimit && pitch > 180) pitch = -pitchLimit;
        if (yaw > yawLimit && yaw < 180) yaw = yawLimit;
        else if (yaw < 360 - yawLimit && yaw > 180) yaw = -yawLimit;
        transform.localEulerAngles = new Vector3(pitch,yaw);
    }
}
