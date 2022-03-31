using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class WebsiteConnection
{
    private static readonly int PORT = 8999;
    private static readonly string ADRESS = "http://" + Config.WebsiteIPAdress;

    private static string BaseAdress { get => ADRESS + ":" + PORT.ToString(); }

    public static IEnumerator UpdateBlocklyCode(string blocklyXML)
    {
        WWWForm form = new WWWForm();
        form.AddField("blocklyXML", blocklyXML);

        using (UnityWebRequest www = UnityWebRequest.Post(BaseAdress + "/api/notify-code-change", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Successfully updated website with new blockly code");
            }
        }
    }

    public static IEnumerator GetTaskStatus(Action<TaskStatusResponse> OnResult)
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Get(BaseAdress + "/api/current-task-status"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var parsedResponse = JsonUtility.FromJson<TaskStatusResponse>(www.downloadHandler.text);
                OnResult.Invoke(parsedResponse);
            }
        }
    }

    public static IEnumerator LoadNextTask()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(BaseAdress + "/api/move-to-next-task", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Successfully updated website with new blockly code");
            }
        }
    }

}