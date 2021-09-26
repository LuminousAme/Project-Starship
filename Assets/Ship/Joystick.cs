using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    //class for joysticks that can manually controlled by player input

    //the current state of the joystick, -1 is down/left, 0 is netural, 1 is up/right
    [SerializeField] private Vector2Int joystickState;
    //wheter or not the joystick is even interactable
    private bool isInteractable;
    //wheter or not the joystick is currently selected
    private bool isSelected;

    //mouse position on selection
    private Vector2 mousePosYOnSelected;
    //threshold for mouse movement before changing level
    [SerializeField] private Vector2 threshold;

    //rotation
    private Quaternion startingRotation;
    [SerializeField] private float sideRotDegrees = 45f;
    [SerializeField] private float vertRotDegrees = 45f;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        isInteractable = false;
        startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //if it's selected but the player has left the interaction mode, set to no longer be selected
        if (isSelected && !isInteractable) isSelected = false;

        //if the lever is currently selected 
        if (isSelected)
        {
            //if the player lets go of the mouse button unselect it
            if (Input.GetMouseButtonUp(0)) isSelected = false;

            //get the difference between the mouse position on selected and now
            Vector2 currentMousePos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
            Vector2 diff = currentMousePos - mousePosYOnSelected;

            //if it's gone up enough increase the joystick state and reset the mouse pos tracker
            if (diff.x > threshold.x && joystickState.x < 1)
            {
                joystickState.x++;
                mousePosYOnSelected.x = currentMousePos.x;
            }

            if (diff.y > threshold.y && joystickState.y < 1)
            {
                joystickState.y++;
                mousePosYOnSelected.y = currentMousePos.y;
            }

            //if it's gone down enough, decrease the joystick state and reset the mouse pos tracker
            if (diff.x < -1f * threshold.x && joystickState.x > -1)
            {
                joystickState.x--;
                mousePosYOnSelected.x = currentMousePos.x;
            }

            if (diff.y < -1f * threshold.y && joystickState.y > -1)
            {
                joystickState.y--;
                mousePosYOnSelected.y = currentMousePos.y;
            }

            float targetXEuler = startingRotation.eulerAngles.x - joystickState.x * sideRotDegrees;
            float targetYEuler = startingRotation.eulerAngles.y - joystickState.y * vertRotDegrees;
            targetRotation = Quaternion.Euler(new Vector3(targetXEuler, startingRotation.eulerAngles.y, targetYEuler));
            //consider slerping in the future
            transform.localRotation = targetRotation;
        }
    }

    public Vector2Int GetJoystickState()
    {
        return joystickState;
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    //when the mouse is overlapping this object
    public void mousedOver()
    {
        //only bother if the lever is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the lever, set to selected and save the mouse position in normalized space
            if (!isSelected && Input.GetMouseButton(0))
            {
                isSelected = true;
                mousePosYOnSelected = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
            }
        }
    }
}
