using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PositionCalculator : MonoBehaviour
{
    [SerializeField] TextAsset _positionDataToCalculate;

    [SerializeField] int _visualizeDataDurationSeconds = 2;

    [Range(0.0f, 1.0f)]    
    [SerializeField] float _visualizeDataStart = 0.0f;

    [SerializeField] float _visualizeDataSampleRate = 1.0f;

    [Tooltip("Scale the velocity line with the specified factor to make it easier to see")]
    [SerializeField] float _visualizeVelocityLineScale = 1.0f;

    [Tooltip("Scale the acceleration line with the specified factor to make it easier to see")]
    [SerializeField] float _visualizeAccelerationLineScale = 1.0f;

    private List<DeviceTrackingDataPoint> _headsetDataPointsToDraw = new List<DeviceTrackingDataPoint>();

    public DeviceTrackingData Headset = new DeviceTrackingData();

    // public static void WriteString()
    // {
    //    string path = Application.persistentDataPath + "/test.txt";
    //    //Write some text to the test.txt file
    //    StreamWriter writer = new StreamWriter(path, true);
    //    writer.WriteLine("Test");
    //     writer.Close();
    //    StreamReader reader = new StreamReader(path);
    //    //Print the text from the file
    //    Debug.Log(reader.ReadToEnd());
    //    reader.Close();
    // }

    public static void ReadString()
    {
        // string path = Application.persistentDataPath + "/test.txt";
        // //Read the text from directly from the test.txt file
        // _positionDataToCalculate.text
        // StreamReader reader = new StreamReader(path);
        // Debug.Log(reader.ReadToEnd());
        // reader.Close();
    }

    public void CalculatePositionData()
    {
        var lines = this._positionDataToCalculate.text.Split("\n").ToList();
        lines.RemoveAt(0);

        var headsetPositions = new List<VectorWithTime>();
        foreach (var line in lines)
        {
            var dataSets = line.Split(";");
            var time = float.Parse(dataSets[0]) / 1000.0f;
            var headsetPositionRotationVector = dataSets[1];
            var leftHandPositionRotationVector = dataSets[2];
            var rightHandPositionRotationVector = dataSets[3];

            var headsetPosition = this.ParseTextVector(time, headsetPositionRotationVector);
            headsetPositions.Add(headsetPosition);
        }

        this.Headset.UpdatePositionData(headsetPositions);
        this._headsetDataPointsToDraw = this.Headset.GetDataPointsSampled(this._visualizeDataSampleRate, DeviceTrackingData.SampleMode.Avarage);
    }

    private VectorWithTime ParseTextVector(float time, string textVector)
    {
        var xPos = textVector.IndexOf("x");
        var yPos = textVector.IndexOf("y");
        var zPos = textVector.IndexOf("z");
        var rxPos = textVector.IndexOf("rx");

        var xValue = textVector.Substring(xPos + 3, yPos - xPos - 5);
        var yValue = textVector.Substring(yPos + 3, zPos - yPos - 5);
        var zValue = textVector.Substring(zPos + 3, rxPos - zPos - 5);

        return new VectorWithTime() {
            Time = time,
            Value = new Vector3(float.Parse(xValue, CultureInfo.InvariantCulture), float.Parse(yValue, CultureInfo.InvariantCulture), float.Parse(zValue, CultureInfo.InvariantCulture))
        };
    }

    void OnDrawGizmos()
    {
        this.DrawHeadsetPositions();
    }

    private void DrawHeadsetPositions()
    {
        var index = 0;
        var pointsToDraw = this._headsetDataPointsToDraw.Where((point) => this.ShouldDrawPoint(point, this._headsetDataPointsToDraw)).ToList();
        foreach (var dataPoint in pointsToDraw)
        {
            Gizmos.color = Color.red;
            var sizeInterpolation = (float)index / (float) pointsToDraw.Count;
            Gizmos.DrawSphere(dataPoint.Position, 0.03f * sizeInterpolation + 0.005f * (1 - sizeInterpolation));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(dataPoint.Position, dataPoint.Position + dataPoint.Velocity * this._visualizeVelocityLineScale);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(dataPoint.Position, dataPoint.Position + dataPoint.Acceleration * this._visualizeAccelerationLineScale);

            index++;
        }
    }

    private bool ShouldDrawPoint(DeviceTrackingDataPoint dataPoint, List<DeviceTrackingDataPoint> allDataPoints)
    {
        var lastTime = allDataPoints.Last().Time;
        var maxDuration = this._visualizeDataStart * lastTime;
        var minDuration = maxDuration - this._visualizeDataDurationSeconds;

        return dataPoint.Time > minDuration && dataPoint.Time < maxDuration;
    }
}

public class VectorWithTime
{
    public Vector3 Value { get; set; }

    /// <summary>Time in seconds</summary>
    public float Time { get; set; }
}

public class VelocityVector
{
    public VectorWithTime Velocity { get; set; }
    public Vector3 Origin { get; set; }
}

public class DeviceTrackingDataPoint
{
    /// <summary>Time in seconds</summary>
    public float Time;
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public class DeviceTrackingData
{
    private List<DeviceTrackingDataPoint> _dataPoints = new List<DeviceTrackingDataPoint>();

    public List<DeviceTrackingDataPoint> DataPoints => this._dataPoints;

    public enum SampleMode
    {
        Avarage,
        AbsoluteAvarage
    } 

    public void UpdatePositionData(List<VectorWithTime> positions)
    {
        List<DeviceTrackingDataPoint> dataPoints = new List<DeviceTrackingDataPoint>();
        var filteredPositions = this.FilterOutSmallChanges(positions);
        for (int i = 0; i < filteredPositions.Count - 2; i++)
        {
            var positionOne = filteredPositions[i];
            var positionTwo = filteredPositions[i + 1];
            var positionThree = filteredPositions[i + 2];


            var velocityOne = CalculateChangePerSecond(positionOne, positionTwo);
            var velocityTwo = CalculateChangePerSecond(positionTwo, positionThree);

            var acceleration = CalculateChangePerSecond(velocityOne, velocityTwo);

            dataPoints.Add( new DeviceTrackingDataPoint() {
                Time = positionTwo.Time,
                Position = positionTwo.Value,
                Velocity = (velocityOne.Value + velocityTwo.Value) / 2.0f,
                Acceleration = acceleration.Value
            });
        }
        this._dataPoints = dataPoints;
    }

    private List<VectorWithTime> FilterOutSmallChanges(List<VectorWithTime> positions)
    {
        List<VectorWithTime> filteredPositions = new List<VectorWithTime>();
        VectorWithTime lastPosition = positions.First();
        var minDistance = 0.01f;
        foreach (var position in positions)
        {
            var distance = Vector3.Distance(position.Value, lastPosition.Value);
            if (distance < minDistance) continue;
            filteredPositions.Add(position);

            lastPosition = position;
        }

        return filteredPositions;
    }

    private VectorWithTime CalculateChangePerSecond(VectorWithTime vectorTimeOne, VectorWithTime vectorTimeTwo)
    {
        Vector3 change = vectorTimeTwo.Value - vectorTimeOne.Value;
        float durationInSeconds = vectorTimeTwo.Time - vectorTimeOne.Time;
        float time = vectorTimeOne.Time +  durationInSeconds / 2.0f;
        Vector3 changeVector = change / Mathf.Max(durationInSeconds, 0.001f);
        return new VectorWithTime() { Time = time, Value = changeVector};
    }

    public List<DeviceTrackingDataPoint> GetDataPointsSampled(float sampleRateSeconds, SampleMode sampleMode)
    {
        var dataPointsAvarage = new List<DeviceTrackingDataPoint>();
        if (DataPoints.Count == 0) return dataPointsAvarage;

        int numberOfSamples = Mathf.Max(Mathf.FloorToInt(DataPoints.Last().Time / sampleRateSeconds), 1);
        var lastIndexOfDataPoint = 0;
        for (int i = 0; i < numberOfSamples; i++)
        {
            var positions = new List<Vector3>();
            var velocities = new List<Vector3>();
            var accelerations = new List<Vector3>();
            var times = 0.0f;
            var numberOfDataPointsInSample = 0;
            foreach (var dataPoint in DataPoints.Skip(lastIndexOfDataPoint))
            {
                times += dataPoint.Time;
                positions.Add(dataPoint.Position);
                velocities.Add(dataPoint.Velocity);
                accelerations.Add(dataPoint.Acceleration);
                numberOfDataPointsInSample++;
                if (dataPoint.Time > sampleRateSeconds * (i + 1)) break;
            }

            if (numberOfDataPointsInSample == 0) continue;

            if (sampleMode == SampleMode.Avarage)
            {
                dataPointsAvarage.Add(new DeviceTrackingDataPoint() {
                    Time = times / numberOfDataPointsInSample,
                    Position = this.AvarageVector(positions),
                    Velocity = this.AvarageVector(velocities),
                    Acceleration = this.AvarageVector(accelerations)
                });
            }

            if (sampleMode == SampleMode.AbsoluteAvarage)
            {
                dataPointsAvarage.Add(new DeviceTrackingDataPoint() {
                    Time = times / numberOfDataPointsInSample,
                    Position = this.AvarageVector(positions),
                    Velocity = this.AbsoluteAvarageVector(velocities),
                    Acceleration = this.AbsoluteAvarageVector(accelerations)
                });
            }



            lastIndexOfDataPoint += numberOfDataPointsInSample;
        }

        return dataPointsAvarage;
    }

    private Vector3 AvarageVector(List<Vector3> vectors)
    {
        return vectors.Aggregate(new Vector3(0,0,0), (s,v) => s + v) / (float) vectors.Count;
    }

    private Vector3 AbsoluteAvarageVector(List<Vector3> vectors)
    {
        return vectors.Aggregate(new Vector3(0,0,0), (s,v) => s + new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z))) / (float) vectors.Count;
    }

}