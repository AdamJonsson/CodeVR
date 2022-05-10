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

        if(GUILayout.Button("Save raw acceleration data"))
        {
            this.SaveRawAccelerationData();
        }
    }

    private void SaveHeadsetData()
    {
        PositionCalculator positionCalculator = (PositionCalculator)target;
        var path = this.OpenSaveFileDialog();
        var dataPoints = positionCalculator.Headset.GetDataPointsSampled(1000, DeviceTrackingData.SampleMode.AbsoluteAvarage);

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Time,AccX,AccY,AccZ");

        var invC = CultureInfo.InvariantCulture;
        foreach (var dataPoint in dataPoints)
        {
            writer.WriteLine($"{dataPoint.Time.ToString(invC)},{dataPoint.Acceleration.x.ToString(invC)},{dataPoint.Acceleration.y.ToString(invC)},{dataPoint.Acceleration.z.ToString(invC)}");
        }
        writer.Close();
    }

    private void SaveRawAccelerationData()
    {
        PositionCalculator positionCalculator = (PositionCalculator)target;
        var path = this.OpenSaveFileDialog();
        var dataPoints = positionCalculator.Headset.DataPoints;

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Time,AccX,AccY,AccZ");

        var invC = CultureInfo.InvariantCulture;
        foreach (var dataPoint in dataPoints)
        {
            writer.WriteLine($"{dataPoint.Time.ToString(invC)},{(dataPoint.Acceleration.x / 9.8f).ToString(invC)},{(dataPoint.Acceleration.y / 9.8f + 1.0f).ToString(invC)},{(dataPoint.Acceleration.z / 9.8f).ToString(invC)}");
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
