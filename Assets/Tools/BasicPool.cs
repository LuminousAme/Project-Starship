using System.Collections.Generic;
using UnityEngine;

public class BasicPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static BasicPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool(6);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
            GrowPool(2);

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    private void GrowPool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var instanceToAdd = Instantiate(prefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject thing)
    {
        thing.SetActive(false);
        availableObjects.Enqueue(thing);
    }
}