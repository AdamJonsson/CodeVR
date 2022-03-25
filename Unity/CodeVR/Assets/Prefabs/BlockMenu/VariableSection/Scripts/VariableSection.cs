using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSection : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _titleText;
    [SerializeField] private TMPro.TMP_Text _blockText;
    [SerializeField] private BlockDuplication _blockDuplication;

    [SerializeField] private VariableNameForm _nameForm;
    [SerializeField] private Button _editNameButton;
    [SerializeField] private Button _deleteVariableButton;
    [SerializeField] private GameObject _blockContainer;

    private VariableDeclaration _variableDecleration;
    private VariableDeclarationManager _variableDeclerationManager;
    
    // Start is called before the first frame update
    void Start()
    {
        this._variableDeclerationManager = FindObjectOfType<VariableDeclarationManager>();
        
        this._blockDuplication.OnClone += this.OnClone;
        this._editNameButton.onClick.AddListener(this.OnEdit);
        this._deleteVariableButton.onClick.AddListener(this.OnDelete);

        this._nameForm.OnDone += this.OnConfirmEditNameForm;
        this._nameForm.OnClose += this.OnCloseEditNameForm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClone(CodeBlock block)
    {
        var dropdownHandler = block.GetComponent<VariableDropdownHandler>();
        dropdownHandler.OverrideStartValue = this._variableDecleration.ID;
    }

    private void OnEdit()
    {
        this.ToggleNameFormVisibility(true);
    }

    private void OnDelete()
    {
        this._variableDeclerationManager.RemoveVariable(this._variableDecleration.ID);
        Destroy(this.gameObject);
    }

    private void OnConfirmEditNameForm(string newName)
    {
        this.ToggleNameFormVisibility(false);
        this._variableDeclerationManager.Rename(this._variableDecleration.ID, newName);
        this.SetNameToTextElements(newName);

    }

    private void OnCloseEditNameForm()
    {
        this.ToggleNameFormVisibility(true);
    }

    public void SetVariableDecleration(VariableDeclaration variableDeclaration)
    {
        this._variableDecleration = variableDeclaration;
        this.SetNameToTextElements(this._variableDecleration.Name);
    }

    private void SetNameToTextElements(string name)
    {
        this._titleText.text = name;
        this._blockText.text = name;
    }

    private void ToggleNameFormVisibility(bool show)
    {
        this._nameForm.gameObject.SetActive(show);
        this._blockContainer.SetActive(!show);
    }
}
