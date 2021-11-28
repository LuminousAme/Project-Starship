using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Joystick : Interactables
{
    //class for joysticks that can manually controlled by player input

    //the current state of the joystick, -1 is down/left, 0 is netural, 1 is up/right
    private Vector2Int joystickState;

    //mouse position on selection
    private Vector2 mousePosOnSelected;
    //threshold for mouse movement before changing level
    [SerializeField] private Vector2 threshold;
    [SerializeField] private float mobileThresholdMultipler = 2f;
    private Vector2 acutalThreshold;

    //rotation
    private Quaternion startingRotation;
    [SerializeField] private float sideRotDegrees = 45f;
    [SerializeField] private float vertRotDegrees = 45f;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    protected override void Init()
    {
        startingRotation = transform.rotation;
        acutalThreshold = (PlatformManager.GetIsMobileStatic()) ? mobileThresholdMultipler * threshold : threshold;
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
                Vector2 currentMousePos = (PlatformManager.GetIsMobileStatic()) ?
                    new Vector2(Input.GetTouch(thisTouchIndex).position.x / (float)Screen.width, Input.GetTouch(thisTouchIndex).position.y / (float)Screen.height) :
                    new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);

                Vector2 diff = currentMousePos - mousePosOnSelected;

                //if it's gone up enough increase the joystick state and reset the mouse pos tracker
                if (diff.x > acutalThreshold.x && joystickState.x < 1)
                {
                    joystickState.x++;
                    mousePosOnSelected.x = currentMousePos.x;
                }

                if (diff.y > acutalThreshold.y && joystickState.y < 1)
                {
                    joystickState.y++;
                    mousePosOnSelected.y = currentMousePos.y;
                }

                //if it's gone down enough, decrease the joystick state and reset the mouse pos tracker
                if (diff.x < -1f * acutalThreshold.x && joystickState.x > -1)
                {
                    joystickState.x--;
                    mousePosOnSelected.x = currentMousePos.x;
                }

                if (diff.y < -1f * acutalThreshold.y && joystickState.y > -1)
                {
                    joystickState.y--;
                    mousePosOnSelected.y = currentMousePos.y;
                }

                float targetXEuler = startingRotation.eulerAngles.x + joystickState.x * sideRotDegrees;
                float targetYEuler = startingRotation.eulerAngles.z + joystickState.y * vertRotDegrees;
                targetRotation = Quaternion.Euler(new Vector3(targetXEuler, startingRotation.eulerAngles.y, targetYEuler));
                //consider slerping in the future
                transform.rotation = targetRotation;
            }
        }

        mousedOverLastFrame = mousedOverThisFrame;
        mousedOverThisFrame = false;
    }

    public Vector2Int GetJoystickState()
    {
        return joystickState;
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

    public override void touched(int fingerID)
    {
        //call the base function
        base.touched(fingerID);

        //only bother if the lever is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the lever, set to selected and save the mouse position in normalized space
            if (!isSelected)
            {
                isSelected = true;
                if (mouse != null) mouse.SetTouchInteracting(fingerID);
                persistantTouchId = fingerID;
                FindTouchIndex();
                mousePosOnSelected = new Vector2(Input.GetTouch(thisTouchIndex).position.x / (float)Screen.width, Input.GetTouch(thisTouchIndex).position.y / (float)Screen.height);
            }
        }
    }
}