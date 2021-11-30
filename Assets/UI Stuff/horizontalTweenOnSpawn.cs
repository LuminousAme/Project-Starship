using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horizontalTweenOnSpawn : MonoBehaviour
{
    [SerializeField] private float tweenTime = 1f;
    [SerializeField] private float waitTime = 0f;
    private Vector3 targetScale;


    // Start is called before the first frame update
    void Start()
    {
        targetScale = transform.localScale;
        transform.localScale = new Vector3(0.0f, targetScale.y, targetScale.z);
        StartCoroutine(StartTween());
    }

    private IEnumerator StartTween()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        LeanTween.scale(gameObject, targetScale, tweenTime).setEaseOutBounce();
    }
}
