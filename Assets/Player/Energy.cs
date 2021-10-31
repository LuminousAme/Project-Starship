using UnityEngine;

public class Energy : MonoBehaviour
{
    public EnergyBar bar;
    public bool AbleTask;
    public float energy = 15f;
    public float energyMax = 100f;
    private bool charging;
    private PlayerController playerC;

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
        Debug.Log("Tasking: " + playerC.GetInteracting());
        if (charging)
        {
            //increase energy if charging
            energy = energy + 5f * Time.deltaTime;
        }
        else if (energy > 0f && !playerC.GetInteracting())
        {
            //decrease energy over time if not charging
            energy = energy - .5f * Time.deltaTime;
        }
        else if (energy > 0f && playerC.GetInteracting())
        {
            //decrease energy over time if doing tasks
            energy = energy - 2f * Time.deltaTime;
            // Debug.Log("Tasking");
        }

        if (energy <= 0f)
        {
            energy = 0f;
            AbleTask = false;
        }
        else
        {
            AbleTask = true;
        }

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