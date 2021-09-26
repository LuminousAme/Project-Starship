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

    //the ship's acutal velocity
    [SerializeField] private Vector3 currentVelo;

    //awake, runs when the object is first initliazed
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentVelo = new Vector3(0f, 0f, 0f);
        timeSinceDirChange = 0f;
        lastJoystickState = new Vector2Int(0, 0);
    }

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

        //update the time that has passed
        timeSinceDirChange += Time.deltaTime;
    }

    //called once per frame in the physics update, used to simulate the solar system gravity effect
    private void FixedUpdate()
    {
        //grab all of the celestial bodies
        CelestialBody[] bodies = ManagerForCelestialBodies.bodies;
        //loop thorugh them and add the approriate force from each
        foreach (var body in bodies)
        {
            //calculate force using newton's gravity formula
            Vector3 direction = body.transform.position - rb.position;
            float distanceSquared = direction.sqrMagnitude;
            Vector3 force = direction.normalized * CelestialBody.gravitationalConstant * body.GetMass() / distanceSquared;

            //apply that force to the ship's rigidbody
            rb.AddForce(force, ForceMode.Acceleration);
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

        //adding the acceleration
        currentSpeed = Mathf.Clamp(currentSpeed + deltaSpeed * Time.fixedDeltaTime, 0.0f, maxSpeed);

        //handling the rotation
        float horiRot = dirJoystick.GetJoystickState().x * rotateRate;
        float vertRot = dirJoystick.GetJoystickState().y * rotateRate;

        transform.Rotate(0f, horiRot, 0f, Space.Self);
        transform.Rotate(0f, 0f, -vertRot, Space.Self);

        //add to the movement
        rb.AddForce(currentSpeed * transform.forward, ForceMode.Acceleration);

        //rb.position += currentVelo * Time.fixedDeltaTime;
    }
}
