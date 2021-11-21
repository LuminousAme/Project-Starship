using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    //the energy bar showing the player what their energy level is
    public EnergyBar bar;

    //boolean for if they have enough energy to do a task
    public bool AbleTask;

    //data for charging
    public float energy = 80f;

    public float energyMax = 100f;
    private bool charging;

    //the player controller
    private PlayerController playerC;

    //controls for how energy changes over time
    [SerializeField] private float energyGainRate = 5f;

    [SerializeField] private float energyLossRate = 0.5f;
    [SerializeField] private float energyTaskMultiplier = 4f;

    [SerializeField] private UnityEngine.UI.Button mobileInteractButton;

    [Space]
    [SerializeField] private bool energySystemOff = false;

    // Start is called before the first frame update
    private void Start()
    {
        charging = false;
        AbleTask = true;
        playerC = GetComponent<PlayerController>();
        mobileInteractButton.interactable = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (charging)
        {
            //increase energy if charging
            energy = energy + energyGainRate * Time.deltaTime;
        }
        else if (energy > 0f && !playerC.GetInteracting())
        {
            //decrease energy over time if not charging
            energy = energy - energyLossRate * Time.deltaTime;
        }
        else if (energy > 0f && playerC.GetInteracting())
        {
            //decrease energy over time by an additional multipler if doing tasks
            energy = energy - (energyLossRate * energyTaskMultiplier) * Time.deltaTime;
        }

        if (energySystemOff) energy = 100f;

        //set if it's possible to do tasks
        AbleTask = (energy <= 0f) ? false : true;
        mobileInteractButton.interactable = AbleTask;

        //clamp the energy
        energy = Mathf.Clamp(energy, 0, energyMax);

        //sent the energy level to the hud
        bar.SetValue(energy);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player enters the charging area begin charging
        Charger charge = other.gameObject.GetComponent<Charger>();
        if (charge)
        {
            charging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if the player exits the charging area begin charging
        Charger charge = other.gameObject.GetComponent<Charger>();
        if (charge)
        {
            charging = false;
        }
    }

    public float GetEnergy()
    {
        return energy;
    }
}