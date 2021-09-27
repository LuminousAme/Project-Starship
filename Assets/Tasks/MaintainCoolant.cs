using System.Collections.Generic;

//using System.Linq; // allows to compare contents of two arraws with a built-in function (SequenceEqual)
using UnityEngine;

public class MaintainCoolant : MonoBehaviour
{
    public float heatCountdownTime = 7f; //how much time you have to switch coolants before soemtihng bad happens
    public float changeTime = 8f; //the time interval it takes for a new cooling method to be needed

    public float heatCountdown; //timer to switch coolants before soemtihng bad happens
    public float changeTimer; //the timer for a new cooling method to be needed

    public bool engineExplode = false;
    public bool engineStall = false;

    //coolant options
    public bool fan = true;

    public bool liquid = false;
    public bool liquidNitrogen = false;

    // public bool[] cooling = new bool[3]; // = { fan, liquid, liquidNitrogen };
    private List<bool> coolingMethods = new List<bool>();

    //bools for game to choose which one to tell player to use
    public bool useFan = false;

    public bool useLiquid = false;
    public bool useLiquidNitrogen = false;

    //material for ligths/inidicators
    private Material fanMaterial;

    private Material liquidMaterial;
    private Material nitrogenMaterial;
    public GameObject fanIndicator;
    public GameObject liquidIndicator;
    public GameObject nitrogenIndicator;

    // private bool playerClose; //bool to detemrine if player is in vicinity to actviate the coolants
    private float inputDelay = 0.25f;

    public float methodTracker = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        //get the materials for the light indicators
        fanMaterial = fanIndicator.GetComponent<Renderer>().material;
        fanMaterial.color = Color.green;
        liquidMaterial = liquidIndicator.GetComponent<Renderer>().material;
        nitrogenMaterial = nitrogenIndicator.GetComponent<Renderer>().material;

        heatCountdown = heatCountdownTime;
        changeTimer = changeTime;
        //fill the bool array
        coolingMethods.Add(useFan);
        coolingMethods.Add(useLiquid);
        coolingMethods.Add(useLiquidNitrogen);
        coolingMethods[0] = true;
        //useFan = coolingMethods[0];
        //useLiquid = coolingMethods[1];
        //useLiquidNitrogen = coolingMethods[2];
    }

    // Update is called once per frame
    private void Update()
    {
        inputDelay = inputDelay - Time.deltaTime;
        if (inputDelay < 0f)
        {
            inputDelay = 0f;
        }

        //countdown the change timer
        changeTimer = changeTimer - 1f * Time.deltaTime;

        // playerClose = CheckCloseTo("Player", fanIndicator, 2);
        //if player is close eneough to the fan to act and they press the interact button "E" then they are using fans
        if (CheckCloseTo("Player", fanIndicator, 2) && Input.GetKey(KeyCode.E) && inputDelay == 0f)
        {
            fan = true;
            liquid = false;
            liquidNitrogen = false;
            fanMaterial.color = Color.green;
            liquidMaterial.color = Color.white;
            nitrogenMaterial.color = Color.white;
            inputDelay = 0.25f;
        }

        if (CheckCloseTo("Player", liquidIndicator, 2) && Input.GetKey(KeyCode.E) && inputDelay == 0f)
        {
            fan = false;
            liquid = true;
            liquidNitrogen = false;
            liquidMaterial.color = Color.green;
            fanMaterial.color = Color.white;
            nitrogenMaterial.color = Color.white;
            inputDelay = 0.25f;
        }

        if (CheckCloseTo("Player", nitrogenIndicator, 2) && Input.GetKey(KeyCode.E) && inputDelay == 0f)
        {
            fan = false;
            liquid = false;
            liquidNitrogen = true;
            nitrogenMaterial.color = Color.green;
            liquidMaterial.color = Color.white;
            fanMaterial.color = Color.white;
            inputDelay = 0.25f;
        }

        //if the change timer is 0 then there will be a new coolant needed
        if (changeTimer <= 0f)
        {
            //keeps track of previous option
            for (int i = 0; i < coolingMethods.Count; i++)
            {
                if (coolingMethods[i] == true)
                {
                    methodTracker = i;
                    coolingMethods[i] = false;
                }
            }

            //random option
            int choice = Random.Range(0, coolingMethods.Count);

            //if option is the same then it will find another
            do
            {
                choice = Random.Range(0, coolingMethods.Count);
            }

            while (choice == methodTracker);

            coolingMethods[choice] = true;
            useFan = coolingMethods[0];
            useLiquid = coolingMethods[1];
            useLiquidNitrogen = coolingMethods[2];
            changeTimer = changeTime;
        }

        if (useFan && (!fan))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            fanMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;

            if (nitrogenMaterial.color == Color.green) { liquidMaterial.color = Color.white; }
            else
                nitrogenMaterial.color = Color.white;
        }
        else if (useLiquid && (!liquid))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            liquidMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;

            if (fanMaterial.color == Color.green)
            {
                nitrogenMaterial.color = Color.white;
            }
            else
                fanMaterial.color = Color.white;
        }
        else if (useLiquidNitrogen && (!liquidNitrogen))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            nitrogenMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            if (fanMaterial.color == Color.green)
            {
                liquidMaterial.color = Color.white;
            }
            else
                fanMaterial.color = Color.white;
        }
        else
        {
            if (heatCountdown < heatCountdownTime)
            {
                heatCountdown = heatCountdown + 3f * Time.deltaTime;
            }
            else
            {
                heatCountdown = heatCountdownTime;
            }
        }
        if (heatCountdown <= 0f)
        {
            engineExplode = true;
        }
    }

    //code to check if object with tag is close to a game object. from: https://answers.unity.com/questions/795190/checking-if-player-is-near-any-certain-gameobject.html
    private bool CheckCloseTo(string tag, GameObject thing, float minimumDistance)
    {
        GameObject checker = GameObject.FindGameObjectWithTag(tag);

        if (Vector3.Distance(thing.transform.position, checker.transform.position) <= minimumDistance)
        {
            return true;
        }

        return false;
    }
}