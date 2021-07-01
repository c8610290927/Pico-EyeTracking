using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILeapInterface
{
    /// <summary>
    /// 获取手掌旋转的四元数
    /// </summary>
    /// <returns></returns>
    Quaternion GetQuaternion();

    /// <summary>
    /// 检测是否看到手
    /// </summary>
    /// <returns></returns>
    bool IsFindHand();

    /// <summary>
    /// 获取手
    /// </summary>
    /// <returns></returns>
    Leap.Hand GetFrame();

    /// <summary>
    /// 设置手的位置信息，并生成出来
    /// </summary>
    void SetHandInWord(Vector3 Pos);

    
}
