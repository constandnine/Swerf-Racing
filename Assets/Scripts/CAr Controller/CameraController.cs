using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("input")]

    private PlayerInput playerInput;


    [Header("Camera Movement")]

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offSetTarget;
    [SerializeField] private float movementSpeed;


    [Header("Camera Movement")]

    [SerializeField] private Transform rotateTarget;

    [SerializeField] private float rotationSpeed;

    private float rotationInput;


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


    public void CalculateRotationInput(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<Vector2>().x;
    }


    public void LateUpdate()
    {
        MoveCameraForward();
        RotateCamera();
    }


    private void MoveCameraForward()
    {
        Vector3 desiredDistanceToTarget = target.position + offSetTarget;


        transform.position = Vector3.Lerp(transform.position, desiredDistanceToTarget,Time.deltaTime * movementSpeed);
    }


    private void RotateCamera()
    {
        float verticalRotation = rotationInput * rotationSpeed * Time.deltaTime;


        rotateTarget.Rotate(0, verticalRotation, 0);
    }
}
