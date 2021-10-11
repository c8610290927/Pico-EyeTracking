using System;
using System.Collections.Generic;
using DataSync;
using DataSync.Links;
using Newtonsoft.Json;
using UnityEngine;
using OpenTelemetry.Exporter.WithLocalStorage;
using System.Linq;
using System.Collections.Specialized;
using CommandLine;
using UnityEngine.Networking;
using System.Text;
using System.Diagnostics.Tracing;

public class ClientDataSyncLink
{
    private DataSyncClient client;

    private string testDataPath => Application.persistentDataPath + "/TestData";


    public string CurrentScopeId { get; set; } = null;

    public DataSyncScopedClient Scope { get; set; }


    public AppStartOptions StartOptions = null;


    public ProgressObject ProgressObject { get; set; }


    public bool IsClintInit { get; set; } = false;


    #region Client

    public T GetGameInputData<T>() where T : class, IScopeInputRoot, ILabDataEntity
    {
        if (StartOptions?.ScopeInputTaskScriptContent != null)
        {
            try
            {
                Debug.LogError(StartOptions.ScopeInputTaskScriptContent + "______________________________");
                return JsonConvert.DeserializeObject<T>(StartOptions.ScopeInputTaskScriptContent);
                //TODO 获取输入调整参数
            }
            catch (Exception e)
            {
                return null;
            }
        }

        else
            return null;
    }


    /// <summary>
    /// 客户端实例化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resourcePath"></param>
    public void SyncClientInit<T>(string resourcePath) where T : class, IScopeInputRoot, ILabDataEntity
    {
        GetWebUrlData();

        Debug.Log("[INFO] SyncClientInit");
        //var clientConfig = JsonToDictionary(Resources.Load<TextAsset>(resourcePath).text);

        //这里参数可以从管理平台上https://saas.o-health.cn/app-management/projectChannel点击某个游戏的Channel,下载开发配置文件中找到
        var options = new DataSyncClientOptions(
            //这个参数需要从游戏启动参数里面找，如果没有的话，设置为null或者空string
            tenantIdentifier: "",
            //配置文件中找到
            projectIdentifier: "ConcussionEyeTracking",
            //软件的版本号
            projectVersion: "0.1.0",
            //配置文件中找到
            projectPublicKey: null,
            //配置文件中找到
            projectChannel: "QQQ",
            //日志文件位置，注意要使用可读写路径
            logFilePath: Application.persistentDataPath + "/Log/Log.txt", logLevel: EventLevel.Informational,
            //本地数据储存位置，注意要使用可读写路径
            localStorageFolderPath: testDataPath,
            mode: (Application.isEditor || Debug.isDebugBuild) ? ProjectMode.Development : ProjectMode.Production,
            //api端点。配置文件中找到
            endPointAddress: "http://140.115.54.20:5000/api/v1/eye-tracking/receive");
            //endPointAddress: "http://140.115.54.20:5000/api/v1/test");

        //从命令行参数中覆盖
        if (StartOptions != null)
        {
            try
            {
                options.PatchAndValidateFromOption(StartOptions);
            }
            catch (ArgumentException e)
            {
                //这里其实说明启动错了游戏,但是先不管
                Debug.LogError("启动错误游戏");
            }
        }
        
        Debug.Log("[INFO] StartOptions");

        //如何获取当前用户Id的方法
        //var userLink = new UserIdentifierLink(() => StartOptions?.UserId);
        var userLink = new UserIdentifierLink(() => GameDataManager.FlowData.UserId);

        Debug.Log("[INFO] Create client");
        
        //软件应该保留一个唯一的单例
        client = new DataSync.DataSyncClient(options, userLink);
        //初始化，一定要在软件启动的时候调用
        
        Debug.Log("[INFO] Create client init");
        try
        {
            client.Initialize();
            Debug.Log("Client 初始化成功");
        }
        catch (ArgumentException e)
        {
            Debug.Log("Client 初始化失败" + e);
        }
        
        Debug.Log("[INFO] Create client OK");
        
        //            //可以不在一个游戏范围内发送数据，比如说发送软件启动的相关telemetry
        //client.Send(new TestData());

        Debug.Log(StartOptions?.ScopeId + "_________这里是ScopeId——————01");

        CurrentScopeId = StartOptions?.ScopeId ?? System.Guid.NewGuid().ToString("N");

        ProgressObject = client.ReadProgress();

        IsClintInit = true;
        Debug.Log("[INFO] SyncClientInit END");
    }

    /// <summary>
    /// 客户端注销
    /// </summary>
    public void ClientDataSyncLinkOnDestroy()
    {
        client?.UnInitialize();
    }


    private void GetWebUrlData()
    {
        Debug.Log("解析传递数据");
        string CommandLine = Environment.CommandLine;
        string[] CommandLineArgs = Environment.GetCommandLineArgs();
        Debug.Log(CommandLineArgs.Length);
        StartOptions = null;
        var path = CommandLineArgs.Length > 1 ? CommandLineArgs[1] : null;

        Debug.Log("INFO GetWebUrlData Init");
        
        try
        {
            if (path != null)
            {
                var index = path.IndexOf("?");
                if (index != -1)
                {
                    //说明有接收到值
                    var query = path.Substring(index, path.Length - index);
                    NameValueCollection queryParameters = new NameValueCollection();
                    string[] querySegments = query.Split('&');
                    foreach (string segment in querySegments)
                    {
                        string[] parts = segment.Split('=');
                        if (parts.Length > 0)
                        {
                            string key = parts[0].Trim(new char[] {'?', ' '});
                            string val = parts[1].Trim();
                            queryParameters.Add(key, val);
                        }
                    }

                    List<string> arglist = new List<string>();
                    foreach (var p in queryParameters.AllKeys)
                    {
                        arglist.Add(
                            $"-{p} {UnityWebRequest.UnEscapeURL(queryParameters.GetValues(p).Last(), Encoding.UTF8)}");
                    }

                    //将queryParameters转化为AppStartOption
                    Parser.Default.ParseArguments<AppStartOptions>(arglist)
                        .WithParsed<AppStartOptions>(o =>
                        {
                            StartOptions = o;
                            StartOptions.Normalize();
                        });

                    Debug.Log(StartOptions?.ScopeId + "_________这里是ScopeId——————00");
                }
                else
                {
                    //没有找到index
                }
            }
            
        }
        catch (Exception e)
        {
            Debug.Log("INFO GetWebUrlData Fail");
            Debug.LogError(e);
        }
        Debug.Log("[INFO] GetWebUrlData OK");
    }


    private Dictionary<string, string> JsonToDictionary(string jsonStr)
    {
        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        return dic;
    }

    #endregion


    #region 任务 Scope 部分

    public void TaskScopeInit()
    {
        Debug.Log("[create]: CurrentScopeId" + CurrentScopeId);
        //创建一个游戏的任务范围，通过这个scope发送的数据都会自动加上游戏任务的Id,通常这个ScopeId来自服务器
        Scope = client.CreateNewScope(CurrentScopeId);
        Debug.Log("[create]: CreateNewScope   被调用了");
        Scope.Start();
        Debug.Log("[create]: 重新开始了ScopeStart");
    }

    public void TaskScopeStop()
    {
        Debug.Log("[create]: TaskScopeStop");
        client.ForceFlush();
        Scope?.Dispose();
    }


    /// <summary>
    /// 数据传送进度
    /// </summary>
    /// <returns></returns>
    public float GetScopeUploadProgress()
    {
        ProgressObject = client.ReadProgress();

        var total = ProgressObject.RemainSize + ProgressObject.SentSize;

        var p = client.ReadProgress();

        // if (Debug.isDebugBuild)
        //     Debug.Log(
        //         $"<b>[LabData]</b> \rProgress:{(total == 0 ? 0 : ((p.SentSize / (double) total)) * 100).ToString("f2")} % " +
        //         $"\r SentSize {p.SentSize} " +
        //         $"\r RemainSize {p.RemainSize}" +
        //         $"\r StorageSize {p.StorageSize}" +
        //         $"\r BufferSize {p.BufferSize}" +
        //         $"\r SenderSize {p.SenderSize}"
        //     );

        if (total != 0)
        {
            return (float) (ProgressObject.SentSize / (double) total);
        }
        else
            return 0;
    }


    /// <summary>
    /// 数据是都上传完成
    /// </summary>
    /// <returns></returns>
    public bool GetScopeSendOver()
    {
        return GetScopeUploadProgress() == 1 ? true : false;
    }

    #endregion
}