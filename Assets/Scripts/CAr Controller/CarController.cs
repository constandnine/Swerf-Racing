using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CarController : MonoBehaviour
{
    [Header("Input")]

    private PlayerInput playerInput;


    [Header("Driving And Steering")]

    private float accelerationValue;
    private float brakeValue;
    private float steeringValue;

    [SerializeField] private float maximalTorque;
    [SerializeField] private float maximalSteerAngle;
    [SerializeField] private float targetSteerAngle;


    [Header("Driving And Steering")]

    [SerializeField] private WheelCollider[] frontWheelColliders; 
    [SerializeField] private WheelCollider[] rearWheelColliders;

    [SerializeField] private Transform[] frontWheelTransforms;
    [SerializeField] private Transform[] rearWheelTransforms;


    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }


    private void OnDisable()
    {
        playerInput.Disable();
    }


    private void FixedUpdate()
    {
        Acceleration();
        Brake();
        Steering();


        UpdateWheels(frontWheelColliders, frontWheelTransforms);
        UpdateWheels(rearWheelColliders, rearWheelTransforms);
    }


    public void CalculateAccelerationValue(InputAction.CallbackContext context)
    {
        accelerationValue = context.ReadValue<float>();
    }


    public void CalculateBreakValue(InputAction.CallbackContext context)
    {
        brakeValue = context.ReadValue<float>();
    }


    public void CalculateSteeringValue(InputAction.CallbackContext context)
    {
        steeringValue = context.ReadValue<Vector2>().x;
    }


    private void Acceleration()
    {
        for (int i = 0; i < rearWheelColliders.Length; i++)
        {
            rearWheelColliders[i].motorTorque = accelerationValue * maximalTorque;
        }
    }


    private void Brake()
    {
        for (int i = 0; i < rearWheelColliders.Length; i++)
        {
            rearWheelColliders[i].brakeTorque = brakeValue * maximalTorque;
        }

        for (int i = 0; i < frontWheelColliders.Length; i++)
        {
            frontWheelColliders[i].brakeTorque = brakeValue * maximalTorque;
        }
    }


    private void Steering()
    {
        targetSteerAngle = maximalSteerAngle * steeringValue;


        for (int i = 0; i < frontWheelColliders.Length; i++)
        {
            frontWheelColliders[i].steerAngle = targetSteerAngle;
        }
    }


    void UpdateWheels(WheelCollider[] colliders, Transform[] transforms)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;
            colliders[i].GetWorldPose(out position, out rotation);

            transforms[i].position = position;
            transforms[i].rotation = rotation;
        }
    }
}
