using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private bool ObjectAlreadySelected;

    [SerializeField] private PlatformManager platform;

    // Update is called once per frame
    void Update()
    {
        if (platform.GetIsMobile())
            MobileUpdate();
        else
            DesktopUpdate();
    }

    //sets wheter or not an object has already been selected for interaction
    public void SetObjectAlreadySelected(bool alreadySelected)
    {
        ObjectAlreadySelected = alreadySelected;
    }

    private void DesktopUpdate()
    {
        if (!ObjectAlreadySelected)
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

                if (objectDetected.GetComponent<Interactables>() != null)
                {
                    objectDetected.GetComponent<Interactables>().mousedOver();
                }
            }
        }
    }

    private void MobileUpdate()
    {
        if (!ObjectAlreadySelected && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //fire a raycast out from the camera to the mouse position
            Ray ray = cam.ScreenPointToRay(touch.position);

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

                if (objectDetected.GetComponent<Interactables>() != null)
                {
                    objectDetected.GetComponent<Interactables>().touched();
                }
            }
        }
    }
}
