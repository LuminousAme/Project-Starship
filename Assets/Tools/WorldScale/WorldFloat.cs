using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that handles the floating origin for any object tree subscribed to it
//based on this video https://youtu.be/jLi9oo413js
public class WorldFloat : MonoBehaviour
{
    //when this is first created subsrcibe to the floating origin correction event
    private void OnEnable()
    {
        FloatingOrigin.onCorrection += Correction;
    }

    //when it's destroyed unsubsribe from the event
    private void OnDisable()
    {
        FloatingOrigin.onCorrection -= Correction;
    }

    //function that updates the position of all the child objects when a floating origin correction happens
    private void Correction(Vector3 offset)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position -= offset;
        }
    }
}
