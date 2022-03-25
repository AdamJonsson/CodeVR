using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariablesDeclerationMenu : MonoBehaviour
{
    [SerializeField] private Button _addNewVariableButton;
    [SerializeField] private VariableNameForm _nameForm;
    [SerializeField] private GridLayoutGroup _variableContainer;
    [SerializeField] private VariableSection _variableSection;

    private VariableDeclarationManager _variableDeclerationManager;

    // Start is called before the first frame update
    void Start()
    {
        this._addNewVariableButton.onClick.AddListener(this.OnAddNewVariableButtonPressed);
        this._nameForm.OnDone += this.OnNameFormDone;
        this._nameForm.OnClose += this.OnNameFormClose;

        this._variableDeclerationManager = FindObjectOfType<VariableDeclarationManager>();

        foreach (var variable in this._variableDeclerationManager.Variables)
        {
            this.InstantiateVariableSection(variable);
        }
    }

    private void OnAddNewVariableButtonPressed()
    {
        this._nameForm.SetInputValue("");
        this._nameForm.gameObject.SetActive(true);
    }

    private void OnNameFormDone(string outputValue)
    {
        var variable = this._variableDeclerationManager.AddVariable(outputValue);
        this._nameForm.gameObject.SetActive(false);
        this.InstantiateVariableSection(variable);
    }
    
    private void InstantiateVariableSection(VariableDeclaration variable)
    {
        var newVariableSection = Instantiate(this._variableSection, Vector3.zero, Quaternion.identity);
        newVariableSection.transform.SetParent(this._variableContainer.gameObject.transform, false);
        newVariableSection.SetVariableDecleration(variable);
        this._nameForm.gameObject.SetActive(false);
    }

    private void OnNameFormClose()
    {
        this._nameForm.gameObject.SetActive(false);
    }
}
