using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRKeyboard : MonoBehaviour
{

    [SerializeField] private List<Keycap> _keycaps;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _shiftButton;

    public event Action<string> OnChange;
    public event Action OnClose;

    private bool _isShiftActive;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var keycap in this._keycaps)
        {
            keycap.Button.onClick.AddListener(() => KeyTyped(keycap));
        }

        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _shiftButton.onClick.AddListener(OnShiftButtonClicked);
    }

    private void KeyTyped(Keycap keycap)
    {
        if (this.OnChange != null)
            this.OnChange.Invoke(keycap.Letter);
    }

    private void OnCloseButtonClicked()
    {
        if (this.OnClose != null)
            this.OnClose.Invoke();
    }

    private void OnShiftButtonClicked()
    {
        this._isShiftActive = !this._isShiftActive;

        foreach (var keycap in this._keycaps)
        {
            keycap.NotifyShiftChange(this._isShiftActive);
        }

    }

}
