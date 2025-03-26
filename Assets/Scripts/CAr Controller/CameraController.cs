using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]

    [SerializeField] private Transform target;
    [SerializeField] private Transform car;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float minimalDistance;
    [SerializeField] private float maximalDistance;


    [Header("Speed VFX")]

    [SerializeField] private Camera Chasecamera;


    public void Update()
    {
        MoveCamera();
        SpeedEffect();
    }


    private void MoveCamera()
    {
        if (Vector3.Distance(transform.position, car.position) >= minimalDistance)
        {
            transform.position = Vector3.Lerp(transform.position, car.position, Time.deltaTime * movementSpeed);
        }


        Vector3 offset = transform.position - car.position;

        if (offset.magnitude > maximalDistance)
        {
            transform.position = car.position + offset.normalized * maximalDistance;
        }


        transform.position = target.position * (1 - movementSpeed) + transform.position * movementSpeed;


        transform.LookAt(target.transform);
    }


    private void SpeedEffect()
    {
        if (Chasecamera.fieldOfView > 60 && Chasecamera.fieldOfView < 80)
        {
            Chasecamera.fieldOfView += .5f;
        }
    }
}
