using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("CarController")]

    [SerializeField] private CarController carController;


    [Header("Camera Movement")]

    [SerializeField] private Transform target;
    [SerializeField] private Transform car;

    [SerializeField] private float followSpeed;
    [SerializeField] private float minimalDistance;
    [SerializeField] private float maximalDistance;


    [Header("Speed VFX")]

    [SerializeField] private Camera chaseCamera;

    [SerializeField] private ParticleSystem speedStripes;

    [SerializeField] private float speedVFXRadius;
    private float SpeedVFXStartRadius;


    [Header("Camera Shake Effect")]

    [SerializeField] private float shakeIntensity;

    private Vector3 currentPosition;


    private void Start()
    {
        var speedVFXShape = speedStripes.shape;
        SpeedVFXStartRadius = speedVFXShape.radius;
    }


    public void Update()
    {
        MoveCamera();
        SpeedEffect();
    }


    private void MoveCamera()
    {
        if (Vector3.Distance(transform.position, car.position) > minimalDistance)
        {
            transform.position = Vector3.Lerp(transform.position, car.position, Time.deltaTime * followSpeed);
        }


        Vector3 offset = transform.position - car.position;

        if (offset.magnitude > maximalDistance)
        {
            transform.position = car.position + offset.normalized * maximalDistance;
        }


        transform.LookAt(target.transform);
    }


    private void SpeedEffect()
    {
        if (carController.accelerationValue > 0 && chaseCamera.fieldOfView <= 100)
        {
            chaseCamera.fieldOfView += .1f;
        }

        if(carController.brakeValue > 0 && chaseCamera.fieldOfView >= 60)
        {
            chaseCamera.fieldOfView -= .2f;
        }
          
        if (chaseCamera.fieldOfView > 60 && carController.brakeValue == 0 && carController.accelerationValue == 0)
        {
            chaseCamera.fieldOfView -= .1f;
        }


        if (carController.speedInKmph > 75)
        {
            CamaraShake();


            var speedVFXShape = speedStripes.shape;

            if(speedVFXShape.radius > speedVFXRadius)
            {
                speedVFXShape.radius -= .1f;
            }
        }

        else if(carController.speedInKmph < 75 && speedStripes.shape.radius < SpeedVFXStartRadius)
        {
            var speedVFXShape = speedStripes.shape;
            speedVFXShape.radius += .5f;
        }
    }


    private void CamaraShake()
    {
        currentPosition = transform.localPosition;
        currentPosition.x += Random.insideUnitSphere.x * shakeIntensity;
        transform.localPosition = currentPosition;
    }
}
