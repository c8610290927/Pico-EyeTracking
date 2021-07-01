//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using UnityEngine;
using UnityEngine.Assertions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySample : MonoBehaviour
            {
                Camera mainCamera;
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;

                private void Start()
                {
                    mainCamera = GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).GetComponent<Camera>();
                    
                    print("name: "+mainCamera.name);

                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);
                }

                private void Update()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;
                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    else return;
                    //Vector3 GazeDirectionCombined = mainCamera.transform.TransformDirection(GazeDirectionCombinedLocal);
                    Vector3 GazeDirectionCombined = mainCamera.transform.TransformDirection(Vector3.forward);
                    GazeRayRenderer.SetPosition(0, mainCamera.transform.position - mainCamera.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, mainCamera.transform.position + GazeDirectionCombined * LengthOfRay);
                    //Vector3 GazeDirectionCombined = GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).TransformDirection(GazeDirectionCombinedLocal);
                    //GazeRayRenderer.SetPosition(0, GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).position - GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).up * 0.05f);
                    //GazeRayRenderer.SetPosition(1, GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).position + GazeDirectionCombined * LengthOfRay);

                    //print("name"+mainCamera.name);
                }
            }
        }
    }
}
