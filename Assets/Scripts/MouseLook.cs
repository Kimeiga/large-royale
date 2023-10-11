﻿using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

/// MouseLook rotates the transform based on the mouse delta. /// Minimum and Maximum values can be used to constrain the possible rotation
/// To make an FPS style character: /// - Create a capsule. /// - Add a rigid body to the capsule /// - Add the MouseLook script to the capsule. /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it) /// - Add FPSWalker script to the capsule
/// - Create a camera. Make the camera a child of the capsule. Reset it's transform. /// - Add a MouseLook script to the camera. /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public bool canRotate = true;
    public bool offsetOnly;

    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    public Quaternion originalRotation;

    public float xOffset = 0;
    public float yOffset = 0;

    // may have to add turnOffset if we add "skiing"
    public float tiltAmount = 0;

    void Update()
    {
        if (offsetOnly)
        {
            Quaternion xQuaternion = Quaternion.AngleAxis(xOffset, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(yOffset, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            return;
        }

        if (axes == RotationAxes.MouseXAndY)
        {
            if (canRotate)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);
            }

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            if (canRotate)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);
            }

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            // this will be for the head

            if (canRotate)
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);
            }


            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);
            Quaternion tempRot = originalRotation * yQuaternion;

            // yaw tilt from thrusting comes here:
            // assuming that 0, 0, 0
            Quaternion tempTilt = Quaternion.Euler(0, 0, tiltAmount);


            transform.localRotation = tempRot * tempTilt;
        }
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    public void ResetOffsets()
    {
        DOTween.To(() => xOffset, x => xOffset = x, 0, 0.7f).SetEase(Ease.OutQuart);
        DOTween.To(() => yOffset, x => yOffset = x, 0, 0.7f).SetEase(Ease.OutQuart);
    }

    public void Reset()
    {
        rotationX = 0;
        rotationY = 0;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }

        if (angle > 360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }
}