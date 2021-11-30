using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotate : MonoBehaviour
{
    [SerializeField] private float maxRotateRate;
    private float acutalRotateRate;
    [SerializeField] private float acelDcelRate;

    [SerializeField] private MaintainCoolant coolantTask;

    private void Start()
    {
        acutalRotateRate = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //if the fan is not running
        if(!coolantTask.fan)
        {
            //lower it's rotation rate down until it gets to zero
            acutalRotateRate = Mathf.Clamp(Mathf.Lerp(acutalRotateRate, 0f, acelDcelRate * Time.deltaTime), 0f, maxRotateRate);
        }
        else
        {
            //increase it's rotation until it's up to the max
            acutalRotateRate = Mathf.Clamp(Mathf.Lerp(acutalRotateRate, maxRotateRate, acelDcelRate * Time.deltaTime), 0f, maxRotateRate);
        }

        transform.Rotate(transform.up, acutalRotateRate * Time.deltaTime, Space.World);
    }
}
