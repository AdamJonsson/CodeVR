using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DebugMovementManager : MonoBehaviour
{
    [SerializeField] private InputAction _enableMovementInput;
    private ContinuousMoveProviderBase _movementScript;
    private SnapTurnProviderBase _turnScript;

    void Awake()
    {
        this._movementScript = GetComponent<ContinuousMoveProviderBase>();
        this._turnScript = GetComponent<SnapTurnProviderBase>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _enableMovementInput.performed += this.OnToggleMovement;
        _enableMovementInput.Enable();
    }

    private void OnToggleMovement(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle movement!");
        this._movementScript.enabled = !this._movementScript.enabled;
        this._turnScript.enabled = !this._turnScript.enabled;
    }

}
