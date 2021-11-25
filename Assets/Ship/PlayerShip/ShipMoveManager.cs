using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipMoveManager : MonoBehaviour
{
    private ShipFreeSpaceMove freeSpaceMove;
    private ShipLandingMovement landingMove;

    //all of the celestial bodies that may apply gravity
    private static CelestialBodyDeterministic[] gravityBodies;
    public float gravitySOIMultiplier = 1.5f;
    public float gravitationalConstant = 0.2f;

    //button that will takeoff or land
    [SerializeField] private Button landingButton; 

    private void Start()
    {
        //get all of the movement scripts
        freeSpaceMove = this.GetComponent<ShipFreeSpaceMove>();
        freeSpaceMove.enabled = true;
        landingMove = this.GetComponent<ShipLandingMovement>();
        landingMove.enabled = false;

        //get all of the bodies that might be able to apply gravity to the player's ship
        gravityBodies = FindObjectsOfType<CelestialBodyDeterministic>();
    }

    public static CelestialBodyDeterministic[] GetGravityBodies() => gravityBodies;

    //some functions to transition between the different movemement modes
    public void BeginLanding(CelestialBodyDeterministic targetBody)
    {
        //disable the free flying movement script
        freeSpaceMove.enabled = false;
        //set the target body to land on and enable it
        landingMove.SetClosetBody(targetBody);
        landingMove.enabled = true;
        landingMove.StartLanding();
    }

    public void EndTakeOff()
    {
        //disable the landing movement script
        landingMove.enabled = false;
        //set the free fly to active
        freeSpaceMove.enabled = true;
    }

    public bool GetButtonState() => landingButton.GetButtonState();

    public void SetButtonState(bool state)
    {
        landingButton.SetButtonState(state);
    }
}
