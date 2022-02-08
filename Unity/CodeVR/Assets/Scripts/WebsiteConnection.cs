using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

}