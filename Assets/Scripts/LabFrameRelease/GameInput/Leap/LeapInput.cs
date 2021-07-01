using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class LeapInput : ILeapInterface
{
    /// <summary>
    /// 跷跷板专用，获取四元数
    /// </summary>
    /// <returns></returns>
    public Quaternion GetQuaternion()
    {
        return getHandInput();
    }

    /// <summary>
    /// 获取手是否被看到
    /// </summary>
    /// <returns></returns>
    public bool IsFindHand()
    {
        Update();
        if (_hand != null)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 获取手的具体数据类
    /// </summary>
    /// <returns></returns>
    public Hand GetFrame()
    {
        if (IsFindHand())
        {
            return _hand;
        }
        return null;
    }
    
    /// <summary>
    /// 设置位置生成对指所使用的手模型
    /// </summary>
    /// <param name="pos"></param>
    public void SetHandInWord(Vector3 pos)
    {
        //TODO
        //生成Leap 双手的Prefabs，用来实现对指
    }

    /// <summary>
    /// 手的Object 物体
    /// </summary>
    public GameObject HandObject { get; set; } = null;

    public float ROTATE_MOD = 3.14F;

    private Leap.Controller _controller;

    private Leap.Frame _frame;

    private Leap.Hand _hand;

    public LeapInput()
    {
        _controller = new Leap.Controller();
    }

    private Leap.Frame Frame
    {
        get
        {
            return _frame;
        }
    }

    private Leap.Hand Hand
    {
        get
        {
            return _hand;
        }
    }

    private void Update()
    {
        if (_controller != null)
        {
            _frame = _controller.Frame();
            if (_frame != null)
            {
                Debug.Log("frame！=null");
                if (_frame.Hands.Count > 0)
                {
                    _hand = _frame.Hands[0];
                }
                else
                    _hand = null;

            }
        }
    }

    public Quaternion getHandInput()
    {
        Quaternion leapData = getLeapData();
        return leapData;
    }

    private Quaternion getLeapData()
    {
        Update();
        Quaternion leapData = new Quaternion();
        if (_hand != null)
        {
            leapData = new Quaternion(_hand.Rotation.x, _hand.Rotation.y, _hand.Rotation.z, _hand.Rotation.w); //-ROTATE_MOD * PalmNormal.x;

        }
        else
        {
            leapData = new Quaternion();
        }
        return leapData;
    }

   
}
