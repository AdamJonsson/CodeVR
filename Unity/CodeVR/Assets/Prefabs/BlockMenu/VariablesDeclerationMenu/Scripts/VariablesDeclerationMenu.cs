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

    private List<VariableSection> _allInstantiatedSections = new List<VariableSection>();

    // Start is called before the first frame update
    void Start()
    {
        this._addNewVariableButton.onClick.AddListener(this.OnAddNewVariableButtonPressed);
        this._nameForm.OnDone += this.OnNameFormDone;
        this._nameForm.OnClose += this.OnNameFormClose;

        this._variableDeclerationManager = FindObjectOfType<VariableDeclarationManager>();
        this._variableDeclerationManager.OnVariablesChanged += OnVariableChanged;

        // foreach (var variable in this._variableDeclerationManager.Variables)
        // {
        //     this.InstantiateVariableSection(variable);
        // }
        this.InstantiateAllVariableSections();
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
    }

    private void OnVariableChanged()
    {
        this.DeleteAllInsantiatedVariableSections();
        this.InstantiateAllVariableSections();
    }
    
    private void InstantiateAllVariableSections()
    {
        foreach (var variable in this._variableDeclerationManager.Variables)
        {
            this.InstantiateVariableSection(variable);
        }
    }

    private void InstantiateVariableSection(VariableDeclaration variable)
    {
        var newVariableSection = Instantiate(this._variableSection, Vector3.zero, Quaternion.identity);
        newVariableSection.transform.SetParent(this._variableContainer.gameObject.transform, false);
        newVariableSection.SetVariableDecleration(variable);
        this._nameForm.gameObject.SetActive(false);
        this._allInstantiatedSections.Add(newVariableSection);
    }


    private void DeleteAllInsantiatedVariableSections()
    {
        foreach (var instantiatedSection in this._allInstantiatedSections)
        {
            Destroy(instantiatedSection.gameObject);
        }
        this._allInstantiatedSections.Clear();
    }

    private void OnNameFormClose()
    {
        this._nameForm.gameObject.SetActive(false);
    }
}
