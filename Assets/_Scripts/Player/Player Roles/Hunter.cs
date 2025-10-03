using Unity.Netcode;
using UnityEngine;

public class Hunter : PlayerRole
{
    [SerializeField] Weapon weaponInstance;
   
    public override void FirstAction(bool isPressed = false)
    {
        
    }

    public override void PlayFirstActionVisuals(bool isPressed)
    {
        DetectHit(); 
    }

    private void DetectHit()
    {
        weaponInstance.DetectHit();
    }
}
