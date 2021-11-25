using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLandingMovement : MonoBehaviour
{
    //the ship's rigidbody
    private Rigidbody rb;
    //the movement manager
    private ShipMoveManager manager;

    [SerializeField] float landingSpeed = 10f;
    [SerializeField] float takeOffSpeed = 10f;

    CelestialBodyDeterministic closetBody;

    Quaternion startingRotation, endingRotation;
    Vector3 startingPosition, endingPosition;
    Vector3 targetLastPosition, targetCurrentPosition, targetChange;
    float currentTime, totalTime;

    private enum landingStates
    {
        None,
        Landing,
        Landed,
        TakingOff
    }

    private landingStates landedState;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        manager = this.GetComponent<ShipMoveManager>();
        landedState = landingStates.None;
    }

    public void SetClosetBody(CelestialBodyDeterministic body)
    {
        closetBody = body;
    }

    // Update is called once per frame
    void Update()
    {
        targetCurrentPosition = closetBody.transform.position;
        targetChange = targetCurrentPosition - targetLastPosition;

        rb.position += targetChange;
        startingPosition += targetChange;
        endingPosition += targetChange;

        //update based on the current state
        switch (landedState)
        {
            case landingStates.Landing:
                landingUpdate();
                break;
            case landingStates.Landed:
                GroundedUpdate();
                break;
            case landingStates.TakingOff:
                takeOffUpdate();
                break;
        }

        targetLastPosition = targetCurrentPosition;
    }

    public void StartLanding()
    {
        landedState = landingStates.Landing;
        rb.isKinematic = true;
        startingRotation = transform.rotation;
        Vector3 down = (closetBody.transform.position - transform.position).normalized;
        endingRotation = Quaternion.FromToRotation(-transform.up, down) * transform.rotation;

        startingPosition = transform.position;

        //this is causing it to land outside the planet for some reason
        LayerMask mask = 1 << 10; //only try against celestial bodies
        RaycastHit hit;
        Physics.Raycast(rb.position, down, out hit, Mathf.Infinity, mask);
        endingPosition = rb.position + down * hit.distance;

        currentTime = 0f;
        totalTime = (endingPosition - startingPosition).magnitude / landingSpeed;
        targetCurrentPosition = closetBody.transform.position;
        targetLastPosition = targetCurrentPosition;
    }

    private void StartTakeOff()
    {
        landedState = landingStates.TakingOff;
        rb.isKinematic = true;

        startingPosition = rb.position;
        //calculate the new ending position
        float maxGravityDistance = manager.gravitySOIMultiplier * closetBody.radius;
        float targetDIstance = 0.75f * maxGravityDistance;
        endingPosition = rb.position + (transform.up * targetDIstance);

        currentTime = 0f;
        totalTime = (endingPosition - startingPosition).magnitude / takeOffSpeed;
    }

    private void landingUpdate()
    {
        //calculate t
        float t = Mathf.Clamp(currentTime / totalTime, 0.0f, 1.0f);

        //update position
        rb.position = Vector3.Lerp(startingPosition, endingPosition, t);
        //update rotation
        transform.rotation = Quaternion.Slerp(startingRotation, endingRotation, t);

        //update current time 
        currentTime += Time.deltaTime;

        //if we're done landing switch to the grounded update
        if (currentTime >= totalTime) landedState = landingStates.Landed;
    }

    private void takeOffUpdate()
    {
        //calculate t
        float t = Mathf.Clamp(currentTime / totalTime, 0.0f, 1.0f);

        //update position
        rb.position = Vector3.Lerp(startingPosition, endingPosition, t);

        //update current time 
        currentTime += Time.deltaTime;

        //if we're done transistion to free flying
        if (currentTime >= totalTime)
        {
            landedState = landingStates.None;
            manager.EndTakeOff();
        }
    }

    private void GroundedUpdate()
    {
        //if the button has been unpressed you need to take off
        if (!manager.GetButtonState())
        {
            StartTakeOff();
        }
    }
}