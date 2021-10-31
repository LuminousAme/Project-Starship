using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractEnter : MonoBehaviour
{
    //gameobject for the player who is going to be interacting
    private Transform player;
    private Transform playerCamera;

    //the transform for where the player should be standing and what way they should be facing to interact
    [SerializeField] private Transform interactTarget;
    [SerializeField] private Transform cameraInteractTarget;

    //variable to control how fast the player moves and rotates after interacting
    [SerializeField] private float transitionLenght = 0.2f;
    private float transistionProgress = 0.0f;
    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion camStartRot;
    private bool isBeginningInteraction;

    //all of the interactable objects that need to be activated
    [SerializeField] private Interactables[] interactableObjects;

    // Update is called once per frame
    void Update()
    {
        if (isBeginningInteraction) BeginInteracting();
        if (!isBeginningInteraction && player != null) player.position = interactTarget.position;
    }

    //function that allows the player to take the wheel
    public void SetEntityInteracting(Transform entity, Transform cam)
    {
        //if the entity interacting is the player, save it and start the transition process
        if (entity != null)
        {
            player = entity;
            playerCamera = cam;
            startPos = entity.transform.position;
            startRot = entity.transform.rotation;
            camStartRot = cam.transform.rotation;
            transistionProgress = 0.0f;
            isBeginningInteraction = true;

            foreach (var obj in interactableObjects) obj.SetInteractable(true);
        }
        //otherwise set the player to null
        else
        {
            player = null;
            foreach (var obj in interactableObjects) obj.SetInteractable(false);
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
        //rotate the camera towards the control panel
        playerCamera.rotation = Quaternion.Slerp(camStartRot, cameraInteractTarget.rotation, t);

        if (transistionProgress >= transitionLenght)
        {
            isBeginningInteraction = false;
        }
    }
}