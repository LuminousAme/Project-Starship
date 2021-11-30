using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Interactables : MonoBehaviour
{
    //wheter or not the object is even interactable
    protected bool isInteractable;
    //wheter or not the object is currently selected
    protected bool isSelected;

    //the mouse manager
    protected MouseManager mouse;

    //materials
    protected Shader baseShader;
    protected Shader selectedShader;
    protected bool ShouldBeSelectedMat;
    protected bool isSelectedMat;
    protected bool mousedOverThisFrame, mousedOverLastFrame;

    //number of mobile touches
    protected static int numTouches = 0, numTouchesLastFrame = 0;
    //most recent touch id on mobile
    protected int persistantTouchId = -1;
    protected int thisTouchIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        isInteractable = false;
        mouse = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        ShouldBeSelectedMat = false;
        isSelectedMat = false;
        mousedOverThisFrame = false;
        mousedOverLastFrame = false;

        baseShader = Shader.Find("Standard");
        selectedShader = Shader.Find("Custom/OutlineEffect");

        Init();
    }

    // Update is called once per frame
    void Update()
    {
        numTouches = Input.touchCount;

        if (!isSelected && mousedOverLastFrame && !mousedOverThisFrame)
        {
            ShouldBeSelectedMat = false;
        }

        if (ShouldBeSelectedMat != isSelectedMat)
        {
            if (ShouldBeSelectedMat)
            {
                List<Material> objectMats = this.GetComponent<Renderer>().materials.ToList();
                foreach (var mat in objectMats)
                {
                    mat.shader = selectedShader;
                    mat.SetFloat("_OutlineWidth", 0.008f);
                }
                List<Material> parentMats = transform.parent.GetComponent<Renderer>().materials.ToList();
                foreach (var mat in parentMats)
                {
                    mat.shader = selectedShader;
                    mat.SetFloat("_OutlineWidth", 0.006f);
                }
            }
            else
            {
                List<Material> objectMats = this.GetComponent<Renderer>().materials.ToList();
                foreach (var mat in objectMats)
                {
                    mat.shader = baseShader;
                }
                List<Material> parentMats = transform.parent.GetComponent<Renderer>().materials.ToList();
                foreach (var mat in parentMats)
                {
                    mat.shader = baseShader;
                }
            }

            isSelectedMat = ShouldBeSelectedMat;
        }

        //if it's selected but the player has left the interaction mode, set to no longer be selected
        if (isSelected && !isInteractable)
        {
            isSelected = false;
            ShouldBeSelectedMat = false;
        }

        FindTouchIndex();

        Process();   
    }

    protected void FindTouchIndex()
    {
        //if the touch id isn't -1, find the associated touch in the array and save it's index
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (persistantTouchId == -1) break;
            else if (touch.fingerId == persistantTouchId)
            {
                thisTouchIndex = i;
                break;
            }
        }
    }

    protected bool HasTouchReleased()
    {
        //if the persistent id is -1 don't both
        if (persistantTouchId == -1) return false;

        //if a touch has been released we need to check if it was this one
        if (numTouches < numTouchesLastFrame)
        {
            //iterate through all of the touches, if it finds this touch id then it must have been released
            bool idNotInList = true;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if(touch.fingerId == persistantTouchId)
                {
                    idNotInList = false;
                    break;
                }
            }

            //if it has, set the persistent id back to -1 until it has acutally be released
            if (idNotInList) persistantTouchId = -1;

            return idNotInList;
        }
        //if none of the touches have been released, this one definitely hasn't
        else return false;
    }

    private void LateUpdate()
    {
        numTouchesLastFrame = numTouches;
    }

    //inits the interactable object
    protected virtual void Init() { }

    //processes the stuff with the object
    protected virtual void Process() { }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    //when the mouse is overlapping this object
    public virtual void mousedOver()
    {
        ShouldBeSelectedMat = true;
        mousedOverThisFrame = true;
    }

    //when this object is being touched from a mobile device
    public virtual void touched(int fingerID)
    {
        ShouldBeSelectedMat = true;
        mousedOverThisFrame = true;
        persistantTouchId = fingerID;
    }
}