using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour 
{
    public static PlayerSpawner Instance { get; private set; }

    [SerializeField] private GameObject _propPrefab;
    [SerializeField] protected GameObject _hunterPrefab;

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestPropSpawnServerRpc(ulong clientId)
    {
        SpawnPlayer(_propPrefab, clientId, true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestHunterSpawnServerRpc(ulong clientId)
    {
        SpawnPlayer(_hunterPrefab, clientId, false);
    }

    private void SpawnPlayer(GameObject prefab, ulong clientId, bool isProp) 
    {
        GameObject spawnPoint = isProp ? ServerPlayerSpawnPoints.Instance.GetRandomPropSpawnPoint() : ServerPlayerSpawnPoints.Instance.GetRandomHunterSpawnPoint();

        Vector3 spawnPos = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
        Quaternion spawnRot = spawnPoint ? spawnPoint.transform.rotation : Quaternion.identity;

        GameObject player = Instantiate(prefab, spawnPos, spawnRot);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        player.GetComponent<PlayerController>().SetSpawnPositionClientRpc(spawnPos, spawnRot);
    }
}
