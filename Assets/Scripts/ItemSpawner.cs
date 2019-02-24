using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class Items
{
    public GameObject item;
    public int rarity;

    public Items(GameObject it, int rare)
    {
        item = it;
        rarity = rare;
    }
}


public class ItemSpawner : NetworkBehaviour {
    

    public Items[] itemsToSpawn;//spawn these items on the itemSpawnPositions
    //[SerializeField] Transform itemParent;
    int itemVariety;

    private void Awake()
    {
        itemVariety = itemsToSpawn.Length;


        //for (int i = 0; i < 20; i++)
        //{
        //    Items item = itemsToSpawn[(int)Random.Range(0, itemVariety)];
        //    Debug.Log(item.item.name);
        //    if (Random.value % item.rarity <= 0.06f) Debug.Log("item wont spawn");
        //}
    }

    private void Start()
    {

        //if (doneOnce) return;
    
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("itemSpawnPoint");

        Debug.Log("spawnPoints.length = " + spawnPoints.Length);
        foreach (var point in spawnPoints)
        {
            StartCoroutine(SpawnItem(point.transform));
        }
        //RpcDone();
    }

    
    IEnumerator SpawnItem(Transform pos)
    {
        Items item = itemsToSpawn[(int)Random.Range(0, itemVariety)];
        //TODO BALANCING LATER:: might need to change 0.25f to give a better result in item spawns
        if (Random.value % item.rarity > 0.25f) //this is my 'weighted random', the item is less likely to spawn if it is rare
        {

            //Debug.Log("spawning " + item.item.name);
            var it = Instantiate(item.item, pos.position, pos.rotation);
            it.transform.SetParent(this.transform);

            yield return new WaitUntil(() => NetworkServer.active);
            yield return new WaitForEndOfFrame();
            NetworkServer.Spawn(it);
        }

        yield return null;
    }

    
}
