using System.Collections.Generic;
using UnityEngine;

public class ClearFilter : MonoBehaviour
{
    //  [SerializeField] private float junkRespawn = 5f; //time for filter to clog up again
    public float junkRespawn = 5f; //time for filter to clog up again

    [SerializeField] private float junkTimer; //time for filter to clog up again
    public List<FilterJunk> junk;

    //the player controller
    [SerializeField] private PlayerController playerC;

    private Vector3 startPos;

    // Start is called before the first frame update
    private void Start()
    {
        //var position = new Vector3(Random.Range(-0.0010f, 0.0010f), Random.Range(-0.0010f, 0.0010f), 0);

        foreach (var j in junk)
        {
            j.SetJunkState(false);
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
                    j.SetJunkState(false);
                    //j.SetPos(j.GetStartPos());
                    j.RandomizePos(transform.localScale/2);
                    //do
                    //{
                    //    j.RandomizePos(transform.localScale / 2);
                    //}
                    //while (j.transform.position.x > transform.localScale.x);
                }
            }
        }
        //if the timer reaches the respawn time but the player is interacting with the filter than just lower the timer a bit
        else if (junkTimer >= junkRespawn && playerC.GetInteracting() == true)
        {
            junkTimer = junkTimer - 5f;
        }
    }

    public void RespawnJunk()
    {
    }
}