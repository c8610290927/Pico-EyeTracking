using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKinectTrack 
{
    /// <summary>
    /// 生成Kinect控制脚本组件
    /// </summary>
    /// <param name="vector3"></param>
    void SetKinectController(Vector3 vector3);

    /// <summary>
    /// 判断是否检测到玩家
    /// </summary>
    bool IsUserDetected();

    /// <summary>
    /// 获取相对应的骨骼点信息
    /// </summary>
    /// <param name="kinectBoneType"></param>
    /// <returns></returns>
    Transform GetKinectBone(KinectBoneType kinectBoneType);

}
