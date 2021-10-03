using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjectsAlong : MonoBehaviour
{
    //based on the following video https://youtu.be/PVtf3vg8BXw

    //list of all the rigidbody this object is dragging alone with it
    [SerializeField] private List<GameObject> objects;

    //the rigidbody that is acting as the base reference frame
    private Rigidbody rb;

    //the position of the rigidbody on the last frame
    private Vector3 lastPhysFramePos;
    private Vector3 lastFramePos;

    // Start is called before the first frame update
    void Start()
    {
        //grab the parent (ship)'s rigidbody
        rb = this.GetComponent<Rigidbody>();

        //save the last position
        lastPhysFramePos = rb.position;
        lastFramePos = rb.transform.position;
    }

    //Update on the physics frame
    private void FixedUpdate()
    {
        Vector3 change = rb.position - lastPhysFramePos;

        //iterate over all of the bodies that should be dragged along 
        foreach (GameObject gameObject in objects)
        {
            //update the position
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            if (body != null) body.MovePosition(body.position + change);
        }

        //update the last frame's position
        lastPhysFramePos = rb.position;
    }

    private void Update()
    {
        Vector3 change = rb.transform.position - lastFramePos;

        //iterate over all of the bodies that should be dragged along 
        foreach (GameObject gameObject in objects)
        {
            //update the position
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            if (body == null) gameObject.transform.Translate(change);

            /*
            //update the rotation
            Quaternion adjustedRot = Quaternion.FromToRotation(gameObject.transform.up, rb.transform.up) *
                Quaternion.FromToRotation(gameObject.transform.forward, rb.transform.forward) * 
                Quaternion.FromToRotation(gameObject.transform.right, rb.transform.right) *
                gameObject.transform.rotation;

            gameObject.transform.rotation = adjustedRot;*/
        }

        //update the last frame's position
        lastFramePos = rb.transform.position;
    }
}
