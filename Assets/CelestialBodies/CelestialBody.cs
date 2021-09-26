using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Celesital body simulation script, inspired by Sebastian Lague's Solar System simulation 

//Celestial Bodies must have rigidbodies
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : MonoBehaviour
{
    //data for the celesital body such as mass, radius, and inital velocity
    static public float gravitationalConstant = 0.2f;
    [SerializeField] private float surfaceGravity = 10f;
    [SerializeField] private Vector3 initialVelocity;

    //private data it needs during execute
    private Vector3 currentVelocity;
    private float mass;
    private Rigidbody rb;
    private float radius;

    //awake, runs when the object is first initliazed
    private void Awake()
    {
        //grab the rigidbody component, get the velocity, and calculate the mass and radius
        rb = GetComponent<Rigidbody>();
        currentVelocity = initialVelocity;
        radius = 0.5f * transform.localScale.x;
        mass = surfaceGravity * radius * radius / gravitationalConstant;
        rb.mass = mass;
    }

    //function to update the body's velocity, will be called by a seperate manager script
    public void UpdateVelocity(CelestialBody[] bodies)
    {
        //loop through all of the celestial bodies in the scene and figure out this body's current velocity based on their gravity
        foreach (var body in bodies)
        {
            if (body != this)
            {
                //calculate force using newton's F = G(m1 * m2 / r^2) formula
                Vector3 direction = body.rb.position - rb.position;
                float distanceSquared = direction.sqrMagnitude;
                Vector3 force = direction.normalized * gravitationalConstant * mass * body.mass / distanceSquared;

                //from the force calculate the accleration from newton's a = F/m formula
                Vector3 acel = force / mass;

                //add that acceleration to the current velocity
                currentVelocity += acel * Time.fixedDeltaTime;
            }
        }
    }

    //function to update the body's position, will be called by a seperate manager script
    public void UpdatePosition()
    {
        rb.position += currentVelocity * Time.fixedDeltaTime;
    }

    public float GetMass()
    {
        return mass;
    }
}