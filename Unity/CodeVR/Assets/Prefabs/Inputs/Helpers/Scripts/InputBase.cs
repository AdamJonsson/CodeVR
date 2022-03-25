using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBase : MonoBehaviour
{
    public abstract string Value { get; }

    public abstract RectTransform RectTransform { get; }

    public Action<string> OnChange;
}
