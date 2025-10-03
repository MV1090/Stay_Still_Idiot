using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class DestructibleManager : NetworkBehaviour
{
    public static DestructibleManager Instance;

    private NetworkList<int> destroyedIds;

    private Dictionary<int, Destructible> destructibleDictionary = new Dictionary<int, Destructible>();
    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        destroyedIds = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        destructibleDictionary.Clear();

        foreach(Destructible destructible in FindObjectsByType<Destructible>(FindObjectsSortMode.None))
        {
            destructibleDictionary[destructible.ObjectId] = destructible;
        }

        foreach(var id in destroyedIds)
        {
            RemoveDestroyedObjects(id);
        }         
    }

    private void RemoveDestroyedObjects(int id)
    {
        if(destructibleDictionary.TryGetValue(id, out var destructible) && destructible != null)
        {
            Destroy(destructible.gameObject);
            destructibleDictionary[id] = null;  
        }
    }

    public void HandleDestructibleHit(Destructible destructible)
    {
        if (!IsServer)
            return;

        int id = destructible.ObjectId;

        if (!destroyedIds.Contains(id))
        {           
            destroyedIds.Add(id);
            DestroyDestructibleClientRpc(id);
            RemoveDestroyedObjects(id);
        }               
    }

    [ClientRpc]
    private void DestroyDestructibleClientRpc(int id)
    {
        RemoveDestroyedObjects(id);
    }

}
