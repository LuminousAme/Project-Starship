using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    // Update is called once per frame
    void Update()
    {
        //fire a raycast out from the camera to the mouse position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //set up a variable to take the information from the result of the raycast
        RaycastHit rayhitdata;

        //set up the bitmask so it only raycasts to interactables
        int layermask = 1 << 6;

        //if the ray has hit an interactable object
        if (Physics.Raycast(ray, out rayhitdata, Mathf.Infinity, layermask))
        {
            //grab the gameobject that has been hit
            // (note this HAS to be .collider here, for some reason trying to access the gameobject from the transform or rigidbody 
            // returns the wrong object, I have no idea why, I wasted several hours on this - Ame)
            GameObject objectDetected = rayhitdata.collider.gameObject;

            if (objectDetected.GetComponent<Lever>() != null)
            {
                objectDetected.GetComponent<Lever>().mousedOver();
            }
            else if (objectDetected.GetComponent<Joystick>() != null)
            {
                objectDetected.GetComponent<Joystick>().mousedOver();
            }
        }
    }
}
