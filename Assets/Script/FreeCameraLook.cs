using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FreeCameraLook : Pivot 
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSpeed = 1.5f;
    [SerializeField] float turnSmoothing = 0.1f;
    [SerializeField] float tiltMax = 75f; // x axis of rotation of pivot
    [SerializeField] float tiltMin = 45f;
    [SerializeField] bool lockCursor = false;

    private float lookAngle;
    private float tiltAngle;
    private float smoothX = 0f;
    private float smoothY = 0f;
    private float smoothXVelocity = 0f;
    private float smoothYVelocity = 0f;

    private const float LookDistance = 100f;


    protected override void Awake()
    {
        base.Awake();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        cam = GetComponentInChildren<Camera>().transform;

        pivot = cam.parent;
    }


    protected override void Update()
    {
        base.Update();
        HandleRotationMovement();
        if(lockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    private void Disable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    protected override void Follow(float deltaTime)
    {
        transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
    }

    void HandleRotationMovement()
    {
        //float x = Input.GetAxis("Mouse X");
        //float y = Input.GetAxis("Mouse Y");
        float x = Input.GetAxis("RightStickX");
        float y = Input.GetAxis("RightStickY");

        if (turnSmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXVelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYVelocity, turnSmoothing);
        }
        else
        {
            smoothX = x;
            smoothY = y;
        }

        lookAngle += smoothX * turnSpeed;
        transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);

        tiltAngle -= smoothY * turnSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

}
