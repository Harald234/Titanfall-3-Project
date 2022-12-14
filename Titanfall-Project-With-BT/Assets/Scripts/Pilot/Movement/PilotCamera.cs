using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PilotCamera : NetworkBehaviour
{
    public float minX = -60f;
    public float maxX = 60f;

    public float sensitivity;
    public Camera cam;

    public Vector2 look;
    
    [Networked]
    private float rotY { get; set; }
    
    [Networked]
    private float rotX { get; set; }

    PilotMovement move;

    public float sprintBobSpeed;
    public float runBobSpeed;
    public float sprintBobAmount;
    public float runBobAmount;
    float defaultY;
    private float timer;

    void Start()
    {
        if (!HasInputAuthority)
        {
            cam.enabled = false;
            if (cam.gameObject.TryGetComponent(out AudioListener audioListener))
            {
                audioListener.enabled = false;
            }
        }

        if (transform == null)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        move = GetComponentInParent<PilotMovement>();

        defaultY = cam.transform.localPosition.y;
    }
    
    void Update()
    {
        if (move.canMove == false)
            return;

        rotY += look.x;
        rotX += look.y;

        rotX = Mathf.Clamp(rotX, minX, maxX);

        transform.localEulerAngles = new Vector3(0, rotY, 0);
        cam.transform.localEulerAngles = new Vector3(-rotX, 0, move.tilt);

        HandleHeadBob();
    }

    void HandleHeadBob()
    {
        if (move.isMoving && move.isGrounded && !move.isSliding)
        {
            timer += Time.deltaTime * (move.isSprinting ? sprintBobSpeed : runBobSpeed);
            Transform camTransform = cam.transform;
            camTransform.localPosition = new Vector3(camTransform.localPosition.x, defaultY + Mathf.Sin(timer) * (move.isSprinting ? sprintBobAmount : runBobAmount), camTransform.localPosition.z);
        }
    }

}
