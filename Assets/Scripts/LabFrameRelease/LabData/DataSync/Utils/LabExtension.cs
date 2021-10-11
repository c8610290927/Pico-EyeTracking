using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
//using I2.Loc;
using JsonToCsv;

namespace LabData
{
    public static class LabExtension 
    {
        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
                
            });           
        }
        /// <summary>
        /// 获取这个Key的翻译内容
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        ///
        /// 
        /*
        public static string GetTranslation(this string o)
        {
            //var translate = LocalizationManager.GetTranslation(o);
            return string.IsNullOrEmpty(translate) ? o : translate;
        }
        */

        public static string ToCsv(this object o)
        {
            //todo
            var converter = new JsonToCsvConverter();           
            return converter.Convert(o.ToJson());
        }

        public static string ToTitle(this object o)
        {
            //todo
            var converter = new JsonToCsvConverter();
            return converter.ConvertTitle(o.ToJson());
        }

        public static Mindfrog.Pos ToPos(this Vector3 vector3)
        {
            return new Mindfrog.Pos()
            {
                X = vector3.x,
                Y = vector3.y,
                Z = vector3.z
            };
        }

        public static Mindfrog.Pos2 ToPos(this Vector2 vector2)
        {
            return new Mindfrog.Pos2()
            {
                X = vector2.x,
                Y = vector2.y,
            };
        }
        public static Mindfrog.Qua ToQuaternionPos(this Quaternion quaternion)
        {
            return new Mindfrog.Qua()
            {
                X = quaternion.x,
                Y = quaternion.y,
                Z = quaternion.z,
                W = quaternion.w
            };
        }

      
    }

}
