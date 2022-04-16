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

    public DropdownInput DropdownInput { get => this._dropdownInput; }

    [SerializeField] private GameObject _blockSizeToFollow;
    
    public GameObject BlockSizeToFollow { get => this._blockSizeToFollow; }

    private CodeBlock _codeBlock;
    private CodeBlockSize _codeBlockSize;
    private List<ParameterSection> _currentParameterBlocks = new List<ParameterSection>();

    private CodeBlockManager _codeBlockManager;
    private CodeBlockConnectionManager _connectionManager;

    private List<FunctionDeclareBlock> _currentFunctionsInScene = new List<FunctionDeclareBlock>();

    private int _currentNumberOfFunctionsDeclareBlocks = 0;

    private Dictionary<int, CodeBlockConnector> _connectorsDisconnectedByParameterChange = new Dictionary<int, CodeBlockConnector>();
    
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

    private void AddParameterSection(ExpandableBlock parent, string name, bool isLastParameter = false)
    {
        var child = Instantiate(this._parameterBlockPrefab, Vector3.zero, Quaternion.identity, parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.rotation = this._codeBlock.transform.rotation;
        child.SetBlocklySetting(this._currentParameterBlocks.Count);

        // We do not want any margin on the last parameter block
        if (isLastParameter)
            child.ExpandableBlock.SetExtraExpandSize(Vector3.zero);

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
        {
            int parameterIndex = this._currentParameterBlocks.IndexOf(child);

            // Create or override value in dictionary
            if(!this._connectorsDisconnectedByParameterChange.TryAdd(parameterIndex, child.Connector.Connection))
                this._connectorsDisconnectedByParameterChange[parameterIndex] = child.Connector.Connection;

            this._connectionManager.DetachConnector(child.Connector);
        }

        parent.Expandables.Remove(child.ExpandableSetting);
        this._codeBlock.RemoveConnector(child.Connector);
        this._codeBlock.RemoveColliderFromInteractable(child.Collider);
        this._codeBlockSize.HeightExpandableBlocks.Remove(child.ExpandableBlock);
        this._currentParameterBlocks.Remove(child);
        Destroy(child.gameObject);
    }

    private void ReconnectDisconnectedConnectorsFromParameterChange()
    {
        int index = -1;
        foreach (var parameterBlock in this._currentParameterBlocks)
        {
            index++;
            if (!this._connectorsDisconnectedByParameterChange.ContainsKey(index)) continue;
            var connector = this._connectorsDisconnectedByParameterChange[index];
            this._connectorsDisconnectedByParameterChange.Remove(index);

            if (connector.IsConnected || parameterBlock.Connector.IsConnected) continue;
            this._connectionManager.ConnectBlocks(connector, parameterBlock.Connector);
        }
    }

    private void HandleNewOrDeletedBlocks()
    {
        List<FunctionDeclareBlock> functionDeclareBlocks = this.FindCurrentFunctionBlocksInScene();
        this._currentFunctionsInScene = functionDeclareBlocks;

        if (functionDeclareBlocks.Count == this._currentNumberOfFunctionsDeclareBlocks) return;
        
        this.AddChangeListenersToFunctionBlocks(functionDeclareBlocks);
        this.GenerateDropdownOptions(functionDeclareBlocks);
        this.GenerateAllParameterOptionsToDropdown(functionDeclareBlocks);

        this._currentNumberOfFunctionsDeclareBlocks = functionDeclareBlocks.Count;
    }

    private void GenerateAllParameterOptionsToDropdown(List<FunctionDeclareBlock> functionDeclareBlocks)
    {
        foreach (var functionDeclareBlock in functionDeclareBlocks)
        {
            this.OnParameterChanged(functionDeclareBlock);
        }
    }

    private void AddChangeListenersToFunctionBlocks(List<FunctionDeclareBlock> functionDeclareBlocks)
    {
        foreach (var functionDeclareBlock in functionDeclareBlocks)
        {
            functionDeclareBlock.OnParameterChanged -= this.OnParameterChanged;
            functionDeclareBlock.OnParameterChanged += this.OnParameterChanged;

            functionDeclareBlock.OnNameChanged -= this.OnNameChanged;
            functionDeclareBlock.OnNameChanged += this.OnNameChanged;
        }
    }

    private List<FunctionDeclareBlock> FindCurrentFunctionBlocksInScene()
    {
        List<FunctionDeclareBlock> functionDeclareBlocks = new List<FunctionDeclareBlock>();
        foreach (var block in this._codeBlockManager.AllCodeBlocks)
        {
            bool isFunctionBlock = block.BlocklyTypeString == "procedures_defreturn";
            if (!isFunctionBlock) continue;
            FunctionDeclareBlock functionDeclareBlock = block.GetComponent<FunctionDeclareBlock>();
            functionDeclareBlocks.Add(functionDeclareBlock);
        }
        return functionDeclareBlocks;
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
        return functionDeclareBlock.CodeBlock.ID == this._dropdownInput.SelectedOption.Value;
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
            bool isLast = i == parameters.Count - 1;
            this.AddParameterSection(parent, parameter.DropdownInput.SelectedOption.Text, isLast);
        }

        this.ReconnectDisconnectedConnectorsFromParameterChange();
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

    public FunctionDeclareBlock SelectedFunction
    {
        get => this._currentFunctionsInScene.Find((function) => function.CodeBlock.ID == this._dropdownInput.Value);
    }
}
