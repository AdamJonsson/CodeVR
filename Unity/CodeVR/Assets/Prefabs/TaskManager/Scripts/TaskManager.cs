using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public enum State {
        Loading,
        Ready
    }

    [SerializeField] private List<StartingBlockData> _startingBlocksData;

    [SerializeField] private bool _disable;

    public Action<TaskStatusResponse> OnTaskStatusChange;

    private string _currentTaskID = "";

    private CodeBlockConnectionManager _codeBlockConnectionManager;
    private CodeBlockManager _codeBlockManager;
    private VariableDeclarationManager _variableManager;

    private StartingBlockData _currentBlockDataSpawned;

    private State _currentState = State.Loading;
    public State CurrentState { get => this._currentState; }

    private string _taskLogsFilePath;

    private bool _taskIsCompleted = false;

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
        {
            this.CreateNewFile();
            InvokeRepeating(nameof(this.CheckStatus), 1.0f, 1.0f);
        }
    }

    private void CheckStatus()
    {
        StartCoroutine(WebsiteConnection.GetTaskStatus((result) => this.OnCheckStatusResult(result)));
    }

    private void OnCheckStatusResult(TaskStatusResponse taskStatusResponse)
    {
        if (this.OnTaskStatusChange != null)
            this.OnTaskStatusChange.Invoke(taskStatusResponse);

        if (!_taskIsCompleted && taskStatusResponse.isCompleted)
        {
            this._taskIsCompleted = true;
            this.LogTaskCompleted(taskStatusResponse);
            Debug.Log("Log Task Completed!");
        }

        if (_taskIsCompleted && !taskStatusResponse.isCompleted)
        {
            this._taskIsCompleted = false;
        }

        if (taskStatusResponse.task.id != this._currentTaskID) 
            this.HandleNewActiveTask(taskStatusResponse);
    }

    private void HandleNewActiveTask(TaskStatusResponse taskStatusResponse)
    {
        Debug.Log("NEW ACTIVE TASK!");
        this._currentTaskID = taskStatusResponse.task.id;
        this.SpawnStartingBlock(taskStatusResponse.task.id);
        this._currentState = State.Ready;
    }

    private void SpawnStartingBlock(string taskID)
    {
        this.RemoveOldVariablesFromLastSpawn();

        var startingBlockContainerToSpawn = this._startingBlocksData.Find((startingBlockData) => startingBlockData.TaskID == taskID);
        if (startingBlockContainerToSpawn == null) return;
        
        this._codeBlockManager.RemoveAllBlocksInScene();
        var startingBlockSpawned = Instantiate(startingBlockContainerToSpawn.BlocksContainer, this.transform.position, this.transform.rotation);
        
        foreach (var variableName in startingBlockContainerToSpawn.Variables)
        {
            this._variableManager.AddVariable(variableName);
        }

        this._codeBlockManager.AddExistingBlock(startingBlockSpawned.CodeBlocks);

        this._currentBlockDataSpawned = startingBlockContainerToSpawn;
        StartCoroutine(this.ConnectBlocks(startingBlockSpawned.ConnectionsAtStart));
    }

    private void RemoveOldVariablesFromLastSpawn()
    {
        if (this._currentBlockDataSpawned == null) return;
        foreach (var variableName in this._currentBlockDataSpawned.Variables)
        {
            this._variableManager.RemoveVariableByName(variableName);
        }
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
        this._currentState = State.Loading;
        StartCoroutine(WebsiteConnection.LoadNextTask());
    }

    private void CreateNewFile()
    {
        Debug.Log(Application.persistentDataPath);
        var currentDate = DateTime.Now.ToFileTime();
        this._taskLogsFilePath = Application.persistentDataPath + "/task_logger_" + currentDate + ".txt";
        File.Create(this._taskLogsFilePath);
    }

    private void LogTaskCompleted(TaskStatusResponse response)
    {
        StreamWriter sw = new StreamWriter(this._taskLogsFilePath, true);
        sw.Write($"Task completed at;{Time.timeSinceLevelLoad};TaskID;{response.task.id};TaskTitle;{response.task.title}\n");
        sw.Close();
    }
}

[Serializable]
public class StartingBlockData 
{
    public string TaskID;

    public StartingBlocksContainer BlocksContainer;

    public List<string> Variables;
}