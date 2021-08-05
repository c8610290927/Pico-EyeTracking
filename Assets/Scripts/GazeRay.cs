using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GazeRay : MonoBehaviour
{
    // Start is called before the first frame update
    Camera mainCamera;
    public int LengthOfRay = 25;
    [SerializeField] private LineRenderer GazeRayRenderer;
    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;

    private void Start()
    {
        mainCamera = GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).GetComponent<Camera>();
        
        //print("name: "+mainCamera.name);

        Assert.IsNotNull(GazeRayRenderer);
    }

    private void Update()
    {
        //Vector3 GazeDirectionCombined = mainCamera.transform.TransformDirection(GazeDirectionCombinedLocal);
        Vector3 GazeDirectionCombined = gazeRay.Direction;
        GazeRayRenderer.SetPosition(0, mainCamera.transform.position - mainCamera.transform.up * 0.05f);
        GazeRayRenderer.SetPosition(1, mainCamera.transform.position + GazeDirectionCombined * LengthOfRay);
        //Vector3 GazeDirectionCombined = GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).TransformDirection(GazeDirectionCombinedLocal);
        //GazeRayRenderer.SetPosition(0, GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).position - GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).up * 0.05f);
        //GazeRayRenderer.SetPosition(1, GameObject.Find("Pvr_UnitySDK").transform.GetChild(1).position + GazeDirectionCombined * LengthOfRay);

        //print("name"+mainCamera.name);
    }
}
