using System.Collections.Generic;
using UnityEngine;

//we should probably build an object pool for the AI ships eventually
public class AIShip : MonoBehaviour
{
    //https://forum.unity.com/threads/random-flying-objects.30191/
    //https://gamedevelopment.tutsplus.com/series/understanding-steering-behaviors--gamedev-12732
    //https://www.red3d.com/cwr/steer/
    //https://forum.unity.com/threads/enemy-wander-behaviour-and-chasing-player-need-it-to-stop-chasing-if-it-runs-too-far.253204/
    //the ship's rigidbody
    private Rigidbody rb;

    //rotation data
    private Quaternion targetRotation;
    public float rotSpeed = 12f;
    public float shipSpeed = 20f;

    private float wanderRange;
    private float timer;
    private bool wander;
    private Vector3 rotPoint;

    public List<GameObject> avoiding = new List<GameObject>();

    //the particle systems
    [SerializeField] private ParticleSystem rightThruster, leftThruster;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        wanderRange = 2500f;
        rb.velocity = new Vector3(1, 0, 1);
        targetRotation = transform.rotation;
        wander = true;
        Wander();
    }

    private void Start()
    {
        //when the ship is first spawned 
        GameObject[] avoid = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] avoid1 = GameObject.FindGameObjectsWithTag("PlayerShip");
        foreach (GameObject aovidance in avoid)
        {
            avoiding.Add(aovidance);
        }
        foreach (GameObject aovidance in avoid1)
        {
            avoiding.Add(aovidance);
        }

        Transform psSimSpace = GameObject.Find("CelestialBodies").transform;

        var leftMain = leftThruster.main;
        leftMain.simulationSpace = ParticleSystemSimulationSpace.Custom;
        leftMain.customSimulationSpace = psSimSpace;

        var rightmain = rightThruster.main;
        rightmain.simulationSpace = ParticleSystemSimulationSpace.Custom;
        rightmain.customSimulationSpace = psSimSpace;
    }

    // Update is called once per frame
    private void Update()
    {
        rb.velocity = transform.TransformDirection(Vector3.forward) * shipSpeed;
        timer += Time.deltaTime;

        if (timer >= 6f && wander)
        {
            Wander();
            timer = 0;
        }
        if (wander)
        {
            targetRotation = Quaternion.LookRotation(rotPoint);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }

        foreach (GameObject aovidance in avoiding)
        {
            //if ai ship is within player ship or planet flee so they won't collide
            float dist = Vector3.Distance(aovidance.transform.position, transform.position);
            if (/*aovidance.transform.position.magnitude*/ dist < 240)
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
        Vector3 point = rb.transform.position * 2 + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
        // transform.LookAt(point);
        //transform.rotation = Quaternion.LookRotation(rotPoint);

        rotPoint = new Vector3(point.x, 0, point.z);
        //Debug.Log("Rotating: " + rotPoint);

        //Debug.Log("Wandering: " + point + " and " + transform.position/* - point).magnitude*/);
    }

    private void Flee(Vector3 target)
    {
        //Vector3 desiredVelocity = Vector3.Scale(Vector3.Normalize(transform.position - target), new Vector3(15, 0, 15)) /* max_velocity*/;
        Vector3 desiredVelocity = Vector3.Scale((transform.position - target), new Vector3(15, 0, 15)) /* max_velocity*/;
        Vector3 steering = desiredVelocity - rb.velocity;

        //transform.LookAt(steering);
        targetRotation = Quaternion.LookRotation(steering);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);

        //Debug.Log("Fleeing " + steering);
        //return steering;
    }


}