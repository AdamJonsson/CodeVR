using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionCallBlock : MonoBehaviour
{
    [SerializeField] private ParameterSection _parameterBlockPrefab;

    [SerializeField] private ExpandableBlock _rootOfParameterBlocks;

    [SerializeField] private int _numbers = 0;

    [SerializeField] private DropdownInput _dropdownInput;

    [SerializeField] private GameObject _blockSizeToFollow;
    
    public GameObject BlockSizeToFollow { get => this._blockSizeToFollow; }

    private CodeBlock _codeBlock;
    private CodeBlockSize _codeBlockSize;
    private List<ParameterSection> _currentParameterBlocks = new List<ParameterSection>();

    private CodeBlockManager _codeBlockManager;
    private CodeBlockConnectionManager _connectionManager;

    private List<FunctionDeclareBlock> _currentFunctionsInScene = new List<FunctionDeclareBlock>();

    void Awake()
    {
        this._codeBlock = this.GetComponent<CodeBlock>();
        this._codeBlockSize = this.GetComponent<CodeBlockSize>();
        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
        this._connectionManager = FindObjectOfType<CodeBlockConnectionManager>();

        this._codeBlockManager.OnBlocksAddedOrDeleted += this.HandleNewOrDeletedBlocks;
        this._rootOfParameterBlocks.OnResize += this.ResizeParameterChildren;
        this._dropdownInput.OnChange += (_) => this.HandleNewDropdownSelect();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.HandleNewOrDeletedBlocks();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ResizeParameterChildren()
    {
        foreach (var parameterBlock in this._currentParameterBlocks)
        {
            var expandScale = parameterBlock.ExpandableBlock.ExpandScale;
            parameterBlock.ExpandableBlock.ExpandScale = new Vector3(this._rootOfParameterBlocks.ExpandScale.x, expandScale.y, expandScale.z);
            parameterBlock.ExpandableBlock.ApplyScaleToExpandableSettings();
        }
    }

    private ExpandableBlock GetParentParameterFromChildIndex(int index)
    {
        if (index > this._currentParameterBlocks.Count) return null;
        if (index == 0) return this._rootOfParameterBlocks;
        return this._currentParameterBlocks[index - 1].ExpandableBlock;
    }

    private void RemoveAllParameterSections()
    {
        for (int i = this._currentParameterBlocks.Count - 1; i >= 0; i--)
        {
            var child = this._currentParameterBlocks[i];
            var parent = this.GetParentParameterFromChildIndex(i);
            this.RemoveParameterSection(parent, child);
        }
    }

    private void AddParameterSection(ExpandableBlock parent, string name)
    {
        var child = Instantiate(this._parameterBlockPrefab, Vector3.zero, Quaternion.identity, parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.rotation = this._codeBlock.transform.rotation;

        // Block and collider
        parent.Expandables.Add(child.ExpandableSetting);
        this._codeBlock.AddColliderToInteractable(child.Collider);

        // Connectors
        child.Connector.BlockAttachedTo = _codeBlock;
        this._codeBlock.AddConnector(child.Connector);
        this._codeBlockSize.ConnectorsThatEffectWidth.Add(child.Connector);

        // Size
        this._codeBlockSize.HeightExpandableBlocks.Add(child.ExpandableBlock);
        
        // Text
        child.Text.SetStartText(name);
        
        this._currentParameterBlocks.Add(child);
        parent.ApplyScaleToExpandableSettings();
    }

    private void RemoveParameterSection(ExpandableBlock parent, ParameterSection child)
    {
        if (child.Connector.IsConnected)
            this._connectionManager.DetachConnector(child.Connector);

        parent.Expandables.Remove(child.ExpandableSetting);
        this._codeBlock.RemoveConnector(child.Connector);
        this._codeBlock.RemoveColliderFromInteractable(child.Collider);
        this._codeBlockSize.HeightExpandableBlocks.Remove(child.ExpandableBlock);
        this._currentParameterBlocks.Remove(child);
        Destroy(child.gameObject);
    }

    private void HandleNewOrDeletedBlocks()
    {
        List<FunctionDeclareBlock> functionDeclareBlocks = new List<FunctionDeclareBlock>();
        foreach (var block in this._codeBlockManager.AllCodeBlocks)
        {
            bool isFunctionBlock = block.BlocklyTypeString == "procedures_defreturn";
            if (!isFunctionBlock) continue;
            FunctionDeclareBlock functionDeclareBlock = block.GetComponent<FunctionDeclareBlock>();
            functionDeclareBlocks.Add(functionDeclareBlock);

            functionDeclareBlock.OnParameterChanged -= this.OnParameterChanged;
            functionDeclareBlock.OnParameterChanged += this.OnParameterChanged;

            functionDeclareBlock.OnNameChanged -= this.OnNameChanged;
            functionDeclareBlock.OnNameChanged += this.OnNameChanged;
        }

        this._currentFunctionsInScene = functionDeclareBlocks;
        this.GenerateDropdownOptions(functionDeclareBlocks);

        foreach (var functionDeclareBlock in functionDeclareBlocks)
        {
            this.OnParameterChanged(functionDeclareBlock);
        }
    }

    private void GenerateDropdownOptions(List<FunctionDeclareBlock> functions)
    {
        if (functions.Count == 0) return;
        var currentSelectedValue = this._dropdownInput.Value;
        var newDropdownValues = functions.Select((function) => {
            var name = function.NameInput.Value;
            if (name == "") name = "Unnamed function";
            return new DropdownInput.DropdownOption() {
                Text = name,
                Value = function.CodeBlock.ID,
                IsDefaultValue = function.CodeBlock.ID == currentSelectedValue
            };
        }).ToList();

        this._dropdownInput.SetOptions(newDropdownValues);
    }

    private bool FunctionChangeRelevant(FunctionDeclareBlock functionDeclareBlock)
    {
        return functionDeclareBlock.CodeBlock.ID == 
        this._dropdownInput.SelectedOption.Value;
    }

    private void OnNameChanged(FunctionDeclareBlock functionDeclareBlock)
    {
        this.GenerateDropdownOptions(this._currentFunctionsInScene);
    }

    private void OnParameterChanged(FunctionDeclareBlock functionDeclareBlock)
    {
        if (!this.FunctionChangeRelevant(functionDeclareBlock)) return;
        this.RecreateParameterSections(functionDeclareBlock);
    }

    private void RecreateParameterSections(FunctionDeclareBlock functionDeclareBlock)
    {
        this.RemoveAllParameterSections();
        var parameters = functionDeclareBlock.GetCurrentParameters();
        for (int i = 0; i < parameters.Count; i++)
        {
            var parameter = parameters[i];
            var parent = this.GetParentParameterFromChildIndex(i);
            this.AddParameterSection(parent, parameter.DropdownInput.SelectedOption.Text);
        }

        this.ResizeParameterChildren();
        this._codeBlock.ResizeBlockCluster();
    }

    private void HandleNewDropdownSelect()
    {
        var selectedFunctionID = this._dropdownInput.SelectedOption.Value;
        var function = this._currentFunctionsInScene.Find((functionSearch) => functionSearch.CodeBlock.ID == selectedFunctionID);
        if (function == null) return;
        this.RecreateParameterSections(function);
    }
}
