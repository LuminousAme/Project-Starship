using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactables
{
    //class for levers that can manually controlled by player input

    //the current state of the lever, -1 is down, 0 is netural, 1 is up
    private int leverState = 0;

    //mouse y-position on selection
    private float mousePosYOnSelected;
    //threshold for mouse movement before changing level
    [SerializeField] private float threshold;

    //rotation
    private Quaternion startingRotation;
    [SerializeField] private float vertRotDegrees = 45f;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    protected override void Init()
    {
        startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    protected override void Process()
    {
        //if the lever is currently selected 
        if (isSelected)
        {
            bool released = (PlatformManager.GetIsMobileStatic()) ? HasTouchReleased()
            : Input.GetMouseButtonUp(0);

            //if the player lets go of the mouse button unselect it
            if (released)
            {
                isSelected = false;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                ShouldBeSelectedMat = false;
            }
            else if (!PlatformManager.GetIsMobileStatic() || Input.touchCount > 0)
            {
                //get the difference between the mouse position on selected and now
                float currentMousePos = (PlatformManager.GetIsMobileStatic()) ? Input.GetTouch(thisTouchIndex).position.y / (float)Screen.height : Input.mousePosition.y / (float)Screen.height;
                float diff = currentMousePos - mousePosYOnSelected;

                //if it's gone up enough increase the lever state and reset the mouse pos tracker
                if (diff > threshold && leverState < 1)
                {
                    leverState++;
                    mousePosYOnSelected = currentMousePos;
                }

                //if it's gone down enough, decrease the lever state and reset the mouse pos tracker
                if (diff < -1f * threshold && leverState > -1)
                {
                    leverState--;
                    mousePosYOnSelected = currentMousePos;
                }

                float targetYEuler = startingRotation.eulerAngles.y + leverState * vertRotDegrees;
                targetRotation = Quaternion.Euler(new Vector3(targetYEuler, startingRotation.eulerAngles.y, startingRotation.eulerAngles.z));
                //consider slerping in the future
                transform.localRotation = targetRotation;
            }
        }

        mousedOverLastFrame = mousedOverThisFrame;
        mousedOverThisFrame = false;
    }

    public int GetLeverState()
    {
        return leverState;
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
                mousePosYOnSelected = Input.mousePosition.y / (float)Screen.height;
            }
        }
    }

    public override void touched(int fingerID)
    {
        base.touched(fingerID);

        //only bother if the lever is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the lever, set to selected and save the mouse position in normalized space
            if (!isSelected && Input.GetMouseButton(0))
            {
                isSelected = true;
                if (mouse != null) mouse.SetTouchInteracting(fingerID);
                persistantTouchId = fingerID;
                FindTouchIndex();
                mousePosYOnSelected = Input.GetTouch(thisTouchIndex).position.y / (float)Screen.height;
            }
        }
    }
}