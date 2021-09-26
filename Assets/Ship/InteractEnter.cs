using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractEnter : MonoBehaviour
{
    //gameobject for the player who is going to be interacting
    private Transform player;

    //the transform for where the player should be standing and what way they should be facing to interact
    [SerializeField] private Transform interactTarget;

    //variable to control how fast the player moves and rotates after interacting
    [SerializeField] private float transitionLenght = 0.2f;
    private float transistionProgress = 0.0f;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool isBeginningInteraction;

    //all of the interactable objects that need to be activated
    [SerializeField] private Lever[] levers;
    [SerializeField] private Joystick[] joysticks;

    // Update is called once per frame
    void Update()
    {
        if (isBeginningInteraction) BeginInteracting();
    }

    //function that allows the player to take the wheel
    public void SetEntityInteracting(Transform entity)
    {
        //if the entity interacting is the player, save it and start the transition process
        if (entity != null)
        {
            player = entity;
            startPos = entity.transform.position;
            startRot = entity.transform.rotation;
            transistionProgress = 0.0f;
            isBeginningInteraction = true;

            foreach (var joystick in joysticks) joystick.SetInteractable(true);
            foreach (var lever in levers) lever.SetInteractable(true);
        }
        //otherwise set the player to null
        else
        {
            player = null;
            foreach (var joystick in joysticks) joystick.SetInteractable(false);
            foreach (var lever in levers) lever.SetInteractable(false);
        }
    }

    //function to move the player to the correct position and rotation for interacting
    private void BeginInteracting()
    {
        //update the trasnistion progress
        transistionProgress += Time.deltaTime;

        //find the t value 
        float t = Mathf.Clamp(transistionProgress / transitionLenght, 0.0f, 1.0f);

        //move player to the control panel
        player.position = Vector3.Lerp(startPos, interactTarget.position, t);
        //rotate the player towards the control panel
        player.rotation = Quaternion.Slerp(startRot, interactTarget.rotation, t);

        if (transistionProgress >= transitionLenght)
        {
            isBeginningInteraction = false;
        }
    }
}