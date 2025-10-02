using Unity.Netcode;
using UnityEngine;

public class Hunter : PlayerRole
{
    [SerializeField] Weapon weaponInstance;

    public override void OnNetworkSpawn()
    {
        //NetworkObjectReference weaponRef = weaponInstance.GetComponent<NetworkObject>();
        //if (weaponInstance.TryGetComponent(out NetworkObject weaponObj))
        //{
        //    weaponObj.ChangeOwnership(OwnerClientId);
        //}
            
    }
    public override void FirstAction(bool isPressed = false)
    {
        
    }

    public override void PlayFirstActionVisuals(bool isPressed)
    { DetectHit(); }

    private void DetectHit()
    {
        weaponInstance.DetectHit();
    }
}
