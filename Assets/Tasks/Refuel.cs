using UnityEngine;
using UnityEngine.SceneManagement;

public class Refuel : MonoBehaviour
{
    [SerializeField] private Handle fuelHandle;
    private bool lastHandleState;
    [SerializeField] private bool refueling;

    [SerializeField] private float fuelUseRate = 1f;
    private float fuelUseMod;
    [SerializeField] private float refuelRate = 3f;
    private float refuelMod;

    public GameObject fuelIndicator;
    private Material indicatorMaterial;

    public float blowUpThreshold = 120f;
    public float fuelWarning = 100f;
    public float fuel = 0f;

    [SerializeField] private bool shipBlowUp;
    [SerializeField] private bool fuelEmpty;

    private int fuelState = 2, lastFuelState = 2; //0 is empty, 1 is too low, 2 is good, 3 is too high

    // Start is called before the first frame update
    private void Start()
    {
        //last state starts the same as the handle, when they're different is used to detect the release/build up change
        lastHandleState = fuelHandle.GetHandleState();

        //get material
        indicatorMaterial = fuelIndicator.GetComponent<Renderer>().material;
        //max out fuel on ship start
        fuel = fuelWarning;
        refueling = false;
        shipBlowUp = false;

        fuelUseMod = DifficultyMod.GetMultiplier("FuelUseRate");
        refuelMod = DifficultyMod.GetMultiplier("RefuelRate");
    }

    // Update is called once per frame
    private void Update()
    {
        //increment fuel
        if (!refueling)
        {
            //decrease fuel when not refueling
            fuel = fuel - Time.deltaTime * (fuelUseMod * fuelUseRate);
        }
        else if (refueling)
        {
            //increase fuel when refueling
            fuel = fuel + Time.deltaTime * (refuelMod * refuelRate);
        }

        //check if the enignes have fuel or not
        if (fuel <= 0f)
        {
            fuelEmpty = true;
            indicatorMaterial.color = Color.red;
            fuel = 0f;
            fuelState = 0;
        }
        else fuelEmpty = false;

        //when fuel gets too low or too high
        if ((fuel <= 20f || fuel > fuelWarning) && fuelEmpty == false)
        {
            //warning color
            indicatorMaterial.color = Color.yellow;
            fuelState = (fuel <= 20f) ? 1 : 3;
        }
        else if ((fuel > 20f || fuel < fuelWarning) && fuelEmpty == false)
        {
            //all good
            indicatorMaterial.color = Color.green;
            fuelState = 2;
        }

        if (fuel > blowUpThreshold)
        {
            indicatorMaterial.color = Color.red;
            shipBlowUp = true;
        }

        fuel = Mathf.Clamp(fuel, 0, blowUpThreshold + 1.0f);

        //if the player has moved the handle change if it's releasing or not
        if (lastHandleState != fuelHandle.GetHandleState())
        {
            refueling = !refueling;
            lastHandleState = fuelHandle.GetHandleState();
        }

        if (shipBlowUp)
        {
            SceneManager.LoadScene("End Menu");
        }

        //update the notifications related to fuel
        if (fuelState != lastFuelState)
        {
            //if the fuel is good remove any notifcations
            if (fuelState == 2) NotificationSystem.instance.RemoveMessagesWithId(5);
            //if it's empty play that
            else if (fuelState == 0) NotificationSystem.instance.AddMessage(5, 2, "Fuel Tank Empty");
            //if the fuel is running low play that
            else if (fuelState == 1) NotificationSystem.instance.AddMessage(5, 1, "Low Fuel");
            //if there is too much fuel play that
            else if (fuelState == 3) NotificationSystem.instance.AddMessage(5, 2, "Fuel Levels Critical");
        }

        lastFuelState = fuelState;
    }

    public bool shouldShipBlow()
    {
        return shipBlowUp;
    }
}