using UnityEngine;

public class FilterJunk : Interactables
{
    //class for junk that can be manually controlled by player input

    //state of the junk, false is not smacked away, true is smacked away
    public bool junkState = false;

    //positional change
    private Vector3 startPos;

    private Vector3 startPosSpawn;

    private Vector3 targetPos;

    //  [SerializeField] private GameObject interactableList;

    // Start is called before the first frame update
    protected override void Init()
    {
        //GameObject parentt = transform.parent.gameObject;
        // transform.parent = interactableList.transform;

        //starting pos of junk
        startPos = transform.position;

        //starting pos for spawning
        startPosSpawn = startPos;

        //target is behind the junk
        targetPos = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    protected override void Process()
    {
        bool released = (PlatformManager.GetIsMobileStatic()) ? HasTouchReleased()
            : Input.GetMouseButtonUp(0);

        //if the junk is currently selected and the player lets go of it, unselect it
        if (isSelected && released)
        {
            isSelected = false;
            if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
            ShouldBeSelectedMat = false;
        }

        //player has hit it and the junk should fade
        if (junkState)
        {
            transform.position -= targetPos * Time.deltaTime;
            transform.parent.position = transform.position;
        }
        //if not smacked away yet or respwaned
        else
        {
            transform.position = startPosSpawn;
            transform.parent.position = transform.position;
        }

        mousedOverLastFrame = mousedOverThisFrame;
        mousedOverThisFrame = false;
        //Debug.Log("TEst");
    }

    public bool GetJunkState()
    {
        return junkState;
    }

    public void SetJunkState(bool state)
    {
        junkState = state;
    }

    public Vector3 GetStartPos()
    {
        return startPos;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
        transform.parent.position = transform.position;
    }

    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
    }

    public void SetPosSpawn(Vector3 pos)
    {
        startPosSpawn = pos;
    }

    public void RandomizePos(Vector3 range)
    {
        //Vector3 position = new Vector3(Random.Range(0.1f, 0.5f), Random.Range(-0.1f, -0.5f), 0);
        //Vector3 position2 = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0);
        Vector3 ranges = new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), 0);

        //transform.position = transform.position + (startPos);

        startPosSpawn = startPos + ranges;
        transform.parent.position = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    //when the mouse is overlapping this object
    public override void mousedOver()
    {
        //call the base function
        base.mousedOver();

        //only bother if the junk is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the button, set to selected and change the value
            if (!isSelected && Input.GetMouseButton(0))
            {
                isSelected = true;
                if (mouse != null) mouse.SetObjectAlreadySelected(isSelected);
                junkState = true;
            }
        }
    }

    public override void touched(int fingerID)
    {
        //call the base function
        base.touched(fingerID);

        //only bother if the junk is currently interactable
        if (isInteractable)
        {
            //if the player clicks on the button, set to selected and change the value
            if (!isSelected)
            {
                isSelected = true;
                if (mouse != null) mouse.SetTouchInteracting(fingerID);
                junkState = true;
            }
        }
    }
}