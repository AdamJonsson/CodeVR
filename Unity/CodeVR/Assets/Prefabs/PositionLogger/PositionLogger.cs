using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionLogger : MonoBehaviour
{
    [SerializeField] private List<PositionLogObject> _logObjects = new List<PositionLogObject>();

    private StreamWriter _streamWriter;

    // Start is called before the first frame update
    void Start()
    {
        this.CreateNewFile();
    }

    private void CreateNewFile()
    {
        var filePath = FilePath();
        var fileStream = File.Create(filePath);
        Debug.Log("Created a log file for position data at: " + filePath);
        this._streamWriter = new StreamWriter(fileStream);

        this._streamWriter.Write("Ticks;");
        foreach (var logObject in this._logObjects)
        {
            this._streamWriter.Write(logObject.Name + ";");
        }
        this._streamWriter.Write("\n");
    }

    private string FilePath()
    {
        var currentDate = DateTime.Now.ToFileTime();
        return Application.persistentDataPath + "/position_logger_" + currentDate + ".txt";
    }

    // Update is called once per frame
    void Update()
    {
        var currentDate = DateTime.Now.Ticks;
        this._streamWriter.Write(currentDate + ";");
        foreach (var logObject in this._logObjects)
        {
            var position = logObject.GameObject.transform.position;
            var rotation = logObject.GameObject.transform.rotation.eulerAngles;
            this._streamWriter.Write($"(x: {position.x}, y: {position.y}, z: {position.z}, rx: {rotation.x}, ry: {rotation.y}, rz: {rotation.z});");
        }
        this._streamWriter.Write("\n");
    }
}

[Serializable]
public struct PositionLogObject
{
    public string Name;
    public GameObject GameObject;
}