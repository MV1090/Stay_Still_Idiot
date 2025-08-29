using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartNetwork : MonoBehaviour
{
    [SerializeField] Button Button;
    [SerializeField] Button Button1;
    [SerializeField] Button Button2;
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Button.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
    }
}
