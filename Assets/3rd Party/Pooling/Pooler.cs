using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    private List<GameObject> inactivePool = new List<GameObject>();

    public GameObject itemPrefab;
    public int initialPoolSize = 10;

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddItemToPool();
        }
    }

    private void AddItemToPool()
    {
        GameObject newItem = Instantiate(itemPrefab);
        newItem.SetActive(false);

        inactivePool.Add(newItem);
    }

    public GameObject GetItem()
    {
        if(inactivePool.Count == 0)
        {
            AddItemToPool();
        }

        GameObject returnItem = inactivePool[0];

        return returnItem;
    }

    public void ReturnItem(GameObject returnedItem)
    {
        inactivePool.Add(returnedItem);
    }
}
