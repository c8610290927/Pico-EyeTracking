using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSync;
using UnityEngine;



namespace LabData
{
    public enum SaveType
    {
        Json,
        Csv
    }

    public sealed class LabDataManager : ILabDataManager
    {
        //private SaveType _saveType = SaveType.Json;
        [SerializeField] private bool _sendToServer = false;

        public bool IsClientRunning { get; private set; }
        private bool _isClientInit = false;
        private static DataSyncClient _client;
        private string _saveDataPath;
        private LabDataScope Scope { get; set; }
        private Func<string> _userId;
        private SimpleApplicationLifecycle _applicationLifecycle;
        private string labDataSavePath => Application.dataPath + "/TestData";
        // private readonly List<DataWriter> _dataWriters = new List<DataWriter>();
        private string _localSaveDataTimeLayout;
        private ConcurrentQueue<LabDataBase> _dataQueue;
        private Thread _writeThread;
        private Dictionary<Type, LabDataWriter> _dataWriterDic;
        public Action<LabDataBase> GetDataAction { get; set; }




        /// <summary>
        /// 数据采集,传入数据,频率,是否循环采集
        /// </summary>
        /// <param name="data"></param> 
        /// <param name="loop"></param>
        /// <param name="frequency"></param>
        [Obsolete("功能弃用，新的使用方法请查看Demo")]
        public void DataCollect(LabDataBase data, bool loop = true, int frequency = 200)
        {

        }

        /// <summary>
        /// 传入UserID初始化LabDataCollect
        /// </summary>
        /// <param name="userId"></param>
        public void LabDataCollectInit(Func<string> userId)
        {

            if (_isClientInit)
            {
                return;
            }

            #region 初始化本地存储
            _localSaveDataTimeLayout = LabTools.GetConfig<LabDataConfig>().LocalSaveDataTimeLayout;
            _userId = userId;
            _saveDataPath = Application.dataPath + "/Output";
            LabTools.CreatSaveDataFolder(_saveDataPath);
            var userStr = _userId.Invoke().PadLeft(2, '0');
            _saveDataPath = string.Join("_", _saveDataPath + "/" + DateTime.Now.ToString(_localSaveDataTimeLayout), userStr);
            _saveDataPath = LabTools.CreatSaveDataFolder(_saveDataPath);
            #endregion

            #region 初始化上传服务

            var options = new DataSyncClientOptions()
            {
                EndpointAddress = "http://localhost:4000/api/data",
                ProjectId = LabTools.GetConfig<LabDataConfig>().ProjectId,
                LogFilePath = labDataSavePath + "/ log.txt"
            };

            //Docker
            options.EndpointAddress = "http://localhost/api/data";

            //server
            _sendToServer = LabTools.GetConfig<LabDataConfig>().SendToServer;
            options.EndpointAddress = LabTools.GetConfig<LabDataConfig>().ServerPath;


            if (!Directory.Exists("TestStore"))
            {
                Directory.CreateDirectory("TestStore");
            }
            _applicationLifecycle = new SimpleApplicationLifecycle();


            _client = new DataSyncClient(new UnityApplicationFolderProvider(labDataSavePath + "/TestStore"),
                _applicationLifecycle, options, _userId);

            _client.Init();

            _isClientInit = true;

            StartUpload();

            #endregion

            Application.wantsToQuit += () => !IsClientRunning;
            _dataWriterDic = new Dictionary<Type, LabDataWriter>();
            _dataQueue = new ConcurrentQueue<LabDataBase>();
            _writeThread = new Thread(Queue2Send);
            _writeThread.Start();
        }


        async void ILabDataManager.LabDataDispose()
        {
            await Task.Run(() =>
            {
                while(_dataQueue.Count > 0)
                {
                    Debug.Log(( $"Remain {0} Data to be stored", _dataQueue.Count));
                    Thread.Sleep(100);                   
                }
            });
            foreach (var item in _dataWriterDic)
            {
                item.Value.WriterDispose();
            }
            GetDataAction = null;
            Debug.LogError("LabDataDispose");
            StopUpload();
            _isClientInit = false;
            // _dataWriters?.ForEach(p => p.Dispose());
        }

        /// <summary>
        /// 传输数据
        /// </summary>
        public void SendData(LabDataBase data)
        {
            _dataQueue.Enqueue(data);
            GetDataAction?.Invoke(data);
        }


        private void Queue2Send()
        {
            while (IsClientRunning)
            {
                var dataList = new List<LabDataBase>();
                while (_dataQueue.TryDequeue(out var resultData))
                {
                    dataList.Add(resultData);
                }
                foreach (var d in dataList)
                {
                    DoOnce(d);
                }
            }
        }
        private void DoOnce(LabDataBase data)
        {
            if (!_isClientInit)
            {
                Debug.LogError("LabData未初始化");
                return;
            }

            DataWriterFunc(data);

            if (_sendToServer)
            {
                Scope.Send(data);
            }

        }

        private void DataWriterFunc(LabDataBase data)
        {
            var datatype = data.GetType();
            if (!_dataWriterDic.ContainsKey(datatype))
            {
                string dataPath = string.Join("_", _saveDataPath + "/", _userId.Invoke().PadLeft(2, '0'), data.GetType().Name + ".json");
                LabTools.CreatData(dataPath);
                _dataWriterDic.Add(datatype, new LabDataWriter(dataPath));
            }

            _dataWriterDic[datatype].WriteData(data);
        }

        private void StartUpload()
        {
            if (IsClientRunning)
            {
                return;
            }
            Debug.Log("开始");
            _applicationLifecycle.OnStarted(EventArgs.Empty);
            Scope = _client.CreateNewScope();
            Scope.StartScope();
            IsClientRunning = true;
        }

        private void StopUpload()
        {
            if (!IsClientRunning)
            {
                return;
            }
            Debug.Log("停止");
            Scope.StopScope();
            Scope.Dispose();

            _applicationLifecycle.OnStopping(ApplicationStoppingEventArgs.Empty);
            IsClientRunning = false;
        }

    }

    public class LabDataWriter
    {
        private readonly FileStream _fs;
        private readonly StreamWriter _sw;
        public LabDataWriter(string path)
        {
            _fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            _sw = new StreamWriter(_fs);

        }
        public void WriteData(LabDataBase data)
        {
            _sw.WriteLine(data.ToJson());
        }

        public void WriterDispose()
        {
            _sw.Flush();
            _fs.Close();
        }
    }
}



