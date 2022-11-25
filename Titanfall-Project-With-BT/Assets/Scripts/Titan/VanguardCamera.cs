using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class VanguardCamera : NetworkBehaviour
{
    public Animator titanAnimator;
    public float minX = -60f;
    public float maxX = 60f;
 
    public float sensitivity;
    public Camera cam;
 
    float rotY = 0f;
    float rotX = 0f;
    float yaw;
 
    EnterVanguardTitan enterScript;
    VanguardMovement moveScript;
 
    bool shouldHeadBob;
 
    public float sprintBobSpeed;
    public float walkBobSpeed;
    public float runBobSpeed;
    public float walkBobAmount;
    public float sprintBobAmount;
    public float runBobAmount;
    float defaultY;
    private float timer;
 
    Vector3 lastMousePosition;
    Vector2 look;
 
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
 
        defaultY = cam.transform.localPosition.y;
 
        enterScript = GetComponent<EnterVanguardTitan>();
        moveScript = GetComponent<VanguardMovement>();
    }

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
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>() * sensitivity;
    }

    void Update()
    {
        if (!HasInputAuthority) return;
        
        if (enterScript.inTitan)
        {
           
            rotY += look.x;
            rotX += look.y;
 
            rotX = Mathf.Clamp(rotX, minX, maxX);
   
            transform.localEulerAngles = new Vector3(0, rotY, 0);
            cam.transform.localEulerAngles = new Vector3(-rotX, 0, 0);
           
            yaw -= Input.GetAxisRaw("Mouse Y") * 0.1f;
            yaw = Mathf.Clamp(yaw, -1f, 1f);
 
            titanAnimator.SetFloat("aim", yaw, 0.1f, Time.deltaTime);  
 
            HandleHeadBob();  
        }
 
    }
 
    void HandleHeadBob()
    {
        if (moveScript.isMoving && !moveScript.isDashing)
        {
            timer += Time.deltaTime * (moveScript.isSprinting ? sprintBobSpeed : moveScript.isWalking ?  walkBobSpeed : runBobSpeed);
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, defaultY + Mathf.Sin(timer) * (moveScript.isSprinting ? sprintBobAmount : moveScript.isWalking ? walkBobAmount : runBobAmount), cam.transform.localPosition.z);
        }
    }

}
