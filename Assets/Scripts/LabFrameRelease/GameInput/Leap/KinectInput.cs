using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KinectBoneType
{
    ClavicleLeft,
    ShoulderLeft,
    ElbowLeft,
    HandLeft,
    FingersLeft,
    ThumbLeft,
    ClavicleRight,
    ShoulderRight,
    ElbowRight,
    HandRight,
    FingersRight,
    ThumbRight,

}

public class KinectInput : IKinectTrack
{

    /// <summary>
    /// KinectManager
    /// </summary>
    private KinectManager _kinectManager;

    /// <summary>
    /// 骨骼信息脚本
    /// </summary>
    private AvatarControllerClassic _avatarControllerClassic;

    /// <summary>
    /// 获取骨骼信息
    /// </summary>
    /// <param name="kinectBoneType"></param>
    /// <returns></returns>
    public Transform GetKinectBone(KinectBoneType kinectBoneType)
    {
       return (Transform)_avatarControllerClassic.GetType().GetField(kinectBoneType.ToString()).GetValue(_avatarControllerClassic);
    }

    /// <summary>
    /// 是否检测到骨骼
    /// </summary>
    /// <returns></returns>
    public bool IsUserDetected()
    {
        return _kinectManager.IsUserDetected();
    }

    /// <summary>
    /// 生成KinectController Prefabs
    /// </summary>
    /// <param name="vector3"></param>
    public void SetKinectController(Vector3 vector3)
    {
        //生成KinectManager
        //并将Kinect 赋值
        GameObject Controller = null;
        _kinectManager = Controller.GetComponentInChildren<KinectManager>();
        _avatarControllerClassic= Controller.GetComponentInChildren<AvatarControllerClassic>();
    }

    

}
