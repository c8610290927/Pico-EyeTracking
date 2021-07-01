using System;
using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    /// <summary>
    /// Add click functionalities to monobehaviours
    /// </summary>
    public class ClickManager : MonoBehaviour
    {
        public double MaxTimeToClick = 0.5;
        public bool IsDebug;
        private TimeSpan _maxDuration;
        
        private DateTime _previousClick;
        
        private void Awake()
        {
            _previousClick = DateTime.Now;
            _maxDuration = TimeSpan.FromSeconds(MaxTimeToClick);
        }

        public void DoubleClick()
        {
            if (DateTime.Now - _previousClick < _maxDuration)
            {
                if (IsDebug)
                    Debug.Log("Double Click");

                GetComponent<IDoubleClickAction>().PerformDoubleClickAction();
            }
            else
            {
                if (IsDebug)
                    Debug.Log("Time out");
            }

            _previousClick = DateTime.Now;
        }
    }
}