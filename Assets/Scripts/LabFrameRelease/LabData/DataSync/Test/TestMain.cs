using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataSync;
using DataSync.Test;
using UnityEngine;

public class TestMain : MonoBehaviour
{

    private bool isRunning = false;
    private static DataSyncClient _client;
    private LabDataScope scope;

    private SimpleApplicationLifecycle applicationLifecycle;

    //Git ignore
    private string testDataPath => Application.dataPath + "/TestData";
    // Use this for initialization
    void Start () {

        var options = new DataSyncClientOptions()
        {
            EndpointAddress = "http://localhost:4000/api/data",
            ProjectId = "testproject",
            LogFilePath = testDataPath +"/ log.txt"
        };

        //Docker
        options.EndpointAddress = "http://localhost/api/data";

        //server
        options.EndpointAddress = "http://api.mindfrog.cn/api/data";


        if (!Directory.Exists("TestStore"))
        {
            Directory.CreateDirectory("TestStore");
        }
        applicationLifecycle = new SimpleApplicationLifecycle();


        _client = new DataSyncClient(new UnityApplicationFolderProvider(testDataPath + "/TestStore"),
            applicationLifecycle, options,()=>"testUser");

        _client.Init();

        
        StartUpload();
      

    }

    // Update is called once per frame
    void Update () {

	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        if (!isRunning)
	        {
	            StartUpload();
	        }
	        else
	        {
	            StopUpload();
	        }
	    }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("发送数据");
            for (int i = 0; i < 1000; i++)
            {
                scope.Send(new TestData()
                {
                    X = UnityEngine.Random.Range(0, 10),
                    Y = UnityEngine.Random.Range(0, 10),
                    Z = UnityEngine.Random.Range(0, 10),
                });
            }
           
        }


    }

    void StartUpload()
    {
        if (isRunning)
        {
            return;
        }
        Debug.Log("开始");
        applicationLifecycle.OnStarted(EventArgs.Empty);
        scope = _client.CreateNewScope();
        scope.StartScope();
        isRunning = true;
    }

    void StopUpload()
    {
        if (!isRunning)
        {
            return;
        }
        Debug.Log("停止");
        scope.StopScope();
        scope.Dispose();
       
        
        isRunning = false;
    }

    void OnDestroy()
    {
        StopUpload();
        applicationLifecycle.OnStopping(ApplicationStoppingEventArgs.Empty);
    }

}
