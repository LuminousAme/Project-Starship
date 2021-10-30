/*using System;
using System.Collections;

using System.Collections.Generic;
*/

using System.Collections.Generic;

using UnityEngine;

public class AIShip : MonoBehaviour
{
    //https://forum.unity.com/threads/random-flying-objects.30191/
    //https://gamedevelopment.tutsplus.com/series/understanding-steering-behaviors--gamedev-12732
    //https://www.red3d.com/cwr/steer/
    //https://forum.unity.com/threads/enemy-wander-behaviour-and-chasing-player-need-it-to-stop-chasing-if-it-runs-too-far.253204/
    //the ship's rigidbody
    private Rigidbody rb;

    private Quaternion targetRotation;
    public float rotSpeed = 5;

    private float wanderRange;
    private float timer;
    private bool flee;
    private bool wander;

    public List<GameObject> avoiding = new List<GameObject>();

    // public GameObject[] avoid;
    //public GameObject[] avoid1;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        wanderRange = 2500f;
        rb.velocity = new Vector3(1, 0, 1);
        targetRotation = transform.rotation;
        // Vector3 p = rb.transform.position + rb.velocity;
        //transform.LookAt(p);
        flee = false;
        wander = true;
        Wander();
        // avoid.Add(GameObject.FindGameObjectsWithTag("Planet"));
        //avoid.Add(GameObject.FindGameObjectsWithTag("Player"));
    }

    private void Start()
    {
        GameObject[] avoid = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] avoid1 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject aovidance in avoid)
        {
            avoiding.Add(aovidance);
            //Debug.Log(aovidance.transform.position.magnitude);
            //Debug.Log("avoid " + aovidance.name);
        }
        foreach (GameObject aovidance in avoid1)
        {
            avoiding.Add(aovidance);
            //Debug.Log("avoid " + aovidance.name);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        rb.velocity = transform.TransformDirection(Vector3.forward) * 15f;
        timer += Time.deltaTime;

        if (timer > 6f && wander)
        {
            Wander();
            timer = 0;
        }

        foreach (GameObject aovidance in avoiding)
        {
            //if ai ship is within player ship or planet flee so they won't collide
            float dist = Vector3.Distance(aovidance.transform.position, transform.position);
            if (/*aovidance.transform.position.magnitude*/ dist < 100)
            {
                Flee(aovidance.transform.position);
                wander = false;
            }
            else
            {
                wander = true;
            }

            //Debug.Log("Distance to  " + aovidance.name + " Distance:" + dist);
        }
    }

    private void Wander()
    {
        Vector3 point = rb.transform.position + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
        transform.LookAt(point);

        //Vector3 rotPoint = new Vector3(point.x, 0, point.z);

        //transform.rotation = Quaternion.LookRotation(rotPoint);

        //  targetRotation = Quaternion.LookRotation(rotPoint);
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);

        Debug.Log("Rotating: " + transform.rotation);
        Debug.Log("Wandering: " + point + " and " + (transform.position - point).magnitude);
    }

    private void Flee(Vector3 target)
    {
        //Vector3 desiredVelocity = Vector3.Scale(Vector3.Normalize(transform.position - target), new Vector3(15, 0, 15)) /* max_velocity*/;
        Vector3 desiredVelocity = Vector3.Scale((transform.position - target), new Vector3(15, 0, 15)) /* max_velocity*/;
        Vector3 steering = desiredVelocity - rb.velocity;

        transform.LookAt(steering);
        //targetRotation = Quaternion.LookRotation(steering);

        Debug.Log("Fleeing " + steering);
        //return steering;
    }
}