using DataSync;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabData
{
    public interface ILabDataManager
    {
         void LabDataCollectInit(Func<string> userId);
         void SendData(LabDataBase data);
         Action<LabDataBase> GetDataAction { get; set; }
         void LabDataDispose();
         bool IsClientRunning { get;  }
    }
}
