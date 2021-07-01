using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DataSync;
using I2.Loc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using UnityEngine;

namespace LabData
{
    public class LabTools
    {

        public static T GetData<T>(LabDataBase data) where T : LabDataBase
        {
            return data is T @base ? @base : null;
        }

        public static string DataPath => Application.dataPath;
        /// <summary>
        /// 创建存储数据的文件夹
        /// </summary>
        /// <param name="floderName"></param>
        /// <param name="isNew"></param>
        public static string CreatSaveDataFolder(string floderName, bool isNew = false)
        {
            if (Directory.Exists(floderName))
            {
                if (isNew)
                {
                    var tempPath = floderName + "_" + DateTime.Now.Millisecond.ToString();
                    Directory.CreateDirectory(tempPath);
                    return tempPath;
                }

                Debug.Log("Folder Has Existed!");
                return floderName;
            }
            else
            {
                Directory.CreateDirectory(floderName);
                Debug.Log("Success Create: " + floderName);
                return floderName;
            }
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        public static void CreatData(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();

                Debug.Log("Success Create: " + path);
            }
        }
        /// <summary>
        /// 获取Enum的Description内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T obj)
        {
            var type = obj.GetType();
            FieldInfo field = type.GetField(Enum.GetName(type, obj));
            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
            {
                return string.Empty;
            }

            return descAttr.Description;
        }

        /// <summary>
        /// 根据Config类型反序列化，如果是需要覆盖旧的config，传入true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNew"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T GetConfig<T>(bool isNew = false, string filePath = "/GameData") where T : class, new()
        {
            var path = DataPath + filePath + "/" + "ConfigData";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = path + "/" + typeof(T).Name + ".json";


            if (isNew && File.Exists(path))
            {
                File.Delete(path);
            }
            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(new T());
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }

            StreamReader sr = new StreamReader(path);
            var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            sr.Close();
            return data;
        }

        /// <summary>
        /// 创建对应数据的文件夹
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateDataFolder<T>(string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 写数据，数据类型必须继承LabDataBase，dataName为需要写的数据名字，isOverWrite选择是否要覆盖已有文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <param name="isOverWrite"></param>
        /// <returns></returns>
        public static void WriteData<T>(T t, string dataName, bool isOverWrite = false, string filePath = "/GameData") where T : LabDataBase, new()
        {
            var path = DataPath + filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else if (File.Exists(path) && isOverWrite)
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                fs.Close();
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else
            {
                Debug.LogError("需要重写数据，请在参数中设置isOverWrite=true");
            }
        }

        /// <summary>
        /// 通过类型T和名字获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static T GetData<T>(string dataName, string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                sr.Close();
                return data;

            }
            else
            {
                Debug.LogError("数据文件不存在！");
                return null;
            }
        }

        /// <summary>
        /// 通过类型T和名字获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static T GetDataByString<T>(string file) where T : class
        {
            JSchema schema = new JSchemaGenerator().Generate(typeof(T));
            Debug.Log(schema);
            JToken token = JToken.Parse(file);

            if (token.IsValid(schema))
            {
                return JsonConvert.DeserializeObject<T>(file);
            }
            else
            {

                Debug.LogError("字符串不匹配");
                return null;
            }

        }
        /// <summary>
        /// 删除数据文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataName"></param>
        public static void DeleteData<T>(string dataName, string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (!File.Exists(path))
            {
                Debug.LogError("数据文件不存在！");
            }
            else
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 获取对应数据的文件夹内的所有文件的名字List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isGetName"></param>
        /// <returns></returns>
        public static List<string> GetDataName<T>(bool isGetName = false, string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name;
            if (Directory.Exists(path))
            {
                var root = new DirectoryInfo(path);
                var files = root.GetFiles();
                if (files.Length <= 0)
                {
                    Debug.LogError("没有可用数据文件！");
                    return null;
                }
                List<string> tempList = new List<string>();
                foreach (var fileInfo in files)
                {
                    if (".json".ToLower().IndexOf(fileInfo.Extension.ToLower(), StringComparison.Ordinal) >= 0)
                    {
                        tempList.Add(fileInfo.Name.Split('.').First());
                    }
                }

                return tempList;
            }

            Debug.LogError("数据文件夹不存在！");
            return null;
        }

        /// <summary>
        ///通过Key获取多语言对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCurrentCultureValue(params string[] key)
        {
            return string.Join("", key.Select(p =>
            {
                var translate = LocalizationManager.GetTranslation(p);
                if (p != null && string.IsNullOrEmpty(translate))
                {
                    return p;
                }
                return translate;
            }));
        }

        public static Type GetScriptType(string name)
        {
            return Type.GetType("UIFrameWork." + name);
        }
    }
}

