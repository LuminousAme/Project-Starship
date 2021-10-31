using UnityEngine;

public class ReleasePressure : MonoBehaviour
{
    public bool EnginesWorking = true;
    public bool releasing = false;
    private bool playerClose = false;

    //  public EnergyBar bar;
    private Renderer thisRend; //Renderer of our Cube

    private Material m_Material;
    private Material engineMaterial;
    public GameObject indicator;
    public GameObject engineIndicator;

    public float pressureLimit = 100f;
    public float pressure = 0f;
    private float inputDelay = 0.25f;

    private bool canTask; // bool to see if player can do tasks rn
    private Energy playerEnergy;
    [SerializeField] private Light[] lights;

    // Start is called before the first frame update
    private void Start()
    {
        thisRend = indicator.GetComponent<Renderer>();

        //Fetch the Material from the Renderer of the GameObject
        m_Material = thisRend.material;
        engineMaterial = engineIndicator.GetComponent<Renderer>().material;
        //can do task
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
        inputDelay = inputDelay - Time.deltaTime;
        if (inputDelay < 0f)
        {
            inputDelay = 0f;
        }
        //increase pressure over time
        pressure = pressure + 1f * Time.deltaTime;

        if (canTask)
        {
            //check if player is close enough to act
            playerClose = CheckCloseTo("Player", 2);

            //if player is close eneough to act and they press the interact button "E" then they are releasing pressure
            if (playerClose && Input.GetKey(KeyCode.E) && inputDelay == 0f)
            {
                releasing = !releasing;
                EnginesWorking = true;
                inputDelay = 0.25f;
            }
        }
        //code that handles releasing
        {
            //if the player pressed the button and is releasing pressure
            if (releasing)
            {
                m_Material.color = Color.green;

                pressure = pressure - 5f * Time.deltaTime;
                if (pressure < 0f)
                {
                    pressure = 0f;
                }
            }
            //changes color back once valve is turned off
            else if (!releasing)
            {
                m_Material.color = Color.red;
            }
        }

        // if the player is releasing and the pressure is at 0 and they walk away with putting the valve back then the engines stop working
        if (releasing && !playerClose && pressure == 0f)
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

        //set pressure
        //  bar.SetValue(pressure);
    }

    //code to check if object with tag is close to this. from: https://answers.unity.com/questions/795190/checking-if-player-is-near-any-certain-gameobject.html
    private bool CheckCloseTo(string tag, float minimumDistance)
    {
        //GameObject[] goWithTag = GameObject.FindGameObjectsWithTag(tag);

        //for (int i = 0; i < goWithTag.Length; ++i)
        //{
        //    if (Vector3.Distance(transform.position, goWithTag[i].transform.position) <= minimumDistance)
        //    {
        //        return true;
        //    }
        //}
        GameObject checker = GameObject.FindGameObjectWithTag(tag);

        if (Vector3.Distance(transform.position, checker.transform.position) <= minimumDistance)
        {
            return true;
        }

        return false;
    }
}