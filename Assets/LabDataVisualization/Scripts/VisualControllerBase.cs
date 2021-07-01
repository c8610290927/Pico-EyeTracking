using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabVisualization
{
    public abstract class VisualControllerBase:MonoBehaviour
    {
        public abstract void VisualInit();
        public abstract void VisualShow();
        public abstract void VisualDispose();

    }

}
