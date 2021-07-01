
using UnityEngine;
using System;

namespace LabVisualization.Cardiogram
{
    /// <summary>
    /// 心电图，皮肤电坐标
    /// x=心电图坐标，y=皮肤电坐标
    /// </summary>
    public interface CardiogramPos
    {
        Func<Vector3> GetCardiogramPos();
    }
}
