using Unity.Netcode;
using UnityEngine;

public class PropObject : MonoBehaviour
{
    [SerializeField] private string _objectId;
    
    public string PropObjectID => _objectId;
    
}
