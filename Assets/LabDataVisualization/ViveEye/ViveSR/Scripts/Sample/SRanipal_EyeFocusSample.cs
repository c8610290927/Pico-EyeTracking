//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using UnityEngine;
using LabData;

namespace ViveSR.anipal.Eye
{
    public class SRanipal_EyeFocusSample : MonoBehaviour
    {
        private FocusInfo FocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

        public Vector2 EyeData { get; set; }
        SingleEyeData LeftData { get; set; }
        SingleEyeData RightData { get; set; }

        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            /*EyePositionData eyepositiondata = new EyePositionData() //記錄eyedata
            {
                words = "ouo"
            };
            GameDataManager.LabDataManager.SendData(eyepositiondata);
            print(eyepositiondata.words);*/

            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                if (SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, MaxDistance))
                {
                    DartBoard dartBoard = FocusInfo.transform.GetComponent<DartBoard>();
                    if (dartBoard != null) dartBoard.Focus(FocusInfo.point);
                    break;
                }
            }
        }
    }
}