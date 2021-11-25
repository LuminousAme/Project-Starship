using System.Runtime.InteropServices;
using UnityEngine;

public class OrbitDLLScript : MonoBehaviour
{
    [DllImport("OrbitDLL")]
    private static extern float orbitMod();

    // Start is called before the first frame update
    private void Start()
    {
        this.GetComponent<CelestialBodyDeterministic>().semiMajorAxis = this.GetComponent<CelestialBodyDeterministic>().semiMajorAxis * orbitMod();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
