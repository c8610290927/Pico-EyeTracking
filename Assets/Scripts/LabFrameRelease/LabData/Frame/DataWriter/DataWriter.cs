using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEngine;


namespace LabData
{

    public class DataWriter : IDisposable
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// 原始数据
        /// </summary>
        public Func<object> StrData { get; private set; }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public int Frequency { get; private set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public SaveType SaveType { get; private set; }


        private readonly Timer _timer = new Timer();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">写入数据路径</param>
        /// <param name="data">写入数据</param>
        /// <param name="type">写入数据格式</param>
        /// <param name="frequency">刷新率</param>
        public DataWriter(string path, Func<object> data, SaveType type, int frequency = 200)
        {
            Path = path;
            StrData = data;
            SaveType = type;
            Frequency = frequency;
        }
        /// <summary>
        /// 计时器功能
        /// </summary>
        public void WriteDataAsyncFrequency()
        {
            _timer.Interval = Frequency;
            _timer.Elapsed += timer_Tick;
            _timer.Start();
        }

 
        /// <summary>
        /// 写入一次
        /// </summary>
        public void WriteOnce()
        {
            using (FileStream fs = new FileStream(Path, FileMode.Append, FileAccess.Write))
            {
                fs.Lock(0, fs.Length);
                StreamWriter sw = new StreamWriter(fs);

                switch (SaveType)
                {
                    case SaveType.Json:
                        sw.WriteLine(StrData().ToJson());
                        break;
                    case SaveType.Csv:
                        sw.WriteLine(StrData().ToCsv());
                        break;
                    default:
                        break;
                }

                fs.Unlock(0, fs.Length);
                //清楚缓存
                sw.Flush();
            }
        }


        

        /// <summary>
        /// 写入表格文件标题
        /// </summary>
        public void WriteCsvTitle()
        {
            using (FileStream fs = new FileStream(Path, FileMode.Append, FileAccess.Write))
            {
                fs.Lock(0, fs.Length);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(StrData().ToTitle());
                fs.Unlock(0, fs.Length);
                //清楚缓存
                sw.Flush();
            }
        }

        /// <summary>
        /// 频繁写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, ElapsedEventArgs e)
        {
            WriteOnce();
        }

        public void Dispose()
        {
            _timer.Stop();
        }
    }
}


