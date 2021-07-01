using System;
using System.Collections;
using System.Collections.Generic;
using LabVisualization.EyeTracing;
using LabVisualization.Cardiogram;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabVisualization
{
    public class Demo : MonoBehaviour
    { 
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
            VisualizationManager.Instance.VisulizationInit();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                VisualizationManager.Instance.StartDataVisualization();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
               VisualizationManager.Instance.DisposeDataVisualization();
            }
        }
    }
}

