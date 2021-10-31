using UnityEngine;

public class Energy : MonoBehaviour
{
    public EnergyBar bar;
    public bool AbleTask;
    public float energy = 10f;
    public float energyMax = 100f;
    private bool charging;

    // Start is called before the first frame update
    private void Start()
    {
        charging = false;
        AbleTask = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (charging)
        {
            //increase energy if charging
            energy = energy + 4f * Time.deltaTime;
        }
        else if (energy > 0f)
        {
            //decrease energy over time if not charging
            energy = energy - .8f * Time.deltaTime;
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