using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LabVisualization.EyeTracing
{
    public interface IEyeTracingPos
    {
        Func<Vector2> GetEyeTracingPos();
    }
}

