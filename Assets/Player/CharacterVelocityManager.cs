using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterVelocityManager : MonoBehaviour
{
    //ship pull velocity
    [SerializeField] Rigidbody ship;
    private Vector3 playerMoveVelo = Vector3.zero;
    private Vector3 positionTargetFromRotation = Vector3.zero;
    private float timeStepSize = 0.0f;

    //the rigidbody component
    private Rigidbody rb;

    private void Start()
    {
        //grab the rigidbody of the attached gameobject
        rb = this.GetComponent<Rigidbody>();
    }

    //update every frame
    private void FixedUpdate()
    {
        //apply the velocity of the ship and the player's movement
        rb.velocity = ship.velocity + playerMoveVelo;
    }

    public void SetPlayerVelo(Vector3 newVelo)
    {
        playerMoveVelo = newVelo;
    }

    public void SetRotationalAdjustmentTargetPos(Vector3 pos, float timestep)
    {
        positionTargetFromRotation = pos;
        timeStepSize = timestep;
    }
}
