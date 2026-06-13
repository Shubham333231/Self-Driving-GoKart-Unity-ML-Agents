using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// Self-Driving Go-Kart Agent using Unity ML-Agents (PPO)
/// Author: Shubham Sharma
/// B.Tech ECE, Matoshri College of Engineering, Nashik
/// </summary>
public class GoKartAgent : Agent
{
    [Header("Kart Physics")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Kart Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 35f;
    public float maxBrakeTorque = 3000f;
    public float maxSpeed = 60f;

    [Header("Track Checkpoints")]
    public Transform[] checkpoints;
    public Transform startPosition;

    [Header("Reward Settings")]
    public float forwardReward = 0.1f;
    public float checkpointReward = 1.0f;
    public float lapCompletionReward = 5.0f;
    public float wallCollisionPenalty = -0.5f;
    public float offTrackPenalty = -1.0f;
    public float stationaryPenalty = -0.01f;

    private Rigidbody rb;
    private int nextCheckpointIndex = 0;
    private float previousDistanceToCheckpoint;
    private float episodeStartTime;
    private bool isOnTrack = true;
    private int totalCheckpointsPassed = 0;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Reset agent to start position at beginning of each episode
    /// </summary>
    public override void OnEpisodeBegin()
    {
        // Reset kart position and rotation
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;

        // Reset physics
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset episode variables
        nextCheckpointIndex = 0;
        totalCheckpointsPassed = 0;
        isOnTrack = true;
        episodeStartTime = Time.time;

        if (checkpoints.Length > 0)
        {
            previousDistanceToCheckpoint = Vector3.Distance(
                transform.position,
                checkpoints[nextCheckpointIndex].position
            );
        }
    }

    /// <summary>
    /// Collect observations from environment:
    /// - Kart velocity (3 values)
    /// - Kart angular velocity (3 values)
    /// - Direction to next checkpoint (3 values)
    /// - Distance to next checkpoint (1 value)
    /// - Kart's forward direction (3 values)
    /// Total: 13 observations
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Velocity observations
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);

        // Checkpoint direction and distance
        if (checkpoints.Length > 0)
        {
            Vector3 directionToCheckpoint = (checkpoints[nextCheckpointIndex].position
                - transform.position).normalized;
            float distanceToCheckpoint = Vector3.Distance(
                transform.position,
                checkpoints[nextCheckpointIndex].position
            );

            sensor.AddObservation(directionToCheckpoint);
            sensor.AddObservation(distanceToCheckpoint / 100f); // Normalized
        }

        // Kart orientation
        sensor.AddObservation(transform.forward);
    }

    /// <summary>
    /// Apply actions from neural network:
    /// - Action[0]: Steering (-1 to 1)
    /// - Action[1]: Acceleration (0 to 1)
    /// - Action[2]: Braking (0 to 1)
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        float steering = actions.ContinuousActions[0];    // -1 to 1
        float acceleration = actions.ContinuousActions[1]; // 0 to 1
        float braking = actions.ContinuousActions[2];      // 0 to 1

        // Apply steering
        float steerAngle = steering * maxSteeringAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        // Apply motor torque to rear wheels
        float motorTorque = acceleration * maxMotorTorque;
        rearLeftWheel.motorTorque = motorTorque;
        rearRightWheel.motorTorque = motorTorque;

        // Apply braking
        float brakeTorque = braking * maxBrakeTorque;
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;

        // Speed limit enforcement
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        ApplyRewards();
    }

    private void ApplyRewards()
    {
        if (checkpoints.Length == 0) return;

        // Reward for moving toward next checkpoint
        float currentDistance = Vector3.Distance(
            transform.position,
            checkpoints[nextCheckpointIndex].position
        );

        float distanceDelta = previousDistanceToCheckpoint - currentDistance;
        if (distanceDelta > 0)
        {
            AddReward(forwardReward * distanceDelta);
        }
        previousDistanceToCheckpoint = currentDistance;

        // Penalty for being stationary
        if (rb.velocity.magnitude < 0.5f)
        {
            AddReward(stationaryPenalty);
        }

        // Penalty for going off track
        if (!isOnTrack)
        {
            AddReward(offTrackPenalty);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpoint reached
        if (other.CompareTag("Checkpoint"))
        {
            int checkpointIndex = System.Array.IndexOf(
                System.Array.ConvertAll(checkpoints, t => t.GetComponent<Collider>()),
                other
            );

            if (checkpointIndex == nextCheckpointIndex)
            {
                AddReward(checkpointReward);
                totalCheckpointsPassed++;
                nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Length;

                // Lap completed
                if (nextCheckpointIndex == 0)
                {
                    AddReward(lapCompletionReward);
                    EndEpisode();
                }
            }
        }

        // Off track detection
        if (other.CompareTag("OffTrack"))
        {
            isOnTrack = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Wall collision penalty
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(wallCollisionPenalty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Track"))
        {
            isOnTrack = false;
        }
    }

    /// <summary>
    /// Manual control for testing (keyboard override)
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");  // Steering
        continuousActions[1] = Input.GetAxis("Vertical") > 0
            ? Input.GetAxis("Vertical") : 0f;                 // Acceleration
        continuousActions[2] = Input.GetAxis("Vertical") < 0
            ? -Input.GetAxis("Vertical") : 0f;                // Braking
    }
}
