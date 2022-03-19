using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Keycap : MonoBehaviour
{
    [SerializeField] private string _letter;
    public string Letter { get => this._letter; }

    [SerializeField] private Button _button;
    public Button Button { get => this._button; }

    [SerializeField] private TMPro.TMP_Text _buttonText;

    private bool _shift = false;

    private Animator _buttonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        this._buttonAnimator = this._button.GetComponent<Animator>();
        this._button.onClick.AddListener(OnButtonSelected);
    }

    private void UpdateButtonLetter()
    {
        this._letter = !this._shift ? this._letter.ToLower() : this._letter.ToUpper();
        this._buttonText.text = this._letter;
    }

    private void OnButtonSelected()
    {
        // TODO: Could improve the visual feedback when pressing.
        // this.SetButtonToNormalMode();
    }

    private void SetButtonToNormalMode()
    {
        this._button.enabled = false;
        this._button.enabled = true;
        this._buttonAnimator.SetTrigger("Normal");
    }

    public void NotifyShiftChange(bool shiftActive)
    {
        this._shift = shiftActive;
        UpdateButtonLetter();
    }
}
