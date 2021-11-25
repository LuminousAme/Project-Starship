using System.Collections.Generic;
using UnityEngine;

public class ClearFilter : MonoBehaviour
{
    //  [SerializeField] private float junkRespawn = 5f; //time for filter to clog up again

    public float junkRespawn = 10f; //time for filter to clog up again

    //time that the palyer has before ship blowup
    [SerializeField] private float filterClogTimeLimit = 30f;

    [SerializeField] private float shipBlowUpTime;

    [SerializeField] private bool shipBlowUp;

    [SerializeField] private float junkTimer; //time for filter to clog up again
    public List<FilterJunk> junk;

    //the player controller
    [SerializeField] private PlayerController playerC;

    //control panel stuff
    [SerializeField] private GameObject controlPanel;

    private InteractEnter controlPanelInteractables;

    [SerializeField] private GameObject interactableList;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            var tempJ = BasicPool.Instance.GetFromPool();
            tempJ.transform.parent = interactableList.transform;

            AddJunk(tempJ.GetComponentInChildren<FilterJunk>());
        }
    }

    //https://answers.unity.com/questions/1506835/how-to-prevent-3d-object-spawn-overlapping.html
    // Start is called before the first frame update
    private void Start()
    {
        //var position = new Vector3(Random.Range(-0.0010f, 0.0010f), Random.Range(-0.0010f, 0.0010f), 0);
        shipBlowUpTime = 0f;
        controlPanelInteractables = controlPanel.GetComponent<InteractEnter>();

        Vector3 temppos = new Vector3(0.45f, 0f, -0.135f);

        foreach (var j in junk)
        {
            controlPanelInteractables.AddInteract(j);
            j.SetStartPos(interactableList.transform.position + temppos);
        }

        foreach (var j in junk)
        {
            // whether or not we can spawn in this position
            bool validPosition = false;

            j.SetJunkState(false);

            while (!validPosition)
            {
                j.RandomizePos(transform.localScale / 3.5f);

                validPosition = true;
                // Collect all colliders within the radius
                Collider[] colliders = Physics.OverlapSphere(j.transform.position, j.transform.localScale.x + 1);
                // Collider[] colliders2 = j.

                // Go through each collider collected
                foreach (Collider col in colliders)
                {
                    // If this collider is tagged as junk
                    if (col.tag == "Junk")
                    {
                        // Then this position is not a valid spawn position
                        validPosition = false;
                    }
                }
            }
            //   j.SetPos(j.GetStartPos());
            //   j.RandomizePos();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerC.GetInteracting() == false)
        {
            //increment timer
            junkTimer = junkTimer + Time.deltaTime;
        }

        //if junk timer reaches respawn time and the player is not interacting with the filter
        if (junkTimer >= junkRespawn && playerC.GetInteracting() == false)
        {
            junkTimer = 0;
            foreach (var j in junk)
            {
                //only respawn junk that has been smacked away
                if (j.GetJunkState() == true)
                {
                    // whether or not we can spawn in this position
                    bool validPosition = false;
                    //set to unsmacked state
                    j.SetJunkState(false);
                    while (!validPosition)
                    {
                        j.RandomizePos(transform.localScale / 3.5f);

                        validPosition = true;
                        // Collect all colliders within the radius
                        Collider[] colliders = Physics.OverlapSphere(j.transform.position, j.transform.localScale.x);
                        Debug.Log(j.transform.localScale.x);
                        // Go through each collider
                        foreach (Collider col in colliders)
                        {
                            // If this collider is tagged as junk
                            if (col.tag == "Junk")
                            {
                                Debug.Log("JUNK COllide");
                                // Then this position is not a valid spawn position since it is overlapping
                                validPosition = false;
                            }
                        }
                    }

                    /*
                    j.SetJunkState(false);
                    //j.SetPos(j.GetStartPos());
                    j.RandomizePos(transform.localScale / 3f);

                    Debug.Log(j.transform.position.x);
                    Debug.Log(transform.localScale.x);
                    */
                }
            }
        }
        //if the timer reaches the respawn time but the player is interacting with the filter than just lower the timer a bit
        else if (junkTimer >= junkRespawn && playerC.GetInteracting() == true)
        {
            junkTimer = junkTimer - 5f;
        }

        //check if the filter is clogged
        foreach (var j in junk)
        {
            //if a junk has not been smacked away increment timer for ship blow up
            if (j.GetJunkState() == false)
            {
                shipBlowUpTime = shipBlowUpTime + Time.deltaTime / 4;
            }
        }
        //if all junk has been smacked away then reset ship blowup timer
        if (CheckJunk() == true)
        {
            shipBlowUpTime = 0f;
        }

        if (shipBlowUpTime >= filterClogTimeLimit)
        {
            //ship should blowup
            shipBlowUp = true;
        }
    }

    //function to check if all the junk has been smack away
    private bool CheckJunk()
    {
        foreach (var j in junk)
        {
            //if one has no been smacked away then junk still remains
            if (j.GetJunkState() == false)
            {
                return false;
            }
        }
        //if all are false then all junk has been smacked away
        return true;
    }

    public bool shouldShipBlow()
    {
        return shipBlowUp;
    }

    public void AddJunk(FilterJunk item)
    {
        junk.Add(item);
    }
}