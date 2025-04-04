using System.Collections.Generic;
using UnityEngine;

public class Follower_AI : MonoBehaviour
{
    #region Public Variables
    public GameObject mainCar;                              // Reference to the main car to compare position
    public GameObject car;                                  // The car controlled by this script
    public Vector2 speedRange = new Vector2(0.6f, 1.8f);    // Min and max speed multipliers
    public Vector2 randomTurnRange = new Vector2(2, 6);     // Range for random turn speed (not used now)
    public GameObject waypointsMain;                        // Parent object containing waypoints
    public float rayDistance = 10f;                         // Distance for obstacle detection rays
    public LayerMask layerMask;                             // Layer mask for raycasts
    public float distanceThreshold = 25f;                   // Distance threshold for speed adjustment
    public float aggressionFactor = 1.2f;                   // Max aggression for initial speed variation
    public float switchDistance = 10f;                      // Distance to switch to next waypoint
    public float offsetMagnitude = 5f;                      // Magnitude of lateral offset
    public float noiseScale = 0.1f;                         // Scale of Perlin noise for path variation
    public float overtakeDistance = 10f;                    // Distance to initiate overtaking
    public float criticalDistance = 5f;                     // Distance for speed reduction when car is in front
    public float overtakeBias = 0.5f;                       // Bias for overtaking direction
    public float speedAdjustmentRate = 2f;                  // Rate at which speed adjusts to target
    public float noiseSmoothRate = 5f;                      // Rate at which noise value is smoothed
    public float turnRate = 5f;                             // Fixed turn rate for smoother rotation
    public RaceLineData nowLine;                            // Optional predefined race line
    #endregion

    #region Private Variables
    private Vector3[] waypoints;            // Array of waypoint positions
    private int waypointIndex = 0;          // Current waypoint index
    private Rigidbody rb;                   // Rigidbody component
    private int updateCounter;              // Counter for periodic updates
    private RaceLineData currentRaceLine;   // Current race line data
    private SaveManager saveManager;        // Save manager component
    private Vector3 nextObstacle;           // Position of detected obstacle
    private float speedMultiplier = 1f;     // Current speed multiplier
    private float targetSpeedMultiplier;    // Target speed multiplier for smooth adjustment
    private float randomAggression;         // Initial random aggression factor
    private float noiseOffset;              // Offset for Perlin noise
    private Vector3 currentStart;           // Start of current waypoint segment
    private Vector3 currentEnd;             // End of current waypoint segment
    private int overtakeSide = 0;           // Overtaking direction (-1 left, 1 right)
    private float smoothedNoiseValue;       // Smoothed noise value for offset
    #endregion

    #region Initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        saveManager = GetComponent<SaveManager>();
        waypoints = GetWaypoints(waypointsMain);

        // Use predefined waypoints if provided
        if (nowLine)
        {
            waypoints = nowLine.locations.ToArray();
        }

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned to Follower_AI!");
            return;
        }

        currentRaceLine = ScriptableObject.CreateInstance<RaceLineData>();
        randomAggression = Random.Range(0.8f, aggressionFactor);
        speedMultiplier = Random.Range(speedRange.x, speedRange.y) * randomAggression;
        targetSpeedMultiplier = speedMultiplier; // Initialize target to starting speed
        noiseOffset = Random.value * 1000f;
        smoothedNoiseValue = 0f; // Initialize smoothed noise value

        waypointIndex = 0;
        currentStart = waypoints[waypoints.Length - 1];
        currentEnd = waypoints[0];
    }
    #endregion

    #region Update
    void FixedUpdate()
    {
        // Update target speed and smoothly adjust current speed
        AdjustSpeed();
        speedMultiplier = Mathf.Lerp(speedMultiplier, targetSpeedMultiplier, Time.fixedDeltaTime * speedAdjustmentRate);
        UpdateAI();
        SetGround();
        nextObstacle = DetectObstacles();

        // Periodic checkpoint saving
        if (++updateCounter >= 2)
        {
            //saveManager.SetCheckPoints(currentRaceLine);
            updateCounter = 0;
        }

        // Debug time scale toggle
        if (Input.GetKeyDown(KeyCode.Space)) timeSwitch();
    }
    #endregion

    #region Adjust Speed Logic
    void AdjustSpeed()
    {
        if (!mainCar) return;

        float distance = Vector3.Distance(car.transform.position, mainCar.transform.position);

        int mainWaypointIndex = waypointIndex;

        if (mainCar)
        {
            mainWaypointIndex = mainCar.GetComponent<Follower_AI>().waypointIndex;
        }

        if (distance > distanceThreshold)
        {
            // Car is behind main car: speed up
            if (waypointIndex < mainWaypointIndex)
            {
                targetSpeedMultiplier = speedRange.y;
            }
            // Car is ahead of main car: slow down
            else if (waypointIndex > mainWaypointIndex)
            {
                targetSpeedMultiplier = speedRange.x;
            }
            // Car is at same waypoint: neutral speed
            else
            {
                targetSpeedMultiplier = (speedRange.x + speedRange.y) / 2f;
            }
        }
        else
        {
            // When close to main car, use default speed
            targetSpeedMultiplier = 1f;
        }
    }
    #endregion

    #region AI Update Logic
    void UpdateAI()
    {
        if (waypoints.Length == 0) return;

        // Switch to the next waypoint if close enough
        if (Vector3.Distance(transform.position, currentEnd) < switchDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            currentStart = currentEnd;
            currentEnd = waypoints[waypointIndex];
        }

        // Calculate progress along the track
        Vector3 trackDirection = (currentEnd - currentStart).normalized;
        float segmentLength = Vector3.Distance(currentStart, currentEnd);
        Vector3 carVector = transform.position - currentStart;
        float projection = Vector3.Dot(carVector, trackDirection);
        projection = Mathf.Clamp(projection, 0, segmentLength);
        float fraction = projection / segmentLength;
        float progress = waypointIndex + fraction;

        // Generate Perlin noise for offset
        float noiseValue = Mathf.PerlinNoise(noiseOffset + progress * noiseScale, 0) * 2 - 1;

        // Handle car in front and overtaking
        float avoidanceMultiplier = 1f;
        float distanceToFrontCar;
        if (DetectCarInFront(out distanceToFrontCar))
        {
            if (distanceToFrontCar < criticalDistance)
            {
                float factor = distanceToFrontCar / criticalDistance;
                avoidanceMultiplier = Mathf.Lerp(0.5f, 1f, factor);
            }
            if (distanceToFrontCar < overtakeDistance)
            {
                if (overtakeSide == 0)
                {
                    overtakeSide = Random.Range(0, 2) * 2 - 1; // -1 or 1
                }
            }
        }
        else
        {
            overtakeSide = 0;
            avoidanceMultiplier = 1f;
        }

        // Calculate target noise value with overtaking bias
        float targetNoiseValue = noiseValue + overtakeSide * overtakeBias;

        // Smooth the noise value
        smoothedNoiseValue = Mathf.Lerp(smoothedNoiseValue, targetNoiseValue, Time.deltaTime * noiseSmoothRate);

        // Define perpendicular directions for current and next segments
        Vector3 currentPerpendicular = Vector3.Cross(trackDirection, Vector3.up).normalized;
        Vector3 nextWaypoint = waypoints[(waypointIndex + 1) % waypoints.Length];
        Vector3 nextTrackDirection = (nextWaypoint - currentEnd).normalized;
        Vector3 nextPerpendicular = Vector3.Cross(nextTrackDirection, Vector3.up).normalized;

        // Calculate offsets using smoothed noise value
        Vector3 currentOffset = currentPerpendicular * smoothedNoiseValue * offsetMagnitude;
        Vector3 nextOffset = nextPerpendicular * smoothedNoiseValue * offsetMagnitude;

        // Blend offsets based on proximity to currentEnd
        float distanceToEnd = Vector3.Distance(transform.position, currentEnd);
        float blend = 0f;
        if (distanceToEnd < switchDistance)
        {
            blend = 1 - (distanceToEnd / switchDistance); // 0 when far, 1 when at waypoint
        }
        Vector3 blendedOffset = Vector3.Lerp(currentOffset, nextOffset, blend);

        // Set target position
        Vector3 targetPosition = currentEnd + blendedOffset;

        // Calculate direction
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Adjust direction with obstacle avoidance
        if (nextObstacle != Vector3.zero)
        {
            Vector3 avoidanceDirection = CalculateAvoidanceDirection(nextObstacle);
            direction += avoidanceDirection * Random.Range(0.5f, 1.5f);
            direction.Normalize();
        }

        // Apply movement force
        float moveForce = 11000f * speedMultiplier * avoidanceMultiplier * Time.fixedDeltaTime * rb.mass;
        rb.AddForce(transform.forward * moveForce, ForceMode.Force);

        // Smoothly rotate towards the direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnRate);
        }
    }
    #endregion

    #region Detect Car in Front
    private bool DetectCarInFront(out float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, overtakeDistance, layerMask))
        {
            if (hit.collider.gameObject != gameObject)
            {
                distance = hit.distance;
                return true;
            }
        }
        distance = 0f;
        return false;
    }
    #endregion

    #region Get Waypoints
    Vector3[] GetWaypoints(GameObject parent)
    {
        List<Vector3> waypointList = new List<Vector3>();
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject != parent) waypointList.Add(child.position);
        }
        return waypointList.ToArray();
    }
    #endregion

    #region Detect Obstacles
    Vector3 DetectObstacles()
    {
        Vector3[] directions = { transform.right, -transform.right};

        foreach (var direction in directions)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayDistance, layerMask))
            {
                return hit.transform.position;
            }
        }
        return Vector3.zero;
    }
    #endregion

    #region Calculate Avoidance Direction
    private Vector3 CalculateAvoidanceDirection(Vector3 obstaclePosition)
    {
        Vector3 awayFromObstacle = (transform.position - obstaclePosition).normalized * Random.Range(1.5f, 3f);
        return awayFromObstacle;
    }
    #endregion

    #region Time Scale Toggle
    public void timeSwitch()
    {
        Time.timeScale = Time.timeScale == 10 ? 1 : 10;
    }
    #endregion

    public void SetGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 6))
        {
            Vector3 newhightpos = transform.position;
            newhightpos.y = hit.point.y + .1f;
            transform.position = newhightpos;
        }
    }
}