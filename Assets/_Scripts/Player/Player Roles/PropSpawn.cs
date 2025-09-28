using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class PropSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        { 
            enabled = false;
            return;
        }
        SpawnPlayer();
        base.OnNetworkSpawn();
    }

    void SpawnPlayer()
    {        
        var spawnPoint = ServerPlayerSpawnPoints.Instance.GetRandomPropSpawnPoint();
        var spawnPosition = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
        Debug.Log("Player Spawned at " + spawnPosition);
        transform.position = spawnPosition;
    }
}
