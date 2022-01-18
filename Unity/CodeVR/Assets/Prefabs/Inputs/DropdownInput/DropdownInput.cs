using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class DropdownInput : MonoBehaviour
{

    [SerializeField] private CodeBlockInput _codeBlockInputPrefab;
 
    [SerializeField] private List<DropdownOption> _options = new List<DropdownOption>();

    [SerializeField] private float _verticalSpace = 1.0f;

    [SerializeField] private AnimationCurve _expantionCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private float _expantionTime;

    private float _expantionValue = 0.0f;

    private CodeBlockInput _thisInput;

    private DropdownOption _selectedOption;

    private bool _optionsVisible = false;

    private EventSystem _eventSystem;

    private bool _currentlyExpanding = false;

    private List<DropdownOption> _allOptionsExceptSelected = new List<DropdownOption>();

    // Start is called before the first frame update
    void Start()
    {
        this._thisInput = GetComponent<CodeBlockInput>();
        this.InstantiateOptions();
        this.RepositionOptions();
        this._eventSystem = FindObjectOfType<EventSystem>();

        this._thisInput.Button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (this._expantionValue < 1.0f && this._optionsVisible) this._expantionValue += Time.deltaTime / this._expantionTime;
        if (this._expantionValue > 0.0f && !this._optionsVisible) this._expantionValue -= Time.deltaTime / this._expantionTime;

        _expantionValue = Mathf.Clamp(_expantionValue, 0.0f, 1.0f);
        this.RepositionOptions();
    }

    private void OnClick()
    {
        if (this._currentlyExpanding) return;
        this.ToggleShowOptions();
    }

    private void OnOptionClicked(DropdownOption optionClicked)
    {
        StartCoroutine(
            this.HideOptions(() => {
                this.SelectOption(optionClicked);
            })
        );
    }

    private void InstantiateOptions()
    {
        DropdownOption optionToSelectAutomatically = null;
        foreach (var option in this._options)
        {
            var spawnedOption = Instantiate(
                this._codeBlockInputPrefab, 
                Vector3.zero, 
                this.transform.rotation, 
                this.transform.parent
            );
            spawnedOption.SetText(option.Text);
            spawnedOption.SetCanvasSize(this._thisInput.CanvasSize);
            spawnedOption.SetTransparency(1.0f);
            spawnedOption.gameObject.SetActive(false);
            option.SetInputObject(spawnedOption);
            spawnedOption.Button.onClick.AddListener(() => OnOptionClicked(option));
            if (option.IsDefaultValue) optionToSelectAutomatically = option;
        }

        if (optionToSelectAutomatically == null)
        {
            optionToSelectAutomatically = this._options.First();
        }

        SelectOption(optionToSelectAutomatically);
    }

    private void ToggleShowOptions()
    {
        if (this._optionsVisible)
        {
            StartCoroutine(this.HideOptions(() => {}));
            this._eventSystem.SetSelectedGameObject(null);
        }
        else
        {
            StartCoroutine(this.ShowOptions());
        }
    }

    private IEnumerator ShowOptions()
    {
        this._optionsVisible = true;
        this._currentlyExpanding = true;

        foreach (var option in this._allOptionsExceptSelected)
        {
            option.InputObject.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(this._expantionTime);

        this._currentlyExpanding = false;
    }

    private IEnumerator HideOptions(Action OnHideDone)
    {
        this._optionsVisible = false;
        this._currentlyExpanding = true;

        yield return new WaitForSeconds(this._expantionTime);

        foreach (var option in this._options)
        {
            option.InputObject.gameObject.SetActive(false);
        }

        this._currentlyExpanding = false;
        OnHideDone();
    }

    private void SelectOption(DropdownOption option)
    {
        this._selectedOption = option;
        this._allOptionsExceptSelected = this.GetAllOptionExceptSelected();
        this._thisInput.SetText(option.Text);
    }

    private List<DropdownOption> GetAllOptionExceptSelected()
    {
        var optionsExceptSelected = new List<DropdownOption>();
        optionsExceptSelected.AddRange(this._options);
        optionsExceptSelected.Remove(this._selectedOption);
        return optionsExceptSelected;
    }

    private void RepositionOptions()
    {
        int index = 0; 
        int spaceIndex = 1;
        foreach (var option in this._allOptionsExceptSelected)
        {
            var direction = index % 2 == 0 ? Vector3.up : Vector3.down;
            option.InputObject.transform.localPosition = 
                this.transform.localPosition + 
                direction * spaceIndex * (this._verticalSpace + 1) * this._thisInput.CanvasSize.y * this._thisInput.CanvasScale.y * this._expantionCurve.Evaluate(this._expantionValue);
            spaceIndex += index % 2;
            index++;
        }
    }

    [Serializable]
    public class DropdownOption
    {
        public string Text;
        public string Value;
        public bool IsDefaultValue;

        private CodeBlockInput _inputObject;

        public CodeBlockInput InputObject {get => _inputObject; }

        public void SetInputObject(CodeBlockInput input)
        {
            this._inputObject = input;
        }
    }
}
