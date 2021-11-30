using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHolder
{
    public int touchId;
    public bool isInteracting;

    public TouchHolder(int fingerId)
    {
        touchId = fingerId;
        isInteracting = false;
    }
}
