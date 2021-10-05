using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    //Class to control the movement of the player ship

    //the lever to know acceleration and the joystick to know how it should be turning
    [SerializeField] private Lever acelLever;
    [SerializeField] private Joystick dirJoystick;

    //other data to determine the limits of the movement
    [SerializeField] private float maxSpeed;
    private float currentSpeed;
    [SerializeField] private float acelRate;
    [SerializeField] private float dcelRate;
    [SerializeField] private float minRotateRate;
    [SerializeField] private float maxRotateRate;
    private float rotateRate;
    [SerializeField] private float timeToMaxRotation = 1f;
    private float timeSinceDirChange;
    private Vector2Int lastJoystickState;

    //the ship's rigidbody
    private Rigidbody rb;

    //the various compounds of the ship's acutal velocity
    private Vector3 currentVelo; //acutal final velo
    private Vector3 gravityAcel; //acceleration due to gravity
    private Vector3 thrustVelo; //velocity of the thruster 
    private float thrustSpeed;

    private Quaternion targetRotation;

    //awake, runs when the object is first initliazed
    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        targetRotation = transform.rotation;
        currentVelo = new Vector3(0f, 0f, 0f);
        timeSinceDirChange = 0f;
        lastJoystickState = new Vector2Int(0, 0);
        thrustSpeed = 0f;
    }

    //update helps handle giving the ship a smoother rotation
    private void Update()
    {
        //check if the rotation has changed
        if (lastJoystickState != dirJoystick.GetJoystickState())
        {
            //if it has update it and begin the transition again
            lastJoystickState = dirJoystick.GetJoystickState();
            timeSinceDirChange = 0.0f;
        }

        //figure out what the rotation rate should be based on the transistion
        float t = Mathf.Clamp(timeSinceDirChange / timeToMaxRotation, 0.0f, 1.0f);
        rotateRate = Mathf.Lerp(minRotateRate, maxRotateRate, t);

        HandleRotation();

        //update the time that has passed
        timeSinceDirChange += Time.deltaTime;
    }

    //called once per frame in the physics update, used to simulate the solar system gravity effect and handle the ship's other momment
    private void FixedUpdate()
    {
        //handling the positional change
        //gravity
        gravityAcel = Vector3.zero;
        //grab all of the celestial bodies
        CelestialBody[] bodies = ManagerForCelestialBodies.bodies;
        //loop thorugh them and add the approriate force from each
        foreach (var body in bodies)
        {
            //calculate force using newton's gravity formula
            Vector3 direction = body.transform.position - rb.position;
            float distanceSquared = direction.sqrMagnitude;
            Vector3 force = direction.normalized * CelestialBody.gravitationalConstant * rb.mass * body.GetMass() / distanceSquared;

            //from the force calculate the accleration from newton's a = F/m formula
            Vector3 acel = force / rb.mass;

            //and add that to the collective aceleration
            gravityAcel += acel;
        }
        
        //movement from the player input

        //the ammount the speed will increase
        float deltaSpeed = 0f;

        //using a switch statement to apply the acceleration from the data in the lever
        switch (acelLever.GetLeverState())
        {
            case 1:
                deltaSpeed = acelRate;
                break;
            case -1:
                deltaSpeed = -dcelRate;
                break;
        }

        //cacluate the speed of the thrusters
        thrustSpeed = Mathf.Clamp(thrustSpeed + deltaSpeed * Time.fixedDeltaTime, 0.0f, maxSpeed);

        //find the velocity of the thrusters from that
        thrustVelo = transform.forward.normalized * thrustSpeed;

        //add the velocities together into the position
        currentVelo += gravityAcel * Time.fixedDeltaTime;
        //rb.MovePosition(rb.position + (currentVelo + thrustVelo) * Time.fixedDeltaTime);
        rb.velocity = thrustVelo + currentVelo;
    }

    private void HandleRotation()
    {
        //handling the rotation
        float horiRot = dirJoystick.GetJoystickState().x * rotateRate;
        float vertRot = dirJoystick.GetJoystickState().y * rotateRate;

        //apply the rotation
        Quaternion yaw = Quaternion.AngleAxis(horiRot, transform.up);
        Quaternion pitch = Quaternion.AngleAxis(vertRot, transform.right);
        targetRotation = yaw * pitch * targetRotation;

        transform.rotation = targetRotation;
    }
}