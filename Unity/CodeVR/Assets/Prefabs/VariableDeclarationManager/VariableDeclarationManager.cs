using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableDeclarationManager : MonoBehaviour
{
    private List<VariableDeclaration> _variables;
    public List<VariableDeclaration> Variables { get => this._variables; }

    public Action OnVariablesChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddVariable(string name)
    {
        this._variables.Add(new VariableDeclaration() {
            ID = System.Guid.NewGuid().ToString("N"),
            Name = name,
        });
        this.NotifyChange();
    }

    public void RemoveVariable(string id)
    {
        this._variables.RemoveAt(this._variables.FindIndex((variable) => variable.ID == id));
        this.NotifyChange();
    }

    public void Rename(string id, string newName)
    {
        var variableToRename = this._variables.Find((variable) => variable.ID == id);
        if (variableToRename == null) return;
        variableToRename.Name = newName;
        this.NotifyChange();
    }

    private void NotifyChange()
    {
        if (this.OnVariablesChanged != null) this.OnVariablesChanged();
    }
}

public class VariableDeclaration 
{
    public string ID { get; set; }
    public string Name { get; set; }
}