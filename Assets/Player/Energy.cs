using UnityEngine;

public class Energy : MonoBehaviour
{
    public EnergyBar bar;
    public bool AbleTask;
    public float energy = 15f;
    public float energyMax = 100f;
    private bool charging;
    private PlayerController playerC;

    [SerializeField] private float energyGainRate = 5f;
    [SerializeField] private float energyLossRate = 0.5f;
    [SerializeField] private float energyTaskMultiplier = 4f;
    

    // Start is called before the first frame update
    private void Start()
    {
        charging = false;
        AbleTask = true;
        playerC = GetComponent<PlayerController>();
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
            //decrease energy over time if doing tasks
            energy = energy - (energyLossRate * energyTaskMultiplier) * Time.deltaTime;
        }

        //set if it's possible to do tasks
        AbleTask = (energy <= 0f) ? false : true;

        //clamp the energy
        energy = Mathf.Clamp(energy, 0, energyMax);

        //sent the energy level to the hud
        bar.SetValue(energy);
    }

    private void OnTriggerEnter(Collider other)
    {
        Charger charge = other.gameObject.GetComponent<Charger>();
        if (charge)
        {
            charging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Charger charge = other.gameObject.GetComponent<Charger>();
        if (charge)
        {
            charging = false;
        }
    }

    public bool GetAble()
    {
        return AbleTask;
    }

    public float GetEnergy()
    {
        return energy;
    }
}