using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var cameraPosition = Camera.main.transform.position;
        this.transform.LookAt(cameraPosition, Vector3.up);
    }
}
