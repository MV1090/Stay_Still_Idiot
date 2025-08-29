using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] private Image crosshairDot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ShowCrosshair(false);
    }

    public void ShowCrosshair(bool show)
    {
        if (crosshairDot != null)
            crosshairDot.enabled = show;
    }       
}
