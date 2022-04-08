using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScreen : MonoBehaviour
{
    [SerializeField] private GameObject _taskStatusContainer;
    [SerializeField] private GameObject _taskCompletedContainer;
    [SerializeField] private GameObject _taskLoadingContainer;
    
    [SerializeField] private TMPro.TMP_Text _title;
    [SerializeField] private TMPro.TMP_Text _description;
    [SerializeField] private TMPro.TMP_Text _testStatus;
    [SerializeField] private TMPro.TMP_Text _inputs;
    [SerializeField] private TMPro.TMP_Text _expectedOutput;
    [SerializeField] private TMPro.TMP_Text _currentOutput;

    [SerializeField] private AudioSource _audioSource;

    private TaskManager _taskManager;

    private bool _taskCompletedHasHappened = false;

    // Start is called before the first frame update
    void Start()
    {
        this._taskManager = FindObjectOfType<TaskManager>();
        this._taskManager.OnTaskStatusChange += this.OnTaskStatusChange;
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        this.CheckForTaskComplated(taskStatus);
        
        this._title.text = taskStatus.task.title;
        this._description.text = taskStatus.task.description;

        this._taskCompletedContainer.SetActive(
            taskStatus.isCompleted && this._taskManager.CurrentState == TaskManager.State.Ready
        );
        this._taskLoadingContainer.SetActive(
            this._taskManager.CurrentState == TaskManager.State.Loading
        );
        this._taskStatusContainer.SetActive(
            !taskStatus.isCompleted && this._taskManager.CurrentState == TaskManager.State.Ready
        );
        
        this._testStatus.text = "Tests failed when:";
        this._inputs.text = taskStatus.failedTest?.inputs ?? "";
        this._expectedOutput.text = taskStatus.failedTest.output;
        this._currentOutput.text = taskStatus.currentOutput;
    }

    private void CheckForTaskComplated(TaskStatusResponse taskStatus)
    {
        if (!this._taskCompletedHasHappened && taskStatus.isCompleted)
        {
            this._taskCompletedHasHappened = true;
            this.OnTaskCompleted();
        }
        if (!taskStatus.isCompleted)
            this._taskCompletedHasHappened = false;
    }

    private void OnTaskCompleted()
    {
        this._audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
