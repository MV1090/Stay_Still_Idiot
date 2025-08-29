using Unity.Netcode;

public abstract class PlayerRole : NetworkBehaviour
{
    public virtual void FirstAction(bool isPressed = false) {}
    public virtual void SecondAction(bool isPressed = false) {}
    public virtual void ThirdAction(bool isPressed = false) {}
    public virtual void FourthAction(bool isPressed = false) {}
    public virtual void PlayFirstActionVisuals(bool isPressed = false) { }
    public virtual void PlaySecondActionVisuals(bool isPressed = false) { }
    public virtual void PlayThirdActionVisuals(bool isPressed = false) { }
    public virtual void PlayFourthActionVisuals(bool isPressed = false) { }
    public virtual void PlayFirstActionLocalFeedback(bool isPressed = false) { }
    public virtual void PlaySecondActionLocalFeedback(bool isPressed = false) { }
    public virtual void PlayThirdActionLocalFeedback(bool isPressed = false) { }
    public virtual void PlayFourthActionLocalFeedback(bool isPressed = false) { }

    [ServerRpc]
    public void RequestFirstActionServerRpc(bool isPressed, ServerRpcParams rpcParams = default)
    {
        FirstAction(isPressed);
        PlayFirstActionClientRpc(isPressed);
    }

    [ServerRpc]
    public void RequestSecondActionServerRpc(bool isPressed, ServerRpcParams rpcParams = default)
    {
        SecondAction(isPressed);
        PlaySecondActionClientRpc(isPressed);
    }

    [ServerRpc]
    public void RequestThirdActionServerRpc(bool isPressed, ServerRpcParams rpcParams = default)
    {
        ThirdAction(isPressed);
        PlayThirdActionClientRpc(isPressed);
    }

    [ServerRpc]
    public void RequestFourthActionServerRpc(bool isPressed, ServerRpcParams rpcParams = default) 
    {
        FourthAction(isPressed);
        PlayFourthActionClientRpc(isPressed);
    }

    [ClientRpc]
    private void PlayFirstActionClientRpc(bool isPressed)
    {
        PlayFirstActionVisuals(isPressed);
    }

    [ClientRpc]
    private void PlaySecondActionClientRpc(bool isPressed)
    {
        PlaySecondActionVisuals(isPressed);
    }

    [ClientRpc]
    private void PlayThirdActionClientRpc(bool isPressed)
    {
        PlayThirdActionVisuals(isPressed);
    }

    [ClientRpc]
    private void PlayFourthActionClientRpc(bool isPressed)
    {
        PlayFourthActionVisuals(isPressed);
    }


}
