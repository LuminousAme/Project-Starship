using UnityEngine;
using UnityEngine.UI;

public class WarningHUD : MonoBehaviour
{
    public Text warning;
    public Color og;
    public ReleasePressure pressureTask;

    // Start is called before the first frame update
    private void Start()
    {
        warning.text = " ";
        og = warning.color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (pressureTask.pressure >= 75f)
        {
            warning.text = "\n PRESSURE LEVELS CRITICAL";
            warning.color = Color.Lerp(og, Color.clear, Mathf.PingPong(Time.time, 1.1f));
        }
        else
        {
            //warning.text += "\n     ";
            warning.text = warning.text.Replace("\n PRESSURE LEVELS CRITICAL", " ");
        }
    }
}