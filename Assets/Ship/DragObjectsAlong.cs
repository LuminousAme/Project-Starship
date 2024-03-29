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

        MovePivotsOnStart();
    }

    //Update on the physics step
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

    //Update runs once per frame
    private void Update()
    {
        Vector3 change = rb.position - lastFramePos;

        //iterate over all of the bodies that should be dragged along 
        foreach (GameObject gameObject in objects)
        {
            //update the position
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            if (body == null) gameObject.transform.Translate(change, Space.World);

            //update the rotation
            gameObject.transform.rotation = rb.transform.rotation;
        }

        //update the last frame's position
        lastFramePos = rb.position;
    }

    private void MovePivotsOnStart()
    {
        //loop through all of the gameobjects
        foreach (GameObject obj in objects)
        {
            //get the difference between their pivot's acutal position and where it should be
            Vector3 diff = Vector3.zero;

            Rigidbody body = obj.gameObject.GetComponent<Rigidbody>();
            if (body != null)
            {
                diff = rb.position - body.position;
                body.MovePosition(body.position + diff);
            }
            else
            {
                diff = rb.position - obj.transform.position;
                obj.transform.Translate(diff);
            }

            //then move all of their children back to where they should be
            MoveChildrenOnStart(obj, diff);
        }
    }

    private void MoveChildrenOnStart(GameObject parent, Vector3 diff)
    {
        //get the children that need to be moved
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        //loop through them
        foreach (Transform child in children)
        {
            //if it is not a direct child just ignore it
            if (child.parent != parent.transform) continue;

            //move the child to the correct location
            Rigidbody body = child.gameObject.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.MovePosition(body.position - diff);
            }
            else
            {
                child.transform.Translate(-diff, Space.World);
            }
        }
    }
}