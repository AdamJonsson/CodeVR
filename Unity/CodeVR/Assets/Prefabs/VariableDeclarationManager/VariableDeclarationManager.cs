using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableDeclarationManager : MonoBehaviour
{
    private List<VariableDeclaration> _variables = new List<VariableDeclaration>() {
        new VariableDeclaration {
            Name = "foo",
            ID = System.Guid.NewGuid().ToString("N")
        },
        new VariableDeclaration {
            Name = "bar",
            ID = System.Guid.NewGuid().ToString("N")
        },
    };
    public List<VariableDeclaration> Variables { get => this._variables; }

    public Action OnVariablesChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public VariableDeclaration AddVariable(string name)
    {
        var newVariable = new VariableDeclaration() {
            ID = System.Guid.NewGuid().ToString("N"),
            Name = name,
        };
        this._variables.Add(newVariable);
        this.NotifyChange();
        return newVariable;
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
        if (this.OnVariablesChanged != null) this.OnVariablesChanged.Invoke();
    }
}

public class VariableDeclaration 
{
    public string ID { get; set; }
    public string Name { get; set; }
}