using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.Drawing;


/// <summary>
/// Controls camera rotation
/// Requires cameraAnchorX for the object rotating on the X axis
/// Requires cameraAnchorZ for the object rotating on the Z axis
/// 
/// Parent cameraAnchorX over cameraAnchorZ over camera.
/// center cameraAnchorZ on cameraAnchorX
/// 
/// For mouselook, center camera on cameraAnchorZ.
/// For orbit camera, move cameraAnchorX away from the camera.
/// 
/// </summary>

public class CameraControl : MonoBehaviour {
    
    //## - Means set in the inspector

    public float XSensitivity = 10f;//##
    public float YSensitivity = 10f;//##
    public float invertX = 1;
    public float invertY = 1;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;//##
    public float MaximumX = 90;//##
    public bool smooth;
    public float smoothTime = 2f;//##
    public Transform cameraAnchorZ;//##
    public Transform cameraAnchorX;//##

    private Quaternion cameraAnchorTargetRotX;
    private Quaternion cameraAnchorTargetRotZ;
    private bool MLMode = true;

    public bool setLockPos = true;
    private Point mouseLockPos;

    public void Awake()
    {
        //Sets the character and camera's target rotation to their current one when the script starts running.
        cameraAnchorTargetRotZ = cameraAnchorZ.localRotation;
        cameraAnchorTargetRotX = cameraAnchorX.localRotation;
    }


    public void Update()
    {
        //on middle mouse click, makes the mouse invisible, locks it in place, and calls to rotate the camera
        if (Input.GetAxis("MiddleClick") == 1)
        {
            if (setLockPos)
            {
                mouseLockPos = System.Windows.Forms.Cursor.Position;
                setLockPos = false;
                UnityEngine.Cursor.visible = false;
            }
            System.Windows.Forms.Cursor.Position = mouseLockPos;
            rotateCam();
        }
        else
        {
            setLockPos = true;
            UnityEngine.Cursor.visible = true;
        }
    }


    private void rotateCam()
    {
        //Updates every frame. If the player's in mouselook mode, it runs the function that does the work and checks to see if they want to lock/unlock the mouse.
        if (MLMode)
        {
            lookRotation();
        }
    }


    private void lookRotation()
    {
        //This is what runs the mouselook, rotating the camera anchors.
        float yRot = Input.GetAxis("Mouse X") * XSensitivity * invertX;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity * invertY;

        cameraAnchorTargetRotZ *= Quaternion.Euler(0f, yRot, 0f);
        cameraAnchorTargetRotX *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            cameraAnchorTargetRotX = clampRotationAroundXAxis(cameraAnchorTargetRotX);

        if (smooth)
        {
            cameraAnchorTargetRotZ = Quaternion.Slerp(cameraAnchorZ.localRotation, cameraAnchorTargetRotZ,
                smoothTime * Time.deltaTime);
            cameraAnchorTargetRotX = Quaternion.Slerp(cameraAnchorX.localRotation, cameraAnchorTargetRotX,
                smoothTime * Time.deltaTime);
        }
        //cameraAnchor.localRotation = Quaternion.Euler(cameraAnchorTargetRotUpright);
        cameraAnchorZ.localRotation = cameraAnchorTargetRotZ;
        cameraAnchorX.localRotation = cameraAnchorTargetRotX;
    }

    private Quaternion clampRotationAroundXAxis(Quaternion q)
    {
        //This clamps the vertical axis so the player can't do terrible things with the camera.
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX); 
        return q;
    }




    public void setMLMode(bool value)
    {
        //This is a function to be called by other scripts to enable/disable rotation
        MLMode = value;
    }

    //These two are for inverting the X and Y movement.
    public void invertXLook()
    {
        invertX *= -1;
    }
    public void invertYLook()
    {
        invertY *= -1;
    }
}