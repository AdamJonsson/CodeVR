using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionDeclareBlock : MonoBehaviour
{
    [SerializeField] private CodeBlock _settingBlock;
    public CodeBlock SettingBlock { get => this._settingBlock; }

    [SerializeField] private TextInput _nameInput;

    public TextInput NameInput { get => this._nameInput; }

    private CodeBlock _codeBlock;
    public CodeBlock CodeBlock { get => this._codeBlock; }

    public Action<FunctionDeclareBlock> OnNameChanged;
    public Action<FunctionDeclareBlock> OnParameterChanged;

    void Awake()
    {
        this._codeBlock = this.GetComponent<CodeBlock>();
        this._nameInput.OnChange += this.HandleNameInputChanged;
        this._settingBlock.OnConnectionsChangedToCluster += this.HandleSettingBlockChanged;
    }

    private void HandleNameInputChanged(string newInputValue)
    {
        if (this.OnNameChanged != null)
            this.OnNameChanged.Invoke(this);
    }

    private void HandleSettingBlockChanged()
    {
        if (this.OnParameterChanged != null)
            this.OnParameterChanged.Invoke(this);
    }

    public List<VariableDropdownHandler> GetCurrentParameters()
    {
        if (_settingBlock == null) return new List<VariableDropdownHandler>();
        var parametersAsCodeBlocks = this._settingBlock.GetBlockCluster(includeSelf: false).Where((block) => block.BlocklyTypeString == "parameter");
        var parametersAsDropdown = parametersAsCodeBlocks.Select(param => param.GetComponent<VariableDropdownHandler>()).ToList();
        return parametersAsDropdown;
    }

    
}
