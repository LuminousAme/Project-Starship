using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    //wheter or not the object is even interactable
    protected bool isInteractable;
    //wheter or not the object is currently selected
    protected bool isSelected;

    //the mouse manager
    protected MouseManager mouse;

    //materials
    [SerializeField] protected Material baseMat;
    [SerializeField] protected Material selectedMat;
    protected bool ShouldBeSelectedMat;
    protected bool isSelectedMat;
    protected bool mousedOverThisFrame, mousedOverLastFrame;

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

        Init();
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
            if (ShouldBeSelectedMat)
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

        Process();
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
}