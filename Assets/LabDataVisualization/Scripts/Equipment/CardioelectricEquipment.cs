using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabVisualization;
using System;
using LabDevice;
using DeviceLibrary;
using static DeviceLibrary.Device;

/// <summary>
/// 电信号设备
/// </summary>
public class CardioelectricEquipment : MonoBehaviour, LabVisualization.Cardiogram.CardiogramPos,LabVisualization.IEquipment
{
    //心电
    BlueToothController blueToothController = new BlueToothController();
    //皮肤电
    Device device = new Device();
    /// <summary>
    /// 是否开启心电设备
    /// </summary>
    private bool IsOpenSkine = false;
    /// <summary>
    /// 用来通信的三维坐标
    /// </summary>
    private Vector3 SingleVector3=new Vector3();
    /// <summary>
    /// 心lv数据
    /// </summary>
    public int Heartrate_data { get; set; }
    /// <summary>
    /// 心电数据
    /// </summary>
    public int Cardioelectric_data { get; set; }
    /// <summary>
    /// 皮肤点数据
    /// </summary>
    public int Skinelectricity_data { get; set; }
    /// <summary>
    /// 设备初始化
    /// </summary>
    public void EquipmentInit()
    {
        blueToothController.Init();
        //if (device.OpenPort("COM5"))
        //{
        //    device.getData();
        //    device.GetSkinHandler = new GetSkinDataDelegate(GetSkinData);
        //    IsOpenSkine = true;
        //}
    }
    /// <summary>
    /// 设备运作
    /// </summary>
    public void EquipmentStart()
    {
       if (!blueToothController.StartConnect()) return;
        blueToothController.GetData();
        blueToothController.GetRawHandler = new BlueToothController.GetRawDataDelegate(GetHeardData);
        blueToothController.GetHeartHandler = new BlueToothController.GetHeartDataDelegate(GetHeartrate);
    }
    /// <summary>
    /// 获取心电信号
    /// </summary>
    /// <param name="data"></param>
    private void GetHeardData(int data)
    {
        Cardioelectric_data = data;
    }
    /// <summary>
    /// 获取心率
    /// </summary>
    /// <param name="data"></param>
    private void GetHeartrate(int[] data)
    {
        Heartrate_data = data[1];
    }
    /// <summary>
    /// 获取皮肤电
    /// </summary>
    /// <param name="data"></param>
    private void GetSkinData(int[] data)
    {
        for (int i = 0; i < 15; i++)
        {
            Debug.Log(data[i] + "皮肤电");
            data[i] = data[i] < 5000 ? data[i] : 0;

            Skinelectricity_data = data[i];
        }
    }
    /// <summary>
    /// 传出通信数据
    /// </summary>
    /// <returns></returns>
    public Func<Vector3> GetCardiogramPos()
    {
        return () => new Vector3(Skinelectricity_data,Heartrate_data, Cardioelectric_data);
    }
    /// <summary>
    /// 设备关闭
    /// </summary>
    public void EquipmentStop()
    {
        blueToothController.StopConnect();

        if (IsOpenSkine)
        {
            device?.StopData();
            IsOpenSkine = false;
        }
    }
}
