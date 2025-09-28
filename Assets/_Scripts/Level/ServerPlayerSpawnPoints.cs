using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerSpawnPoints : Singleton<ServerPlayerSpawnPoints>
{
    [SerializeField] private List<GameObject> _propSpawnPoints;
    [SerializeField] private List<GameObject> _hunterSpawnPoints;

    public GameObject GetRandomPropSpawnPoint()
    {      
        if(_propSpawnPoints.Count == 0)
            return null;

        return _propSpawnPoints[Random.Range(0, _propSpawnPoints.Count)];
    }

    public GameObject GetRandomHunterSpawnPoint()
    {
        if (_hunterSpawnPoints.Count == 0)
            return null;

        return _hunterSpawnPoints[Random.Range(0, _hunterSpawnPoints.Count)];
    }
}
