using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabVisualization.EyeTracing
{
    [SerializeField]
    public class EyeTracingVisualConfig
    {
        public GameObject EyetracingTextureObject { get; private set; }
        public float EyetracingSmooth { get; private set; }

        public EyeTracingVisualConfig(GameObject o,float smooth)
        {
            EyetracingTextureObject = o;
            EyetracingSmooth = smooth;
        }
    }
}

