using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    //class for levers that can manually controlled by player input

    //the current state of the lever, -1 is down, 0 is netural, 1 is up
    [SerializeField] private int leverState = 0;
    //wheter or not the lever is even interactable
    private bool isInteractable;
    //wheter or not the lever is currently selected
    private bool isSelected;

    //mouse y-position on selection
    private float mousePosYOnSelected;
    //threshold for mouse movement before changing level
    [SerializeField] private float threshold;

    //rotation
    private Quaternion startingRotation;
    [SerializeField] private float vertRotDegrees = 45f;
    private Quaternion targetRotation;

    //the mouse manager
    private MouseManager mouse;

    //materials
    [SerializeField] private Material baseMat;
    [SerializeField] private Material selectedMat;
    private bool ShouldBeSelectedMat;
    private bool isSelectedMat;
    private bool mousedOverThisFrame, mousedOverLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        isInteractable = false;
        startingRotation = transform.localRotation;
        mouse = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        ShouldBeSelectedMat = false;
        isSelectedMat = false;
        mousedOverThisFrame = false;
        mousedOverLastFrame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSelected && mousedOverLastFrame && !mousedOverThisFrame)
        {
            ShouldBeSelectedMat = false;
        }

        if (ShouldBeSelectedMat != isSelectedMat)
        {
            if(ShouldBeSelectedMat)
            {
                this.GetComponent<Renderer>().material = selectedMat;
                transform.parent.GetComponent<Renderer>().material = selectedMat;
            }
            else
            {
                this.GetComponent<Renderer>().material = baseMat;
                transform.parent.GetComponent<Renderer>().material = baseMat;
            }

            isSelectedMat = ShouldBeSelectedMat;
        }

        //if it's selected but the player has left the interaction mode, set to no longer be selected
        if (isSelected && !isInteractable)
        {
            isSelected = false;
            ShouldBeSelectedMat = false;
        }

        //if the lever is currently selected 
        if (isSelected)
        {
            //if the player lets go of the mouse button unselect it
            if (Input.GetMouseButtonUp(0))
            {
                isSelected = false;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                ShouldBeSelectedMat = false;
            }
            else
            {
                //get the difference between the mouse position on selected and now
                float currentMousePos = Input.mousePosition.y / (float)Screen.height;
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

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    //when the mouse is overlapping this object
    public void mousedOver()
    {
        ShouldBeSelectedMat = true;
        mousedOverThisFrame = true;

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
}