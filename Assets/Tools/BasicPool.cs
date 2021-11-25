using System.Collections.Generic;
using UnityEngine;

public class BasicPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    //list of objects, first in first out
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    //dequeue takes first item in queue
    //enqueue puts item in queue at the end

    public static BasicPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        IncreasePoolSize(5);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
            IncreasePoolSize(4);

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    private void IncreasePoolSize(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var instanceToAdd = Instantiate(prefab);//create object
            instanceToAdd.transform.SetParent(transform); // set it's transform to the pool's transform
            instanceToAdd.SetActive(false); //make it inactive until it's called on for use
            availableObjects.Enqueue(instanceToAdd); //add it to the end of the queue
        }
    }
}