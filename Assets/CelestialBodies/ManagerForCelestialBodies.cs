using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerForCelestialBodies : MonoBehaviour
{
    static public CelestialBody[] bodies;

    //awake, runs when the object is first initliazed
    private void Awake()
    {
        //find and save references to all the celestial bodies
        bodies = FindObjectsOfType<CelestialBody>();
    }

    //update, runs once a frame, used to update all of the bodies
    private void Update()
    {
        //loop through all of the bodies and update their velocities
        for(int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdateVelocity(bodies);
        }

        //loop through all of the bodies and update their positions
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition();
        }
    }
}