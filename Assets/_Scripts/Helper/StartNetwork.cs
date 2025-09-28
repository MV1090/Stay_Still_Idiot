using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartNetwork : MonoBehaviour
{
    [SerializeField] Button Button;
    [SerializeField] Button Button1;
    [SerializeField] Button Button2;

    [SerializeField] Button Prop;
    [SerializeField] Button Hunter;

    private void Awake()
    {
        Prop.gameObject.SetActive(false);
        Hunter.gameObject.SetActive(false);
    }
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
        Prop.gameObject.SetActive(true);
        Hunter.gameObject.SetActive(true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
        Prop.gameObject.SetActive(true);
        Hunter.gameObject.SetActive(true);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
        Prop.gameObject.SetActive(true);
        Hunter.gameObject.SetActive(true);
    }

    public void OnSpawnProp()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            PlayerSpawner.Instance.RequestPropSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
            Prop.gameObject.SetActive(false);
            Hunter.gameObject.SetActive(false);
        }
    }

    public void OnSpawnHunter()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            PlayerSpawner.Instance.RequestHunterSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
            Prop.gameObject.SetActive(false);
            Hunter.gameObject.SetActive(false);
        }
    }
}
