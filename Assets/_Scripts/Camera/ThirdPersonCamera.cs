using Unity.Cinemachine;
using UnityEngine;



public class ThirdPersonCamera : MonoBehaviour
{

    public UnityEngine.InputSystem.PlayerInput playerInput;
    public CinemachineInputAxisController inputController;

    public float currentSensitivity;
    public float gamePadMultiplier;
    void Awake()
    {
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        inputController = GetComponent<CinemachineInputAxisController>();
        playerInput.onControlsChanged += OnControlsChanged;        
    }
        
    void OnControlsChanged(UnityEngine.InputSystem.PlayerInput pi)
    {

        if (inputController == null)
            return;

        if (pi.currentControlScheme == "KeyboardMouse")
        {
            UpdateSensitivity(currentSensitivity);
            Debug.Log("Mouse"); 
        }
        else if (pi.currentControlScheme == "Gamepad")
        {
            UpdateSensitivity(currentSensitivity * gamePadMultiplier);
            Debug.Log("Gamepad");
        }
    }

    private void UpdateSensitivity(float newSensitivity)
    {
        var controllers = inputController.Controllers;

        foreach (var controller in controllers)
        {
            if (controller.Name == "Look Orbit X")
            {               
                controller.Input.Gain = newSensitivity;
                continue;
            }

            if (controller.Name == "Look Orbit Y")
            {
                controller.Input.Gain = newSensitivity * -1;
                break;
            }
        }               
    }
}
