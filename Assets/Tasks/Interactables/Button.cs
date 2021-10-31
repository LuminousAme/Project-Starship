using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactables
{
    //class for buttons that can manually controlled by player input

    //the current state of the button, false is not pressed, true is pressed
    private bool buttonState = false;

    //positional change
    private Vector3 startPos;
    [SerializeField] private float pressMovement = 0.01f;
    private Vector3 targetPos;

    // Start is called before the first frame update
    protected override void Init()
    {
        startPos = transform.position;
        //the button model's forward faces up, I assume this is some blender to unity weirdness but just using forward instead of up
        //here fixes it the issue so yeet 
        targetPos = startPos + (-transform.forward.normalized * pressMovement);
    }

    // Update is called once per frame
    protected override void Process()
    {
        //if the lever is currently selected and the player lets go of it, unselect it
        if (isSelected && Input.GetMouseButtonUp(0))
        {
            isSelected = false;
            if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
            ShouldBeSelectedMat = false;
        }

        if (buttonState)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = startPos;
        }

        mousedOverLastFrame = mousedOverThisFrame;
        mousedOverThisFrame = false;
    }

    public bool GetButtonState()
    {
        return buttonState;
    }

    //when the mouse is overlapping this object
    public override void mousedOver()
    {
        //call the base function
        base.mousedOver();

        //only bother if the button is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the button, set to selected and change the value
            if (!isSelected && Input.GetMouseButton(0))
            {
                isSelected = true;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                buttonState = !buttonState;
            }
        }
    }
}
