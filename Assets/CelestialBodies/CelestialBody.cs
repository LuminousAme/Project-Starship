using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Celesital body simulation script, inspired by Sebastian Lague's Solar System simulation 
public class CelestialBody : MonoBehaviour
{
    //data for the celesital body such as mass, radius, and inital velocity
    static public float gravitationalConstant = 0.2f;
    [SerializeField] private float surfaceGravity = 10f;
    [SerializeField] private Vector3 initialVelocity;
    static private CelestialBody sun;
    private bool firstFrame = true;

    //private data it needs during execute
    private Vector3 currentVelocity;
    [SerializeField] private float mass;
    private float radius;

    //awake, runs when the object is first initliazed
    private void Awake()
    {
        //get the velocity, and calculate the mass and radius
        currentVelocity = Vector3.zero;
        radius = 0.5f * transform.localScale.x;
        mass = surfaceGravity * radius * radius / gravitationalConstant;
        if (this.name == "Sun") sun = this;
        firstFrame = true;
    }

    //function to update the body's velocity, will be called by a seperate manager script
    public void UpdateVelocity(CelestialBody[] bodies)
    {
        //do not move the sun because of gravity, keep it in place
        if(this != sun)
        {
            //on the first frame, programmatically calculate the actual initial velocity
            if (firstFrame) StartVelocityOnFirstFrame();

            //loop through all of the celestial bodies in the scene and figure out this body's current velocity based on their gravity
            foreach (var body in bodies)
            {
                if (body != this)
                {
                    //calculate force using newton's F = G(m1 * m2 / r^2) formula
                    Vector3 direction = body.transform.position - transform.position;
                    float distanceSquared = direction.sqrMagnitude;
                    Vector3 force = direction.normalized * gravitationalConstant * mass * body.mass / distanceSquared;

                    //from the force calculate the accleration from newton's a = F/m formula
                    Vector3 acel = force / mass;

                    //add that acceleration to the current velocity
                    currentVelocity += acel * Time.deltaTime;
                }
            }
        }
    }

    //function to update the body's position, will be called by a seperate manager script
    public void UpdatePosition()
    {
        transform.Translate(currentVelocity * Time.deltaTime);
    }

    public float GetMass()
    {
        return mass;
    }

    private void StartVelocityOnFirstFrame()
    {
        firstFrame = false;

        Vector3 direction = sun.transform.position - transform.position;
        float distanceSquared = direction.sqrMagnitude;
        Vector3 force = direction.normalized * gravitationalConstant * mass * sun.mass / distanceSquared;

        //get the magnitude
        float forceMag = force.magnitude;
        //use that to caclulate the speed
        float speed = Mathf.Sqrt((forceMag * direction.magnitude) /mass);

        //and set the starting velocity from that
        currentVelocity = initialVelocity.normalized * speed;
    }
}