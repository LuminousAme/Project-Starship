using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//only use this if there is a ship movement component also attached
[RequireComponent(typeof(ShipMovement))]
public class ShipOrbit : MonoBehaviour
{
    //reference to the main movement script and rigidbody
    private ShipMovement sm;
    private Rigidbody rb;

    //if the orbiter is even active
    public bool active;

    //whichever planet is being orbited
    CelestialBody bodyBeingOrbitted;

    // Start is called before the first frame update
    void Start()
    {
        sm = this.GetComponent<ShipMovement>();
        rb = this.GetComponent<Rigidbody>();
        bodyBeingOrbitted = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        //when entering a trigger check if it's a celesital body, if it is start orbitting around it
        CelestialBody body = other.GetComponent<CelestialBody>();
        if (body != null)
        {
            bodyBeingOrbitted = body;
            active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CelestialBody>() != null)
        {
            bodyBeingOrbitted = null;
            active = false;
        }
    }
}
