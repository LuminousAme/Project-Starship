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

    [SerializeField] private Button fanButton;
    [SerializeField] private Button liquidButton;
    [SerializeField] private Button nitrogrenButton;

    public float methodTracker = 0f;

    private bool canTask; // bool to see if player can do tasks rn
    private Energy playerEnergy;

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

        fanButton.SetButtonState(true);
        fan = fanButton.GetButtonState();
        liquid = liquidButton.GetButtonState();
        liquidNitrogen = nitrogrenButton.GetButtonState();

        canTask = true;
        GameObject play = GameObject.FindGameObjectWithTag("Player");
        playerEnergy = play.GetComponent<Energy>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerEnergy.AbleTask == false)
        {
            canTask = false;
        }
        else { canTask = true; }

        //countdown the change timer
        changeTimer = changeTimer - 1f * Time.deltaTime;

        if (canTask)
        {
            //handle the player pressing buttons
            //if the player has pressed the fan button but it has been not been updated here, update it here
            if (!fan && fanButton.GetButtonState())
            {
                fan = true;
                liquid = false;
                liquidButton.SetButtonState(false);
                liquidNitrogen = false;
                nitrogrenButton.SetButtonState(false);
                fanMaterial.color = Color.green;
                liquidMaterial.color = Color.white;
                nitrogenMaterial.color = Color.white;
            }

            //if the player has pressed the liquid cooling button but it has not been updated here, update it here
            if (!liquid && liquidButton.GetButtonState())
            {
                fan = false;
                fanButton.SetButtonState(false);
                liquid = true;
                liquidNitrogen = false;
                nitrogrenButton.SetButtonState(false);
                liquidMaterial.color = Color.green;
                fanMaterial.color = Color.white;
                nitrogenMaterial.color = Color.white;
            }

            //if the player has pressed the liquid nitrogen cooling button but it has not been updated here, update it here
            if (!liquidNitrogen && nitrogrenButton.GetButtonState())
            {
                fan = false;
                fanButton.SetButtonState(false);
                liquid = false;
                liquidButton.SetButtonState(false);
                liquidNitrogen = true;
                nitrogenMaterial.color = Color.green;
                liquidMaterial.color = Color.white;
                fanMaterial.color = Color.white;
            }

            //handle the player unpressing buttons
            if (fan && !fanButton.GetButtonState())
            {
                fan = false;
                fanMaterial.color = Color.white;
            }
            if (liquid && !liquidButton.GetButtonState())
            {
                liquid = false;
                liquidMaterial.color = Color.white;
            }
            if (liquidNitrogen && !nitrogrenButton.GetButtonState())
            {
                liquidNitrogen = false;
                nitrogenMaterial.color = Color.white;
            }
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
}