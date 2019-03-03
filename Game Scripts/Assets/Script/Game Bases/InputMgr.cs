using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMgr
{
    #region 参数
    private static bool hotKeyEnabled = true;
    /// <summary>
    /// 启用快捷键
    /// </summary>
    public static bool HotKeyEnabled
    {
        get
        {
            return hotKeyEnabled;
        }

        set
        {
            hotKeyEnabled = value;
        }
    }

    private static bool _mobileControlKeyEnable = true;
    /// <summary>
    /// 启用移动控制键
    /// </summary>
    public static bool MobileControlKeyEnable
    {
        get
        {
            return _mobileControlKeyEnable;
        }

        set
        {
            _mobileControlKeyEnable = value;
        }
    }
    #endregion

    #region 热键
    private static Dictionary<KeyCode, List<Action>> hotKeyDict = new Dictionary<KeyCode, List<Action>>();
    /// <summary>
    /// 对快捷键字典的操作
    /// </summary>
    private struct Instruction
    {
        public Action action;//操作对象
        public KeyCode key;//对应的快捷键
        public bool isRemove;//是移除操作, 还是加入操作
    }
    //操作集合, 防止因为在字典中方法中修改字典导致的死锁
    private static List<Instruction> instructions = new List<Instruction>();
    /// <summary>
    /// 将方法绑定到一个快捷键上。当松开快捷键时，方法会被调用。下一帧前生效。
    /// </summary>
    /// <param name="action">方法</param>
    /// <param name="key">快捷键</param>
    public static void BindHotKey(Action action, KeyCode key)
    {
        lock (instructions)
        {
            if (!hotKeyDict.ContainsKey(key))
            {
                hotKeyDict.Add(key, new List<Action>());
            }
            if (!hotKeyDict[key].Contains(action))
                instructions.Add(new Instruction() { action = action, key = key, isRemove = false });
        }
    }

    /// <summary>
    /// 将方法解绑。下一帧前生效。
    /// </summary>
    /// <param name="action">方法</param>
    /// <param name="key">快捷键</param>
    public static void UnbindHotKey(Action action, KeyCode key)
    {
        lock (instructions)
        {
            if (hotKeyDict.ContainsKey(key) && hotKeyDict[key].Contains(action))
                instructions.Add(new Instruction() { action = action, key = key, isRemove = true });
        }
    }

    /// <summary>
    /// 检查是否触发快捷键
    /// </summary>
    public static void CheckHotKey()
    {
        //同步。通过操作集修改快捷键字典
        lock (instructions)
            lock (hotKeyDict)
                instructions.RemoveAll(delegate (Instruction inst)
                {
                    if (inst.isRemove)
                        hotKeyDict[inst.key].Remove(inst.action);
                    else hotKeyDict[inst.key].Add(inst.action);
                    return true;
                });
        //根据按键状态调用方法
        if (hotKeyEnabled)
        {
            lock (hotKeyDict)
                foreach (KeyCode key in hotKeyDict.Keys)
                {
                    if (Input.GetKeyUp(key))
                        foreach (Action action in hotKeyDict[key])
                        {
                            action();
                        }
                }
        }
    }
    #endregion

    #region Mobile Control Key
    public static KeyCode ForwardKey = KeyCode.W;
    public static KeyCode BackwardKey = KeyCode.S;
    public static KeyCode LeftKey = KeyCode.A;
    public static KeyCode RightKey = KeyCode.D;
    public static KeyCode AccelerationKey = KeyCode.LeftShift;
    /// <summary>
    /// 当前进键被玩家按下时返回真
    /// </summary>
    /// <returns></returns>
    public static bool GetForwardKey()
    {
        return MobileControlKeyEnable && Input.GetKey(ForwardKey);
    }
    /// <summary>
    /// 当后退键被玩家按下时返回真
    /// </summary>
    /// <returns></returns>
    public static bool GetBackwardKey()
    {
        return MobileControlKeyEnable && Input.GetKey(BackwardKey);
    }
    /// <summary>
    /// 当向左键被玩家按下时返回真
    /// </summary>
    /// <returns></returns>
    public static bool GetLeftkey()
    {
        return MobileControlKeyEnable && Input.GetKey(LeftKey);
    }
    /// <summary>
    /// 当向右键被玩家按下时返回真
    /// </summary>
    /// <returns></returns>
    public static bool GetRightKey()
    {
        return MobileControlKeyEnable && Input.GetKey(RightKey);
    }

    public static string VerticalAxis = "Vertical";
    public static string HorizontalAxis = "Horizontal";
    /// <summary>
    /// Gets the vertical axis.
    /// </summary>
    /// <returns>The vertical axis.</returns>
    public static float GetVerticalAxis()
    {
        return MobileControlKeyEnable ? Input.GetAxis(VerticalAxis) : 0f;
    }
    /// <summary>
    /// Gets the horizontal axis.
    /// </summary>
    /// <returns>The horizontal axis.</returns>
    public static float GetHorizontalAxis()
    {
        return MobileControlKeyEnable ? Input.GetAxis(HorizontalAxis) : 0f;
    }

    #endregion
    
}
