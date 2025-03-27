using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeacherController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Camera trackingCamera; // The camera that the agent will follow
    public bool followCamera = false; // Public switch to enable/disable following
    public float followDistance = 5.0f; // The desired distance between the agent and the camera
    public float followDelay = 2.0f; // Time delay before starting to follow
    public float maxSpeed = 6.0f; // The maximum speed the agent will reach
    public float accelerationTime = 5.0f; // Time it takes to reach max speed
    public Animator animator; // Reference to the Animator component

    private bool isFollowing = false; // Flag to check if following has started
    private float timer = 0.0f; // Timer to track delay
    private float currentSpeed = 0.0f; // The current speed of the agent
    private float speedIncrement; // Incremental speed value

    void Start()
    {
        // Initialize NavMeshAgent speed to 0
        agent.speed = 0.0f;
        currentSpeed = 0.0f;

        // Calculate the speed increment based on acceleration time
        speedIncrement = maxSpeed / accelerationTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (followCamera && trackingCamera != null)
        {
            // Calculate the distance between the agent and the camera
            Vector3 cameraPosition = trackingCamera.transform.position;
            float distance = Vector3.Distance(agent.transform.position, cameraPosition);

            // If the agent is farther than the followDistance, start the timer
            if (distance > followDistance)
            {
                timer += Time.deltaTime;

                if (timer >= followDelay)
                {
                    isFollowing = true;
                }
            }
            else
            {
                timer = 0.0f;
                isFollowing = false;
            }

            // If following is active, move the agent and gradually increase speed
            if (isFollowing)
            {
                // Gradually increase the speed until reaching maxSpeed
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += speedIncrement * Time.deltaTime;
                    currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // Clamp to maxSpeed
                }

                agent.speed = currentSpeed; // Apply the speed to the agent

                // Get the direction from the agent to the camera
                Vector3 directionToCamera = (cameraPosition - agent.transform.position).normalized;

                // Set the target position at the desired follow distance
                Vector3 targetPosition = cameraPosition - directionToCamera * followDistance;

                // Set the agent's destination to the target position
                agent.SetDestination(targetPosition);
            }
            else
            {
                // Reset speed and stop the agent when not following
                currentSpeed = 0.0f;
                agent.speed = 0.0f;
                agent.ResetPath();
            }

            // Update the animator based on the agent's current speed
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            // If not following the camera, reset everything
            currentSpeed = 0.0f;
            agent.speed = 0.0f;
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
        }
    }
}
