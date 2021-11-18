using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to keep the player ship (or player if they're walking around the external solar system near the origin
//based on this video https://youtu.be/jLi9oo413js
public class FloatingOrigin : MonoBehaviour
{
    //should be a singleton, there should never be more than one client size floating origin
    public static FloatingOrigin instance { get; private set; }

    //delegate functions other gameobjects can use in order to correct themselves
    public delegate void originCorrectionFunction(Vector3 correctionOffset);
    //the event through which it's invoked (observer pattern)
    public static event originCorrectionFunction onCorrection;

    //the target that should be near the origin, should normally be either the ship or the player whichever is the client side camera
    //in the wider solar system
    public Transform correctionTarget;

    //the maximum distance before a correction happens
    public float correctionDistance = 1000f;

    //track the client origin if there was no correction 
    //(this isn't super useful right now but will be essential for syncing up over multiplayer later)
    public Vector3 thisClientOrigin;

    //when this script is first awoken
    private void Awake()
    {
        //if an instance of the class already exists just delete this new one and exit the function
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //if the function hasn't exited then this must be the only instance so set it to the instance
        instance = this;

        //make sure the attached persists on load
        DontDestroyOnLoad(gameObject);
    }

    //update once a frame
    private void Update()
    {
        //check first that we have a correction target
        if (correctionTarget != null)
        {
            //if we do get a copy of the current position 
            Vector3 currentPos = correctionTarget.position;

            //and if it's futher than the correction distance do the correction
            if (currentPos.magnitude > correctionDistance)
            {
                //update the origin
                thisClientOrigin += currentPos;

                //send out the signal for other functions to correct themselves
                onCorrection?.Invoke(currentPos);

                //correct the objects position back to the origin
                correctionTarget.position = Vector3.zero;
            }
        }
    }

    public Vector3 GetThisClientOrigin() => thisClientOrigin;
    public Vector3 GetTargetObjectWorldPos() => thisClientOrigin + correctionTarget.position;
}
