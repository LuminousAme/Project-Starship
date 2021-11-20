using UnityEngine;

public class ReleasePressure : MonoBehaviour
{
    public bool EnginesWorking = true;
    public bool releasing = false;

    //  public EnergyBar bar;
    private Renderer thisRend; //Renderer of our Cube

    private Material m_Material;
    private Material engineMaterial;
    public GameObject indicator;
    public GameObject engineIndicator;

    [SerializeField] private Handle pressureHandle;
    private bool lastHandleState;

    public float pressureLimit = 100f;
    public float pressure = 0f;
    [SerializeField] private float timeToNotWorking = 10f;
    private float timerForNotWorking;

    [SerializeField] private float buildUpRate = 1f;
    [SerializeField] private float releaseRate = 5f;

    [SerializeField] private Light[] lights;

    private int pressureState = 2, lastPressureState = 2; //0 is empty, 1 is too low, 2 is good, 3 is too much, 4 is full

    // Start is called before the first frame update
    private void Start()
    {
        thisRend = indicator.GetComponent<Renderer>();

        //Fetch the Material from the Renderer of the GameObject
        m_Material = thisRend.material;
        engineMaterial = engineIndicator.GetComponent<Renderer>().material;

        //last state starts the same as the handle, when they're different is used to detect the release/build up change
        lastHandleState = pressureHandle.GetHandleState();

        //timer for not working
        timerForNotWorking = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        //increase pressure over time
        if (!releasing) pressure = pressure + buildUpRate * Time.deltaTime;

        //if the pressure is too high increase the timer until the engine stops working
        if (pressure > 99f)
        {
            pressureState = 4;
            timerForNotWorking += Time.deltaTime;
        }
        //do the same if the pressure is too low
        else if (pressure < 1f)
        {
            pressureState = 0;
            timerForNotWorking += Time.deltaTime;
        }
        //but otherwise decrease it
        else
        {
            if (pressure < 20f) pressureState = 1;
            else if (pressure > 80f) pressureState = 3;
            else pressureState = 2;
            timerForNotWorking -= Time.deltaTime;
        }
        //finally clamp it between the max and 0
        timerForNotWorking = Mathf.Clamp(timerForNotWorking, 0f, timeToNotWorking);

        //if the player has moved the handle change if it's releasing or not
        if (lastHandleState != pressureHandle.GetHandleState())
        {
            releasing = !releasing;
            EnginesWorking = true;
            lastHandleState = pressureHandle.GetHandleState();
        }

        //code that handles releasing
        {
            //if the player pressed the button and is releasing pressure
            if (releasing)
            {
                m_Material.color = Color.green;

                pressure = pressure - releaseRate * Time.deltaTime;
            }
            //changes color back once valve is turned off
            else if (!releasing)
            {
                m_Material.color = Color.red;
            }
        }

        //if the engine has stopped working set that
        if (timerForNotWorking >= timeToNotWorking)
        {
            EnginesWorking = false;
        }

        //color change for the engines working indiciator
        if (!EnginesWorking)
        {
            engineMaterial.color = Color.black;
        }
        else
        {
            engineMaterial.color = Color.white;
        }

        //clamp the pressure
        pressure = Mathf.Clamp(pressure, 0.0f, pressureLimit);

        //update the notifications related to pressure
        if (pressureState != lastPressureState)
        {
            //if the pressure is good remove any notifcations
            if (pressureState == 2) NotificationSystem.instance.RemoveMessagesWithId(0);
            //if it's empty play that
            else if (pressureState == 0) NotificationSystem.instance.AddMessage(0, 2, "No Pressure");
            //if there is some pressure but not enough play that notifcation
            else if (pressureState == 1) NotificationSystem.instance.AddMessage(0, 1, "Pressure Levels Low");
            //if there is too much pressure but the tank is not yet full play that notifcation
            else if (pressureState == 3) NotificationSystem.instance.AddMessage(0, 1, "Pressure Levels High");
            //if the pressure tank is full play that message
            else if (pressureState == 4) NotificationSystem.instance.AddMessage(0, 2, "Pressure Levels Critical");
        }

        lastPressureState = pressureState;
    }
}