using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
 
public abstract class MonoSingletonManager<T> : MonoBehaviour where T : MonoSingletonManager<T>
{
    protected static T instance = null;
 
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();
        }
 
        return instance;
    }
 
    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
public enum LogLevel
{
    LOG = 0,
    WARNING = 1,
    ERROR = 2,   
}
public class DebugHelper : MonoSingletonManager<DebugHelper>
{
    public int moduleName;  //0:fixations 1:saccades
    static public int queueCount = 0;
    public LogLevel SaveLogLevel;
    public bool IsSameFile;
    Queue<LogItem> m_vLogs;
    FileInfo m_logFileInfo;
    static bool m_isInited;
    string logFolder;
    string logFilename;
    static public string path;
 
    private Dictionary<LogType, LogLevel> logTypeLevelDict = null;
    private void Awake()
    {
        if (m_isInited)
        {
            Debug.Log("existed logfile object，break");
            Destroy(gameObject);
            return;
        }
        Init();
    }
    private void OnDestroy() 
    {
        //Application.logMessageReceived -= OnLogMessage;
        Application.logMessageReceivedThreaded -= OnLogMessageThread;
    }
    void FixedUpdate()
    {
        if (m_isInited)
        {
            this.Refresh(Time.fixedDeltaTime);
        }       
    }
 
    public void Init()
    {
        if (m_isInited)
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_isInited = true;
        logTypeLevelDict = new Dictionary<LogType, LogLevel>() {
                { LogType.Log, LogLevel.LOG },
                /*{ LogType.Warning, LogLevel.WARNING },
                { LogType.Assert, LogLevel.ERROR },
                { LogType.Error, LogLevel.ERROR },
                { LogType.Exception, LogLevel.ERROR }*/};
        // 创建文件
        DateTime timeNow = DateTime.Now;
#if UNITY_EDITOR
        logFolder = Application.dataPath+"/..";
#else
        logFolder = Application.persistentDataPath;
#endif
        if (IsSameFile)
        {
            path = logFolder + "/Log.txt";
        }else
        {
            if (moduleName == 0)
                path = logFolder + "/" + timeNow.ToString("yyyyMMddHHmm") + "fixations.txt";
            else
                path = logFolder + "/" + timeNow.ToString("yyyyMMddHHmm") + "saccades.txt";
            print("log file path:"+path);
        }
 
        m_logFileInfo = new FileInfo(path);
        var sw = m_logFileInfo.CreateText();
        sw.WriteLine("[{0}] - {1}", Application.productName, timeNow.ToString("yyyy/MM/dd HH:mm:ss"));
        sw.Close();
 
        // 注册回调
        m_vLogs = new Queue<LogItem>();
        //Application.logMessageReceived += OnLogMessage;
        Application.logMessageReceivedThreaded += OnLogMessageThread;
        //Debug.Log("Log文件已創建：" + path);
        Debug.Log("Log文件系統已啟動");
    }
 
 
    public void Refresh(float dt)
    {
        queueCount = m_vLogs.Count;
        if (m_vLogs.Count > 0)
        {
            try
            {
                var sw = m_logFileInfo.AppendText();
                var item = m_vLogs.Peek(); // 取队首元素但先不移除
                var timeStr = item.time.ToString("HH:mm:ss.ff");
                var logStr = string.Format("{0}-[{1}]{2}", timeStr, item.logType, item.messageString);
                //if (item.logType.Equals(LogType.Error))
                if(logTypeLevelDict[item.logType].Equals(LogLevel.ERROR))
                {
                    logStr = string.Format("{0}-[{1}]{2}==>{3}", timeStr, item.logType, item.messageString, item.stackTrace);
                }
                sw.WriteLine(logStr);
                sw.Close();
                m_vLogs.Dequeue(); // 成功执行了再移除队首元素
                if (item.messageString == "GameOver")
                {
                    SceneManager.LoadScene("DataAnalysis");
                    gameObject.SetActive(false);
                    //Application.Quit();
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.Message);
            }
        }
        //else Application.Quit();
    }
 
    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if(logTypeLevelDict[type]>= SaveLogLevel)
        {
            m_vLogs.Enqueue(new LogItem()
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
        }      
    }
 
    void OnLogMessageThread(string condition, string stackTrace, LogType type)
    {
        if (logTypeLevelDict[type] >= SaveLogLevel)
        {
            m_vLogs.Enqueue(new LogItem()
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
        }        
    }
}
 
public struct LogItem
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public string messageString;
 
    /// <summary>
    /// 调用堆栈
    /// </summary>
    public string stackTrace;
 
    /// <summary>
    /// 日志类型
    /// </summary>
    public LogType logType;
 
    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime time;
}