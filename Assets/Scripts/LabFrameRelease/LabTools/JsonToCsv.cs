using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonToCsv
{
    public class JsonToCsvConverter
    {
        private  List<String> _keys = new List<String>();

        public String Convert(String jsonStr)
        {
            var obj = JObject.Parse(jsonStr);
            return ToFlat(obj, "root");
        }

        public String ConvertTitle(String jsonStr)
        {
            var obj = JObject.Parse(jsonStr);
            return ToTitle(obj, "Base");
        }

        private String ToTitle(JObject obj, String parent)
        {
            String result = null;

            foreach (var item in obj)
            {
                if (item.Value.Type == JTokenType.Object)
                {
                    var child = (JObject)item.Value;
                    var tmp = ToTitle(child, item.Key);
                    result += tmp;
                }
                else if (item.Value.Type == JTokenType.Array)
                {
                    var jarray = (JArray)item.Value;
                    if (jarray.Count == 0 && !_keys.Contains(item.Key))
                    {
                        result += $"{item.Key},";
                        _keys.Add(item.Key);
                    }
                    else
                    {
                        foreach (var jitem in jarray)
                        {
                            if (jitem.HasValues)
                            {
                                var jchild = (JObject)jitem;
                                String tmp = ToTitle(jchild, item.Key);
                                result += tmp;
                            }
                            else if (!_keys.Contains(item.Key))
                            {
                                result += String.Format("{0},", item.Key);
                                _keys.Add(item.Key);
                            }
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(parent) && !_keys.Contains(item.Key))
                    {
                        result += String.Format("{0},", item.Key);
                        _keys.Add(item.Key);
                    }
                    else if (!_keys.Contains(parent + "_" + item.Key))
                    {
                        result += String.Format("{0}_{1},", parent, item.Key);
                        _keys.Add(parent + "_" + item.Key);
                    }
                }
            }
            return result;
        }

        private String ToFlat(JObject obj, String parent)
        {
            String result = null;

            foreach (var item in obj)
            {
                if (item.Value.Type == JTokenType.Object)
                {
                    var child = (JObject)item.Value;
                    var tmp = ToFlat(child, item.Key);
                    result += tmp;
                }
                else if (item.Value.Type == JTokenType.Array)
                {
                    var jarray = (JArray)item.Value;
                    if (jarray.Count == 0 && !_keys.Contains(item.Key))
                    {
                        result += $"{new JArray()},";
                        _keys.Add(item.Key);
                    }
                    else
                    {
                        foreach (var jitem in jarray)
                        {
                            if (jitem.HasValues)
                            {
                                var jchild = (JObject)jitem;
                                String tmp = ToFlat(jchild, item.Key);
                                result += tmp;
                            }
                            else if (!_keys.Contains(item.Key))
                            {
                                result += String.Format("{0},",new JArray() { jitem });
                                _keys.Add(item.Key);
                            }
                        }
                    }
                }
                else
                {
                    var value = item.Value ?? " ";

                    if (String.IsNullOrEmpty(parent) && !_keys.Contains(item.Key))
                    {
                        result += String.Format("{0},", value);
                        _keys.Add(item.Key);
                    }
                    else if (!_keys.Contains(parent + "_" + item.Key))
                    {
                        result += String.Format("{0},", value);
                        _keys.Add(parent + "_" + item.Key);
                    }
                }
            }
            return result;
        }
    }
}