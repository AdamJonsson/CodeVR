using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(this.CheckStatus), 1.0f, 1.0f);
    }

    private void CheckStatus()
    {
        Debug.Log("Running!");
        StartCoroutine(WebsiteConnection.GetTaskStatus((result) => this.OnCheckStatusResult(result)));
    }

    private void OnCheckStatusResult(TaskStatusResponse response)
    {
        Debug.Log(response.task.functionName);
        Debug.Log("Task is completed: " + response.isCompleted);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
