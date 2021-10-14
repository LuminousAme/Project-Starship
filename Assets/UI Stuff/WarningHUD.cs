using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningHUD : MonoBehaviour
{
    public Text warning;
    public ReleasePressure pressureTask;

    // Start is called before the first frame update
    void Start()
    {
        warning.text = " ";
    }

    // Update is called once per frame
    void Update()
    {
        if (pressureTask.pressure >= 75f)
        {
            warning.text = "WARNING: PRESSURE LEVELS CRITICAL";
        }

        else
        {
            warning.text = " ";
        }
    }
}
