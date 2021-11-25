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

    //0 is all good, 1 is should be fan but isn't, 2 is should be liquid but isn't, 3 is should be nitrogen but isn't
    private int coolantState = 0, lastCoolantState = 0; 

    // Start is called before the first frame update
    private void Start()
    {
        //get the materials for the light indicators
        fanMaterial = fanIndicator.GetComponent<Renderer>().material;
        SetMatEmission(fanMaterial, true, Color.green, 1.45f);
        liquidMaterial = liquidIndicator.GetComponent<Renderer>().material;
        SetMatEmission(liquidMaterial, false, Color.black);
        nitrogenMaterial = nitrogenIndicator.GetComponent<Renderer>().material;
        SetMatEmission(nitrogenMaterial, false, Color.black);

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
    }

    // Update is called once per frame
    private void Update()
    {
        //countdown the change timer
        changeTimer = changeTimer - 1f * Time.deltaTime;

        if (!fan && fanButton.GetButtonState())
        {
            fan = true;
            liquid = false;
            liquidButton.SetButtonState(false);
            liquidNitrogen = false;
            nitrogrenButton.SetButtonState(false);
            SetMatEmission(fanMaterial, true, Color.green, 1.45f);
            SetMatEmission(liquidMaterial, false, Color.black);
            SetMatEmission(nitrogenMaterial, false, Color.black);

            fanIndicator.GetComponent<AudioSource>().Play();
        }

        //if the player has pressed the liquid cooling button but it has not been updated here, update it here
        if (!liquid && liquidButton.GetButtonState())
        {
            fan = false;
            fanButton.SetButtonState(false);
            liquid = true;
            liquidNitrogen = false;
            nitrogrenButton.SetButtonState(false);
            SetMatEmission(liquidMaterial, true, Color.green, 1.45f);
            SetMatEmission(fanMaterial, false, Color.black);
            SetMatEmission(nitrogenMaterial, false, Color.black);

            liquidIndicator.GetComponent<AudioSource>().Play();
        }

        //if the player has pressed the liquid nitrogen cooling button but it has not been updated here, update it here
        if (!liquidNitrogen && nitrogrenButton.GetButtonState())
        {
            fan = false;
            fanButton.SetButtonState(false);
            liquid = false;
            liquidButton.SetButtonState(false);
            liquidNitrogen = true;
            SetMatEmission(nitrogenMaterial, true, Color.green, 1.45f);
            SetMatEmission(liquidMaterial, false, Color.black);
            SetMatEmission(fanMaterial, false, Color.black);

            nitrogenIndicator.GetComponent<AudioSource>().Play();
        }

        //handle the player unpressing buttons
        if (fan && !fanButton.GetButtonState())
        {
            fan = false;
            SetMatEmission(fanMaterial, false, Color.black);
        }
        if (liquid && !liquidButton.GetButtonState())
        {
            liquid = false;
            SetMatEmission(liquidMaterial, false, Color.black);
        }
        if (liquidNitrogen && !nitrogrenButton.GetButtonState())
        {
            liquidNitrogen = false;
            SetMatEmission(nitrogenMaterial, false, Color.black);
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
            //fanMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            SetMatEmission(fanMaterial, true, Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)),
                Mathf.Lerp(1.45f, 1.45f, Mathf.PingPong(Time.time, 1f)));

            coolantState = 1;
        }
        else if (useLiquid && (!liquid))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            //liquidMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            SetMatEmission(liquidMaterial, true, Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)), 
                Mathf.Lerp(1.45f, 1.45f, Mathf.PingPong(Time.time, 1f)));

            coolantState = 2;
        }
        else if (useLiquidNitrogen && (!liquidNitrogen))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            //nitrogenMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            SetMatEmission(nitrogenMaterial, true, Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)),
                Mathf.Lerp(1.45f, 1.45f, Mathf.PingPong(Time.time, 1f)));

            coolantState = 3;
        }
        else
        {
            coolantState = 0;

            //I have no idea what the next few things are doing
            if (heatCountdown < heatCountdownTime)
            {
                heatCountdown = heatCountdown + 3f * Time.deltaTime;
            }
            else
            {
                heatCountdown = heatCountdownTime;
            }
        } //else for wrong buttons ends here

        if (heatCountdown <= 0f)
        {
            engineExplode = true;
        }
        
        //if the state has changed update the notifcation system
        if(coolantState != lastCoolantState)
        {
            if (coolantState == 0) NotificationSystem.instance.RemoveMessagesWithId(1);
            else if (coolantState == 1) NotificationSystem.instance.AddMessage(1, 1, "Turn On Fan");
            else if (coolantState == 2) NotificationSystem.instance.AddMessage(1, 1, "Use Liquid Cooling");
            else if (coolantState == 3) NotificationSystem.instance.AddMessage(1, 1, "Swap to Liquid Nitrogen");
        }

        //update last coolantstate
        lastCoolantState = coolantState;
    }

    public static void SetMatEmission(Material mat, bool on, Color color, float intensity = 0f)
    {
        // for some reason, the desired intensity value (set in the UI slider) needs to be modified slightly for proper internal consumption
        float adjustedIntensity = intensity - (0.4169f);

        // redefine the color with intensity factored in - this should result in the UI slider matching the desired value
        color *= Mathf.Pow(2.0F, adjustedIntensity);

        if (on)
        {
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
            mat.SetColor("_EmissionColor", color);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}