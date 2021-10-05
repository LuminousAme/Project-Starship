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
        Vector3 change = rb.position - lastFramePos;

        //iterate over all of the bodies that should be dragged along 
        foreach (GameObject gameObject in objects)
        {
            //update the position
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            if (body == null) gameObject.transform.Translate(change, Space.World);

            
            //update the rotation
            Quaternion adjustedRot = Quaternion.FromToRotation(gameObject.transform.up, rb.transform.up) *
                Quaternion.FromToRotation(gameObject.transform.forward, rb.transform.forward) * 
                Quaternion.FromToRotation(gameObject.transform.right, rb.transform.right) *
                gameObject.transform.rotation;

            gameObject.transform.rotation = adjustedRot;
        }

        //update the last frame's position
        lastFramePos = rb.position;
    }

    private void MovePivotsOnStart()
    {
        foreach (GameObject obj in objects)
        {
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

            MoveChildrenOnStart(obj, diff);
        }
    }

    private void MoveChildrenOnStart(GameObject parent, Vector3 diff)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.parent != parent.transform) continue;

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