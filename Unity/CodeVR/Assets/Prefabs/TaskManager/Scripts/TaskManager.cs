using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<StartingBlockData> _startingBlocksData;

    [SerializeField] private bool _disable;

    public Action<TaskStatusResponse> OnTaskStatusChange;

    private string _currentTaskID = "";

    private CodeBlockConnectionManager _codeBlockConnectionManager;
    private CodeBlockManager _codeBlockManager;
    private VariableDeclarationManager _variableManager;

    void Awake()
    {
        this._codeBlockConnectionManager = FindObjectOfType<CodeBlockConnectionManager>();
        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
        this._variableManager = FindObjectOfType<VariableDeclarationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_disable)
            InvokeRepeating(nameof(this.CheckStatus), 1.0f, 1.0f);
    }

    private void CheckStatus()
    {
        StartCoroutine(WebsiteConnection.GetTaskStatus((result) => this.OnCheckStatusResult(result)));
    }

    private void OnCheckStatusResult(TaskStatusResponse taskStatusResponse)
    {
        if (this.OnTaskStatusChange != null)
            this.OnTaskStatusChange.Invoke(taskStatusResponse);

        if (taskStatusResponse.task.id != this._currentTaskID) this.HandleNewActiveTask(taskStatusResponse);
    }

    private void HandleNewActiveTask(TaskStatusResponse taskStatusResponse)
    {
        Debug.Log("NEW ACTIVE TASK!");
        this._currentTaskID = taskStatusResponse.task.id;
        this.SpawnStartingBlock(taskStatusResponse.task.id);

    }

    private void SpawnStartingBlock(string taskID)
    {
        var startingBlockContainerToSpawn = this._startingBlocksData.Find((startingBlockData) => startingBlockData.TaskID == taskID);
        if (startingBlockContainerToSpawn == null) return;
        
        this._codeBlockManager.RemoveAllBlocksInScene();
        var startingBlockSpawned = Instantiate(startingBlockContainerToSpawn.BlocksContainer, this.transform.position, this.transform.rotation);
        
        foreach (var variableName in startingBlockContainerToSpawn.Variables)
        {
            this._variableManager.AddVariable(variableName);
        }

        this._codeBlockManager.AddExistingBlock(startingBlockSpawned.CodeBlocks);


        StartCoroutine(this.ConnectBlocks(startingBlockSpawned.ConnectionsAtStart));
    }

    private IEnumerator ConnectBlocks(List<ConnectionAtStart> connectionsAtStart)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var connectionAtStart in connectionsAtStart)
        {
            yield return new WaitForSeconds(0.1f);
            this._codeBlockConnectionManager.ConnectBlocks(connectionAtStart.From, connectionAtStart.To);
        }
    }

    public void LoadNextTask()
    {
        StartCoroutine(WebsiteConnection.LoadNextTask());
    }
}

[Serializable]
public class StartingBlockData 
{
    public string TaskID;

    public StartingBlocksContainer BlocksContainer;

    public List<string> Variables;
}