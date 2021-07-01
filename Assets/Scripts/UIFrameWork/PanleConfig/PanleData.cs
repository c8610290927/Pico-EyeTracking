
using System.Collections.Generic;
using System;

public class PanleData
{
    public List<Data> Panle;

    public class Data
    {
        public string UIFormName;
        public string UIObjectName;
    }
}

public class UIFormConfig
{
    public Dictionary<string, List<PanleData.Data>> PanleKeys = new Dictionary<string, List<PanleData.Data>>();
   
    public UIFormConfig()
    {
        PanleData.Data data = new PanleData.Data();
        data.UIFormName = "Button";
        data.UIObjectName = "StartButton";
        List<PanleData.Data> datas = new List<PanleData.Data>();
        datas.Add(data);
        PanleKeys.Add("Menu", datas);
    }
}

