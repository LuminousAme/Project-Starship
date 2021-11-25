using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider bar;
    public Energy energyVal;

    // Start is called before the first frame update
    private void Start()
    {
        //pressureTask = GameObject.FindGameObjectWithTag("Tasks").GetComponent<ReleasePressure>();
        //bar = GetComponent<Slider>();
        //bar.maxValue = pressureTask.pressureLimit;
        //bar.value = pressureTask.pressure;
        energyVal = GameObject.FindGameObjectWithTag("Player").GetComponent<Energy>();
        bar = GetComponent<Slider>();
        bar.maxValue = energyVal.energyMax;
        bar.value = energyVal.energy;
    }

    public void SetValue(float pr)
    {
        bar.value = pr;
    }
}