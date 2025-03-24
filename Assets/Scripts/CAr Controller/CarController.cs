using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("Input")]

    private PlayerInput playerInput;


    [Header("Driving And Steering")]

    [SerializeField] private float maximalTorque;
    [SerializeField] private float maximalSteerAngle;
    [SerializeField] private float targetSteerAngle;

    [SerializeField] private float topSpeed;
    
    private float currentSpeed;
    private float speedInKmph;

    private float accelerationValue;
    private float brakeValue;
    private float steeringValue;

    private Rigidbody carRigidbody;


    [Header("Drifting")]

    [SerializeField] private float slide;

    public float deafaltGrip;

    public WheelFrictionCurve startGrip;
    public WheelFrictionCurve rearGrip;

    private bool drifting;


    [Header("Wheels")]

    [SerializeField] private WheelCollider[] frontWheelColliders; 
    [SerializeField] private WheelCollider[] rearWheelColliders;

    [SerializeField] private Transform[] frontWheelTransforms;
    [SerializeField] private Transform[] rearWheelTransforms;


    [Header("UI")]

    [SerializeField] private TMP_Text speedometerText;

    [SerializeField] private Slider accelerationSlider;


    private void Awake()
    {
        playerInput = new PlayerInput();


        carRigidbody = GetComponent<Rigidbody>();


        for (int i = 0; i < rearWheelColliders.Length; i++)
        {
            deafaltGrip = rearWheelColliders[i].sidewaysFriction.stiffness;

            startGrip = rearWheelColliders[i].sidewaysFriction;
            rearGrip = frontWheelColliders[i].sidewaysFriction;
        }

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
        CalculateSpeed();


        Acceleration();
        Brake();
        Steering();
        Drift();


        UpdateWheels(frontWheelColliders, frontWheelTransforms);
        UpdateWheels(rearWheelColliders, rearWheelTransforms);
    }


    public void CalculateAccelerationValue(InputAction.CallbackContext context)
    {
        accelerationValue = context.ReadValue<float>();


        accelerationSlider.value = accelerationValue;
    }


    public void CalculateBreakValue(InputAction.CallbackContext context)
    {
        brakeValue = context.ReadValue<float>();
    }


    public void CalculateSteeringValue(InputAction.CallbackContext context)
    {
        steeringValue = context.ReadValue<Vector2>().x;
    }



    public void OnHandbreak(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            drifting = true;
        }


        if (context.canceled)
        {
            drifting = false;
        }
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


    private void Drift()
    {
        if (drifting)
        {
            rearGrip.stiffness = deafaltGrip * slide;


            for (int i = 0; i < rearWheelColliders.Length; i++)
            {
                rearWheelColliders[i].sidewaysFriction = rearGrip;
            }
        }


        else
        {
            for (int i = 0; i < rearWheelColliders.Length; i++)
            {
                rearWheelColliders[i].sidewaysFriction = startGrip;
            }
        }
    }


    private void CalculateSpeed()
    {
        currentSpeed = carRigidbody.velocity.magnitude;


        speedInKmph = currentSpeed * 3.6f;


        if (currentSpeed > topSpeed)
        {
            carRigidbody.velocity = carRigidbody.velocity.normalized * topSpeed;
        }


        int displaySpeed = Mathf.Clamp(Mathf.RoundToInt(speedInKmph), 0, 999);


        speedometerText.text = displaySpeed.ToString("000") + "/KMH";
    }


    private void UpdateWheels(WheelCollider[] colliders, Transform[] transforms)
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


    private void CameraFOV()
    {

    }

}
