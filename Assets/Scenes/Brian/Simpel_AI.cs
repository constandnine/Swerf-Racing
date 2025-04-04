using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simpel_AI : MonoBehaviour
{
    public Vector2 randomRangeTurn = new Vector2(4, 4);             // Range for random turning
    public GameObject waitpointsMain;                               // Parent object containing waypoints
    public float rayDistance = 10f;                                 // Distance for raycasting
    public LayerMask layerMask;                                     // Layer mask for obstacle detection

    private Vector3[] waitpoints;                                   // Array of waypoints
    public int waypointIndex = 0;                                   // Current waypoint index
    private Rigidbody rb;                                           // Rigidbody component
    private int updateCounter;                                      // Counter for updates
    private RaceLineData line;                                      // Current race line data
    public RaceLineData nowLine;                                    // Reference to the current race line data
    private SaveManager saveManager;                                // Reference to SaveManager

    void Awake()
    {
        rb = GetComponent<Rigidbody>();                             // Get Rigidbody component
        saveManager = GetComponent<SaveManager>();                  // Get SaveManager component
        waitpoints = GetWaypoints(waitpointsMain);                  // Get waypoints from parent object

        if (nowLine)
        {
            waitpoints = nowLine.locations.ToArray();               // Use locations from the current race line
        }

        line = ScriptableObject.CreateInstance<RaceLineData>();     // Create a new RaceLineData instance
    }

    Vector3 nextTo;                                                 // Position of the detected obstacle

    void FixedUpdate()
    {
        UpdateAI();                                                 // Update AI behavior
        nextTo = DetectObstacles();                                 // Detect obstacles

        if (++updateCounter >= 5)
        {                                                                                                 
            saveManager.SetCheckPoints(line);                       // Save checkpoints every two updates
            SetGround();
            updateCounter = 0;                                      // Reset update counter
        }
    }

    void UpdateAI()
    {
        float moveInput = Random.Range(1f, 1f);                     // Random movement input
        Vector3 targetPosition = waitpoints[waypointIndex];         // Get target waypoint position
        Vector3 direction = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;     // Calculate direction to target

        // Detect obstacles
        Vector3 obstaclePosition = nextTo;

        // If an obstacle is detected, calculate a new direction to avoid it
        if (obstaclePosition != Vector3.zero)
        {
            //Vector3 avoidanceDirection = CalculateAvoidanceDirection(obstaclePosition);
            //direction += avoidanceDirection;                        // Add avoidance direction to the movement direction
        }

        // Move Forward
        rb.AddForce(transform.forward * 10000f * moveInput * Time.fixedDeltaTime * rb.mass, ForceMode.Force);

        // Rotate Towards Target
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.fixedDeltaTime * Random.Range(randomRangeTurn.x, randomRangeTurn.y));
        }

        // Change Waypoint When Close
        if (Vector3.Distance(transform.position, targetPosition) < 5f)
        {
            if (++waypointIndex == waitpoints.Length)        // Move to the next waypoint)
            {
                saveManager.OnSaveButtonClicked(line);
                waypointIndex = 0;

                return;
            }


            if (waypointIndex == 0)
            {
                //ReloadCurrentScene();

                if (line.locations.Count < waitpoints.Length || !nowLine)   // Check if we need to save the line
                {
                    saveManager.OnSaveButtonClicked(line);                  // Save the line data
                    nowLine = line;

                    // Optionally load the saved line (Editor-only)
#if UNITY_EDITOR
                    //nowLine = saveManager.LoadFirstRaceLine();              // Load the first race line
                    if (nowLine != null)
                    {
                        waitpoints = nowLine.locations.ToArray();           // Update waypoints with loaded locations
                    }
#endif
                }
                else
                {
                    //saveManager.OnSaveButtonClicked(line);                  // Save the line data
                    //line = ScriptableObject.CreateInstance<RaceLineData>(); // Create a new instance for the next cycle
                }
            }
        }
    }

    public void ReloadCurrentScene()
    {
        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    public void SetGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 6))
        {
            Vector3 newhightpos = transform.position;
            newhightpos.y = hit.point.y + .3f;
            transform.position = newhightpos;
        }
    }

    Vector3[] GetWaypoints(GameObject parent)
    {
        List<Vector3> filteredComponents = new List<Vector3>();             // List to hold waypoint positions
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject != parent) filteredComponents.Add(child.position); // Add child position if it's not the parent
        }
        return filteredComponents.ToArray();                                // Return array of waypoints
    }

    Vector3 DetectObstacles()
    {
        Vector3[] directions1 =
        {
            transform.right,
            -transform.right,
        };

        Vector3[] directions2 =
        {
            (transform.right + transform.forward).normalized,
            (-transform.right + transform.forward).normalized
        };

        foreach (var direction in directions1)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayDistance, layerMask))
            {
                Debug.LogError($"Hit: {hit.collider.name} in direction: {direction}");
                return hit.transform.position;                                  // Return position of detected obstacle
            }
            else
            {
                Debug.Log($"No hit in direction: {direction}");
            }
        }

        foreach (var direction in directions2)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayDistance * 2, layerMask))
            {
                Debug.LogError($"Hit: {hit.collider.name} in direction: {direction}");
                return hit.transform.position;                                  // Return position of detected obstacle
            }
            else
            {
                Debug.Log($"No hit in direction: {direction}");
            }
        }

        return Vector3.zero;                                                    // No obstacles detected
    }

    private Vector3 CalculateAvoidanceDirection(Vector3 obstaclePosition)
    {
        Vector3 awayFromObstacle = transform.position - obstaclePosition;       // Calculate direction away from obstacle
        awayFromObstacle.Normalize();                                           // Normalize the direction
        awayFromObstacle *= 2f;                                                 // Scale the direction
        return awayFromObstacle;                                                // Return avoidance direction
    }
}