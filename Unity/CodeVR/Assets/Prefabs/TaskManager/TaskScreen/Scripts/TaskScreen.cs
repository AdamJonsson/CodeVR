using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScreen : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _title;
    [SerializeField] private TMPro.TMP_Text _description;
    [SerializeField] private TMPro.TMP_Text _testStatus;
    [SerializeField] private TMPro.TMP_Text _inputs;
    [SerializeField] private TMPro.TMP_Text _expectedOutput;
    [SerializeField] private TMPro.TMP_Text _currentOutput;

    private TaskManager _taskManager;

    // Start is called before the first frame update
    void Start()
    {
        this._taskManager = FindObjectOfType<TaskManager>();
        this._taskManager.OnTaskStatusChange += this.OnTaskStatusChange;
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        this._title.text = taskStatus.task.title;
        this._description.text = taskStatus.task.description;

        if (taskStatus.isCompleted)
        {
            this._testStatus.text = "Task completed";
            this._inputs.text = "";
            this._expectedOutput.text = "";
            this._currentOutput.text = "";

            this._testStatus.color = new Color(0.0f, 0.7f, 0.0f, 1.0f);
        }
        else
        {
            this._testStatus.text = "Tests failed when:";
            this._inputs.text = taskStatus.failedTest?.inputs ?? "";
            this._expectedOutput.text = taskStatus.failedTest.output;
            this._currentOutput.text = taskStatus.currentOutput;
            
            this._testStatus.color = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
