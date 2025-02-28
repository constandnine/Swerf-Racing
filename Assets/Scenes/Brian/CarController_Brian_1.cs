using System.Collections;
using TMPro;
using UnityEngine;

public class CarController_Brian_1 : MonoBehaviour
{
    public AnimationCurve accelerationSpeed;
    public float acceleration;
    public AnimationCurve steeringAngle;
    public float driftFactor;
    public float turnSpeed;
    public float brakeForce;
    public bool isDrifting = false;

    private float currentSpeed;
    private Rigidbody rb;
    private float driftInput;
    private TextMeshProUGUI text;
    private TrailRenderer[] trailRenderer;

    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f);

    // Update car movement
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        trailRenderer = GetComponentsInChildren<TrailRenderer>();

        rb.centerOfMass = centerOfMassOffset;
    }

    void FixedUpdate()
    {
        HandleInput();
        ApplyDrifting();

        text.text = ((int)rb.velocity.magnitude).ToString();
    }

    // Handle input for movement and drifting
    void HandleInput()
    {
        if (!Physics.Raycast(transform.position, -transform.up, .7f))
        {
            return;
        }

        float turnInput = 0;
        float moveInput = 0;

        // Get input
        moveInput = (int)Input.GetAxis("Vertical");

        if (rb.velocity.magnitude > 0.1 || rb.velocity.magnitude > 0.1)
        {
            turnInput = Input.GetAxis("Horizontal");
        }

        driftInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        // Move the car forward and backward
        currentSpeed = rb.velocity.magnitude;
        if (moveInput > 0)
        {
            float speed = rb.velocity.magnitude + Mathf.Epsilon;
            float accelerationFactor = accelerationSpeed.Evaluate(speed) * acceleration;

            rb.AddForce(transform.forward * accelerationFactor * Time.deltaTime * rb.mass, ForceMode.Force);
            //rb.AddForce(transform.forward * CalculateSpeed() * Time.deltaTime * rb.mass, ForceMode.Force);
        }

        else if (moveInput < 0)
        {
            rb.AddForce(transform.forward * moveInput * brakeForce * Time.deltaTime * rb.mass, ForceMode.Force);
        }

        // Steer the car
        float steerAmount = turnInput * steeringAngle.Evaluate(turnInput) / rb.velocity.magnitude * 500;
        if (rb.velocity.magnitude > 5)
        {
            transform.Rotate(0, steerAmount * Time.deltaTime * turnSpeed, 0);
        }

        // Apply braking force if car is moving too fast in reverse
        if (currentSpeed > 4 && moveInput < 0)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.2f);
        }
    }

    // Apply drifting based on spacebar input
    void ApplyDrifting()
    {
        if (driftInput > 0f)
        {
            // Apply more yaw and reduce friction on the tires to simulate drifting
            rb.drag = 1.5f;
            rb.angularDrag = 7.5f;
            isDrifting = true;

            // Apply yaw force based on the steering input
            transform.Rotate(0, Input.GetAxis("Horizontal") * driftFactor * rb.velocity.magnitude, 0);

            foreach (TrailRenderer i in trailRenderer)
            {
                i.enabled = true;
            }
        }

        else
        {
            isDrifting = false;
            rb.drag = 1f;
            rb.angularDrag = 5f;

            foreach (TrailRenderer i in trailRenderer)
            {
                i.enabled = false;
            }
        }
    }
}
