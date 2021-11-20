using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFreeSpaceMove : MonoBehaviour
{
    //Class to control the movement of the player ship

    //the lever to know acceleration and the joystick to know how it should be turning
    [SerializeField] private Lever acelLever;
    [SerializeField] private I_Joystick dirJoystick;

    //other data to determine the limits of the movement
    [SerializeField] private float maxSpeed;

    //old data
    private float currentSpeed;
    [SerializeField] private float acelRate;
    [SerializeField] private float dcelRate;
    [SerializeField] private float maxAcelForce = 50;
    [SerializeField] private float minRotateRate;
    [SerializeField] private float maxRotateRate;
    private float rotateRate;
    [SerializeField] private float timeToMaxRotation = 1f;
    [SerializeField] private float springDampning = 1;
    private float timeSinceDirChange;
    private Vector2Int lastJoystickState;

    //the ship's rigidbody
    private Rigidbody rb;

    //the various compounds of the ship's acutal velocity
    private float thrustSpeed;

    //the current target rotation of the ship
    [HideInInspector] public Quaternion targetRotation;

    //manager to get data
    ShipMoveManager manager;

    //awake, runs when the object is first initliazed
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        manager = this.GetComponent<ShipMoveManager>();
        targetRotation = transform.rotation;
        lastJoystickState = new Vector2Int(0, 0);
        thrustSpeed = 0f;
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
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

        //check if the ship is trying to land and even can land
        if (manager.GetButtonState())
        {
            //find the closet body

            CelestialBodyDeterministic[] gravityBodies = ShipMoveManager.GetGravityBodies();
            //loop thorugh them and add the approriate force from each
            CelestialBodyDeterministic closetBody = null;
            foreach (var body in gravityBodies)
            {
                Vector3 direction = body.transform.position - rb.position;
                if (body.mass > 0 && direction.magnitude <= body.radius * manager.gravitySOIMultiplier)
                {
                    if (closetBody == null) closetBody = body;
                    else if (direction.magnitude < (closetBody.transform.position - rb.position).magnitude) closetBody = body;
                }
            }

            //if we found one we can land on then begin landing on it
            if (closetBody != null)
            {
                //begin landing
                manager.BeginLanding(closetBody);
                //reset the thrust speed
                thrustSpeed = 0f;
            }
            //otherwise set the button back to false
            else manager.SetButtonState(false);
        }

        //update the time that has passed
        timeSinceDirChange += Time.deltaTime;
    }

    //called once per frame in the physics update, used to simulate the solar system gravity effect and handle the ship's other momment
    private void FixedUpdate()
    {
        //handling the positional change
        //the ammount the speed of the thruster will increase
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
        Vector3 thrustVelo = transform.forward.normalized * thrustSpeed;

        //get the force needed to apply the velocity from the thrusters
        Vector3 thrustForce = GetThrustForce(thrustVelo, Time.fixedDeltaTime);

        //get the force from the gravity
        Vector3 gravityForce = GetGravityForce();

        //apply both of these forces
        rb.AddForce(gravityForce);
        rb.AddForce(thrustForce);

        //rotate the ship based on the target rotation set in the handle rotation function
        Vector3 torque = GetTorqueForRotation();
        rb.AddTorque(torque, ForceMode.VelocityChange);
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
    }

    //function that will return the torque required to rotate the ship into the desired direction
    //based on code by the Very Very Valet devs: https://youtu.be/qdskE8PJy6Q
    private Vector3 GetTorqueForRotation()
    {
        //find the rotation needed to get from the player's current to their target rotation
        Quaternion deltaRot = targetRotation * Quaternion.Inverse(transform.rotation);

        //get the angle axis representation of that rotation
        Vector3 axis;
        float rotD;
        deltaRot.ToAngleAxis(out rotD, out axis);
        axis.Normalize();

        //convert it to radians
        float rotR = rotD * Mathf.Deg2Rad;

        //calculate and return the result
        return (axis * (rotR * springDampning) - (rb.angularVelocity));
    }

    //function that will return the force required to move the ship into the desire speed from it's thrust specfically
    //based on code by the Very Very Valet devs: https://youtu.be/qdskE8PJy6Q
    private Vector3 GetThrustForce(Vector3 desiredVelo, float timeStep)
    {
        //calculated the accleration needed
        Vector3 requiredAcel = (desiredVelo - rb.velocity) / timeStep;
        //clamp it by the maximum acceleration
        requiredAcel = Vector3.ClampMagnitude(requiredAcel, maxAcelForce);

        //calculate and return the result
        return (requiredAcel * rb.mass);
    }

    //function that will return the force required to move the ship acoording to the gravity acting on it 
    private Vector3 GetGravityForce()
    {
        //gravity
        Vector3 gravityAcel = Vector3.zero;

        CelestialBodyDeterministic[] gravityBodies = ShipMoveManager.GetGravityBodies();

        //loop thorugh them and add the approriate force from each
        foreach (var body in gravityBodies)
        {
            Vector3 direction = body.transform.position - rb.position;
            if (body.mass > 0 && direction.magnitude <= body.radius * manager.gravitySOIMultiplier)
            {
                float distanceSquared = direction.sqrMagnitude;
                Vector3 force = direction.normalized * manager.gravitationalConstant * rb.mass * body.mass / distanceSquared;

                //from the force calculate the accleration from newton's a = F/m formula
                Vector3 acel = force / rb.mass;

                //and add that to the collective aceleration
                gravityAcel += acel;
            }
        }

        return gravityAcel * rb.mass;
    }
}
