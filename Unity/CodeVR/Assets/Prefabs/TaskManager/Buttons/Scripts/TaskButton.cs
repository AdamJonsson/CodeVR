using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private GameObject _unlockIcon;

    private TaskManager _taskManager;

    // Start is called before the first frame update
    void Start()
    {
        this._taskManager = FindObjectOfType<TaskManager>();
        this._taskManager.OnTaskStatusChange += this.OnTaskStatusChange;
        this._button.onClick.AddListener(OnClick);
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        Debug.Log(taskStatus.isCompleted);
        this.ToggleDisable(!taskStatus.isCompleted);
    }

    private void ToggleDisable(bool disable)
    {
        this._button.interactable = !disable;
        this._lockIcon.SetActive(disable);
        this._unlockIcon.SetActive(!disable);
    }

    private void OnClick()
    {
        this.ToggleDisable(true);
        this._taskManager.LoadNextTask();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
