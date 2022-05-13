using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Globalization;

[CustomEditor(typeof(PositionCalculator))]
public class PositionCalculatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PositionCalculator positionCalculator = (PositionCalculator)target;
        if(GUILayout.Button("Calculate position data"))
        {
            positionCalculator.CalculatePositionData();
        }

        if(GUILayout.Button("Save acceleration data"))
        {
            this.SaveHeadsetData();
        }
    }

    private void SaveHeadsetData()
    {
        PositionCalculator positionCalculator = (PositionCalculator)target;
        var path = this.OpenSaveFileDialog();
        var dataPoints = positionCalculator.Headset.GetDataPointsSampled(1.0f, DeviceTrackingData.SampleMode.AbsoluteAvarage);

        //Write some text to the test.txt file
        Debug.Log(dataPoints.Count);

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Time,AccX,AccY,AccZ");

        var invC = CultureInfo.InvariantCulture;
        foreach (var dataPoint in dataPoints)
        {
            writer.WriteLine($"{dataPoint.Time.ToString(invC)},{dataPoint.Acceleration.x.ToString(invC)},{dataPoint.Acceleration.y.ToString(invC)},{dataPoint.Acceleration.z.ToString(invC)}");
        }
        writer.Close();
    }

    private string OpenSaveFileDialog()
    {
        var path = EditorUtility.SaveFilePanel(
            "Save acceleration data",
            "",
            "AccelerationData" + ".txt",
            "txt");

        return path;
    }


}
