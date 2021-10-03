using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyBar : MonoBehaviour
{

    public Slider bar;
    public ReleasePressure pressureTask;


    // Start is called before the first frame update
    void Start()
    {

        pressureTask = GameObject.FindGameObjectWithTag("Tasks").GetComponent<ReleasePressure>();
        bar = GetComponent<Slider>();
        bar.maxValue = pressureTask.pressureLimit;
        bar.value = pressureTask.pressure;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetPressure(float pr)
    {
        bar.value = pr;
    }
}
