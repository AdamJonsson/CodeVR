using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskStatusResponse
{
    public TaskStatus task;
    public bool isCompleted;
    public bool isLastTask;
}

[Serializable]
public class TaskStatus
{
    public string id;
    public string title;
    public string description;
    public string functionName;
    public List<string> variables;
    public List<TaskStatusTest> testCases;
}

[Serializable]
public class TaskStatusTest
{
    public List<string> inputs;
    public string output;
}