using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public Action<TaskStatusResponse> OnTaskStatusChange;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(this.CheckStatus), 1.0f, 1.0f);
    }

    private void CheckStatus()
    {
        StartCoroutine(WebsiteConnection.GetTaskStatus((result) => this.OnCheckStatusResult(result)));
    }

    private void OnCheckStatusResult(TaskStatusResponse response)
    {
        if (this.OnTaskStatusChange == null) return;
        this.OnTaskStatusChange.Invoke(response);
    }

    public void LoadNextTask()
    {
        StartCoroutine(WebsiteConnection.LoadNextTask());
    }
}
