using UnityEngine;

public class Refuel : MonoBehaviour
{
    [SerializeField] private Handle fuelHandle;
    private bool lastHandleState;
    [SerializeField] private bool refueling;

    [SerializeField] private float fuelUseRate = 1f;
    [SerializeField] private float refuelRate = 3f;

    public GameObject fuelIndicator;
    private Material indicatorMaterial;

    public float blowUpThreshold = 120f;
    public float fuelWarning = 100f;
    public float fuel = 0f;

    [SerializeField] private bool shipBlowUp;
    [SerializeField] private bool fuelEmpty;

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
    }

    // Update is called once per frame
    private void Update()
    {
        //increment fuel
        if (!refueling)
        {
            //decrease fuel when not refueling
            fuel = fuel - Time.deltaTime * fuelUseRate;
        }
        else if (refueling)
        {
            //increase fuel when refueling
            fuel = fuel + Time.deltaTime * refuelRate;
        }

        //check if the enignes have fuel or not
        if (fuel <= 0f)
        {
            fuelEmpty = true;
            indicatorMaterial.color = Color.red;
        }
        else fuelEmpty = false;

        //when fuel gets too low or too high
        if ((fuel <= 20f || fuel > fuelWarning) && fuelEmpty == false)
        {
            //warning color
            indicatorMaterial.color = Color.yellow;
        }
        else if ((fuel > 20f || fuel < fuelWarning) && fuelEmpty == false)
        {
            //all good
            indicatorMaterial.color = Color.green;
        }

        if (fuel > blowUpThreshold)
        {
            indicatorMaterial.color = Color.red;
            shipBlowUp = true;
        }

        //if the player has moved the handle change if it's releasing or not
        if (lastHandleState != fuelHandle.GetHandleState())
        {
            refueling = !refueling;
            lastHandleState = fuelHandle.GetHandleState();
        }
    }

    public bool shouldShipBlow()
    {
        return shipBlowUp;
    }
}