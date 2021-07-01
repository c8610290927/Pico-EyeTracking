using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabVisualization.Cardiogram
{
    [SerializeField]
    public class CardiogramVisualConfig
    {
        public int EnergyLineWidth { get; private set; }
        public int PointsInEnergyLine { get; private set; }

        public CardiogramVisualConfig(int Width, int Points)
        {
            EnergyLineWidth = Width;
            PointsInEnergyLine = Points;
        }
    }
}
