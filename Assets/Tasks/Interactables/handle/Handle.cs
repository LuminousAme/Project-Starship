using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : Interactables
{
    //class for handles that can be manually controlled by player input

    //current state of the handle, true is up, false is down
    private bool handleState = false;

    //mouse position on selection
    private Vector2 mousePosOnSelected;
    //threshold for mouse movement before changing level
    [SerializeField] private Vector2 threshold;

    //rotation
    private Quaternion startingRotation;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    protected override void Init()
    {
        startingRotation = transform.localRotation;
    }

    protected override void Process()
    {
        //if the lever is currently selected 
        if (isSelected)
        {
            //if the player lets go of the mouse button unselect it
            bool released = (PlatformManager.GetIsMobileStatic()) ? (numTouches < numTouchesLastFrame)
            : Input.GetMouseButtonUp(0);

            if (released)
            {
                isSelected = false;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                ShouldBeSelectedMat = false;
            }
            else if (!PlatformManager.GetIsMobileStatic() || Input.touchCount > 0)
            {

                //get the difference between the mouse position on selected and now
                Vector2 currentMousePos = (PlatformManager.GetIsMobileStatic()) ?
                    new Vector2(Input.GetTouch(0).position.x / (float)Screen.width, Input.GetTouch(0).position.y / (float)Screen.height) :
                    new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);

                Vector2 diff = currentMousePos - mousePosOnSelected;

                if (!handleState && diff.x < -1f * threshold.x && diff.y < -1f * threshold.y)
                {
                    handleState = true;
                    mousePosOnSelected = currentMousePos;
                }

                if (handleState && diff.x > threshold.x && diff.y > threshold.y)
                {
                    handleState = false;
                    mousePosOnSelected = currentMousePos;
                }

                targetRotation = (handleState) ? Quaternion.Euler(startingRotation.x, startingRotation.y, 90f) : startingRotation;
                //consider slerping in the future
                transform.localRotation = targetRotation;
            }
        }

        mousedOverLastFrame = mousedOverThisFrame;
        mousedOverThisFrame = false;
    }

    public bool GetHandleState()
    {
        return handleState;
    }

    //when the mouse is overlapping this object
    public override void mousedOver()
    {
        //call the base function
        base.mousedOver();

        //only bother if the lever is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the lever, set to selected and save the mouse position in normalized space
            if (!isSelected && Input.GetMouseButton(0))
            {
                isSelected = true;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                mousePosOnSelected = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
            }
        }
    }

    public override void touched()
    {
        //call the base function
        base.touched();

        //only bother if the lever is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the lever, set to selected and save the mouse position in normalized space
            if (!isSelected)
            {
                isSelected = true;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                mousePosOnSelected = new Vector2(Input.GetTouch(0).position.x / (float)Screen.width, Input.GetTouch(0).position.y / (float)Screen.height);
            }
        }
    }
}
