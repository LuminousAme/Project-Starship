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

        Process();   
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
    public virtual void touched()
    {
        ShouldBeSelectedMat = true;
        mousedOverThisFrame = true;
    }
}