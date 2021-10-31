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

    // Start is called before the first frame update
    private void Start()
    {
        //get the materials for the light indicators
        fanMaterial = fanIndicator.GetComponent<Renderer>().material;
        //fanMaterial.color = Color.green;
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

        //handle the player pressing buttons
        //if the player has pressed the fan button but it has been not been updated here, update it here
        if (!fan && fanButton.GetButtonState())
        {
            fan = true;
            liquid = false;
            liquidButton.SetButtonState(false);
            liquidNitrogen = false;
            nitrogrenButton.SetButtonState(false);
            //fanMaterial.color = Color.green;
            SetMatEmission(fanMaterial, true, Color.green, 1.45f);
            liquidMaterial.color = Color.white;
            SetMatEmission(liquidMaterial, false, Color.black);
            nitrogenMaterial.color = Color.white;
            SetMatEmission(nitrogenMaterial, false, Color.black);
        }

        //if the player has pressed the liquid cooling button but it has not been updated here, update it here
        if (!liquid && liquidButton.GetButtonState())
        {
            fan = false;
            fanButton.SetButtonState(false);
            liquid = true;
            liquidNitrogen = false;
            nitrogrenButton.SetButtonState(false);
            //liquidMaterial.color = Color.green;
            SetMatEmission(liquidMaterial, true, Color.green, 1.45f);
            fanMaterial.color = Color.white;
            SetMatEmission(fanMaterial, false, Color.black);
            nitrogenMaterial.color = Color.white;
            SetMatEmission(nitrogenMaterial, false, Color.black);
        }

        //if the player has pressed the liquid nitrogen cooling button but it has not been updated here, update it here
        if (!liquidNitrogen && nitrogrenButton.GetButtonState())
        {
            fan = false;
            fanButton.SetButtonState(false);
            liquid = false;
            liquidButton.SetButtonState(false);
            liquidNitrogen = true;
            //nitrogenMaterial.color = Color.green;
            SetMatEmission(nitrogenMaterial, true, Color.green, 1.45f);
            liquidMaterial.color = Color.white;
            SetMatEmission(liquidMaterial, false, Color.black);
            fanMaterial.color = Color.white;
            SetMatEmission(fanMaterial, false, Color.black);
        }

        //handle the player unpressing buttons
        if (fan && !fanButton.GetButtonState())
        {
            fan = false;
            fanMaterial.color = Color.white;
            SetMatEmission(fanMaterial, false, Color.black);
        }
        if (liquid && !liquidButton.GetButtonState())
        {
            liquid = false;
            liquidMaterial.color = Color.white;
            SetMatEmission(liquidMaterial, false, Color.black);
        }
        if (liquidNitrogen && !nitrogrenButton.GetButtonState())
        {
            liquidNitrogen = false;
            nitrogenMaterial.color = Color.white;
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
                Mathf.Lerp(1.55f, 1.4f, Mathf.PingPong(Time.time, 1f)));

            if (nitrogenMaterial.color == Color.green) { liquidMaterial.color = Color.white; }
            else
                nitrogenMaterial.color = Color.white;
        }
        else if (useLiquid && (!liquid))
        {
            heatCountdown = heatCountdown - 1f * Time.deltaTime;
            //liquidMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            SetMatEmission(liquidMaterial, true, Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)), 
                Mathf.Lerp(1.55f, 1.4f, Mathf.PingPong(Time.time, 1f)));

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
            //nitrogenMaterial.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)); //Color.red;
            SetMatEmission(nitrogenMaterial, true, Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f)),
                Mathf.Lerp(1.55f, 1.4f, Mathf.PingPong(Time.time, 1f)));

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

    private void SetMatEmission(Material mat, bool on, Color color, float intensity = 0f)
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