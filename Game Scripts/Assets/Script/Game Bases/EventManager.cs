using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventMgr
{
    #region 游戏初始化事件
    //等同于GameCtrl的初始化
    public class InitEventPublisher
    {
        //事件
        private event Action StartEvents;
        private event Action AwakeEvents;

        /// <summary>
        /// 当游戏开始第一帧时触发
        /// </summary>
        public void OnStart()
        {
            if (StartEvents != null)
                StartEvents();
        }

        /// <summary>
        /// 当游戏实例化时触发
        /// </summary>
        public void OnAwake()
        {
            if (AwakeEvents != null)
                AwakeEvents();
        }

        /// <summary>
        /// 监听游戏Start事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener2StartEvent(Action func)
        {
            StartEvents += func;
        }

        /// <summary>
        /// 取消监听游戏Start事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListenerInStartEvent(Action func)
        {
            StartEvents -= func;
        }

        /// <summary>
        /// 监听游戏Awake事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener2AwakeEvent(Action func)
        {
            AwakeEvents += func;
        }

        /// <summary>
        /// 取消监听游戏Awake事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListenerInAwakeEvent(Action func)
        {
            AwakeEvents -= func;
        }
    }

    /// <summary>
    /// 游戏初始化事件
    /// </summary>
    public static readonly InitEventPublisher initEvent = new InitEventPublisher();
    #endregion

    #region 游戏每帧事件
    //发布器
    public class UpdateEventPublisher
    {
        //事件
        private event Action CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger()
        {
            if (CallEvents != null)
            {
                CallEvents();
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Action func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Action func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 示例事件
    /// </summary>
    public static UpdateEventPublisher UpdateEvent = new UpdateEventPublisher();

    #endregion

    #region 单位最大速度变化事件
    //发布器
    public class MaxVChangeEventPublisher
    {
        public delegate void Handler(MaxVChangeInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(MaxVChangeInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位最大速度变化事件信息
    /// </summary>
    public class MaxVChangeInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousMaxV { get; private set; }
        public float CurrentMaxV { get; private set; }
        /// <summary>
        /// 构造 单位最大速度变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousMaxV">变化前速度</param>
        /// <param name="currentMaxV">变化后速度</param>
        public MaxVChangeInfo(UnitInfo unit, float previousMaxV, float currentMaxV)
        {
            Unit = unit;
            PreviousMaxV = previousMaxV;
            CurrentMaxV = currentMaxV;
        }
    }

    /// <summary>
    /// 单位最大速度变化事件
    /// </summary>
    public static readonly MaxVChangeEventPublisher maxVChangeEvent = new MaxVChangeEventPublisher();

    #endregion

    #region 单位最大速度加成变化事件
    //发布器
    public class MaxVBonusChangeEventPublisher
    {
        //委托
        public delegate void Handler(MaxVBonusChangeInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(MaxVBonusChangeInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位最大速度加成变化事件信息
    /// </summary>
    public class MaxVBonusChangeInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousMaxVBonus { get; private set; }
        public float CurrentMaxVBonus { get; private set; }
        /// <summary>
        /// 构造 单位最大速度加成变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousMaxVBonus">变化前速度加成</param>
        /// <param name="currentMaxVBonus">变化后速度加成</param>
        public MaxVBonusChangeInfo(UnitInfo unit, float previousMaxVBonus, float currentMaxVBonus)
        {
            Unit = unit;
            PreviousMaxVBonus = previousMaxVBonus;
            CurrentMaxVBonus = currentMaxVBonus;
        }
    }

    /// <summary>
    /// 单位最大速度加成变化事件
    /// </summary>
    public static readonly MaxVBonusChangeEventPublisher maxVChangeBonusEvent = new MaxVBonusChangeEventPublisher();

    #endregion

    #region 单位最大角速度变化事件
    //发布器
    public class MaxTurningVChangeEventPublisher
    {
        public delegate void Handler(MaxTurningVChangeInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(MaxTurningVChangeInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位最大角速度变化事件信息
    /// </summary>
    public class MaxTurningVChangeInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousMaxTurningV { get; private set; }
        public float CurrentMaxTurningV { get; private set; }
        /// <summary>
        /// 构造 单位最大角速度变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousMaxTurningV">变化前角速度</param>
        /// <param name="currentMaxTurningV">变化后角速度</param>
        public MaxTurningVChangeInfo(UnitInfo unit, float previousMaxTurningV, float currentMaxTurningV)
        {
            Unit = unit;
            PreviousMaxTurningV = previousMaxTurningV;
            CurrentMaxTurningV = currentMaxTurningV;
        }
    }

    /// <summary>
    /// 单位最大角速度变化事件
    /// </summary>
    public static readonly MaxTurningVChangeEventPublisher maxTurningVChangeEvent = new MaxTurningVChangeEventPublisher();

    #endregion

    #region 单位最大角速度加成变化事件
    //发布器
    public class MaxTurningVBonusChangeEventPublisher
    {
        public delegate void Handler(MaxTurningVBonusChangeEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(MaxTurningVBonusChangeEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位最大角速度加成变化事件信息
    /// </summary>
    public class MaxTurningVBonusChangeEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousValue { get; private set; }
        public float CurrentValue { get; private set; }
        /// <summary>
        /// 构造单位最大角速度加成变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousValue">变化前最大角速度加成</param>
        /// <param name="currentValue">变化后最大角速度加成</param>
        public MaxTurningVBonusChangeEventInfo(UnitInfo unit, float previousValue, float currentValue)
        {
            Unit = unit;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    /// <summary>
    /// 单位最大角速度加成事件
    /// </summary>
    public static readonly MaxTurningVBonusChangeEventPublisher MaxTurningVBonusChangeEvent = new MaxTurningVBonusChangeEventPublisher();

    #endregion

    #region 单位最大护盾值变化事件
    //发布器
    public class MaxSPChangeEventPublisher
    {
        public delegate void Handler(MaxSPChangeEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(MaxSPChangeEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位最大护盾值变化事件信息
    /// </summary>
    public class MaxSPChangeEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousValue { get; private set; }
        public float CurrentValue { get; private set; }
        /// <summary>
        /// 构造单位最大护盾值变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousValue">变化前量</param>
        /// <param name="currentValue">变化后量</param>
        public MaxSPChangeEventInfo(UnitInfo unit, float previousValue, float currentValue)
        {
            Unit = unit;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    /// <summary>
    /// 单位最大护盾值变化事件
    /// </summary>
    public static readonly MaxSPChangeEventPublisher MaxSPChangeEvent = new MaxSPChangeEventPublisher();

    #endregion

    #region 单位护盾值变化事件
    //发布器
    public class SPChangeEventPublisher
    {
        public delegate void Handler(SPChangeEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(SPChangeEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位护盾值变化事件信息
    /// </summary>
    public class SPChangeEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public float PreviousValue { get; private set; }
        public float CurrentValue { get; private set; }
        /// <summary>
        /// 构造单位护盾值变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="previousValue">变化前量</param>
        /// <param name="currentValue">变化后量</param>
        public SPChangeEventInfo(UnitInfo unit, float previousValue, float currentValue)
        {
            Unit = unit;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    /// <summary>
    /// 单位最大护盾值变化事件
    /// </summary>
    public static readonly SPChangeEventPublisher SPChangeEvent = new SPChangeEventPublisher();

    #endregion

    #region 浮点型属性变化事件
    //发布器
    public class FloatProperityChangeEventPublisher
    {
        public delegate void Handler(FloatProperityChangeEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(FloatProperityChangeEventInfo info)
        {
            Debug.Log("Float Properity Change Event : " + info.Unit.UnitCtrl.unit + ", " + info.FloatProperity);
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 浮点型属性变化事件信息
    /// </summary>
    public class FloatProperityChangeEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        public FloatProperityWithBonus FloatProperity { get; private set; }
        public float PreviousValue { get; private set; }
        public float CurrentValue { get; private set; }
        /// <summary>
        /// 构造浮点型属性变化事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        /// <param name="floatProperity">浮点型属性</param>
        /// <param name="previousValue">变化前量</param>
        /// <param name="currentValue">变化后量</param>
        public FloatProperityChangeEventInfo(UnitInfo unit, FloatProperityWithBonus floatProperity, float previousValue, float currentValue)
        {
            Unit = unit;
            FloatProperity = floatProperity;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    /// <summary>
    /// 浮点型属性变化事件
    /// </summary>
    public static readonly FloatProperityChangeEventPublisher FloatPropertiyChangeEvent = new FloatProperityChangeEventPublisher();

    #endregion

    #region 单位出生事件
    //发布器
    public class UnitBirthEventPublisher
    {
        public delegate void Handler(UnitBirthEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(UnitBirthEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位出生事件信息
    /// </summary>
    public class UnitBirthEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        /// <summary>
        /// 构造单位事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        public UnitBirthEventInfo(UnitInfo unit)
        {
            Unit = unit;
        }
    }

    /// <summary>
    /// 单位出生事件
    /// </summary>
    public static readonly UnitBirthEventPublisher UnitBirthEvent = new UnitBirthEventPublisher();

    #endregion

    #region 单位清除事件
    //发布器
    public class UnitClearEventPublisher
    {
        public delegate void Handler(UnitClearEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(UnitClearEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }
    }

    /// <summary>
    /// 单位清除事件信息
    /// </summary>
    public class UnitClearEventInfo : EventArgs
    {
        public UnitInfo Unit { get; private set; }
        /// <summary>
        /// 构造单位清除事件的传入参数
        /// </summary>
        /// <param name="unit">单位全面信息</param>
        public UnitClearEventInfo(UnitInfo unit)
        {
            Unit = unit;
        }
    }

    /// <summary>
    /// 单位清除事件
    /// </summary>
    public static readonly UnitClearEventPublisher UnitClearEvent = new UnitClearEventPublisher();

    #endregion

    #region 施法事件 暂时没用 也没写完
    //发布器
    public class SpellEventPublisher
    {
        public delegate void Handler(SpellEventInfo info);
        //事件
        private event Handler CallEvents;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="info">事件信息</param>
        public void OnTrigger(SpellEventInfo info)
        {
            if (CallEvents != null)
            {
                CallEvents(info);
            }
        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void AddListener(Handler func)
        {
            CallEvents += func;
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="func">事件监听方法</param>
        public void RemoveListener(Handler func)
        {
            CallEvents -= func;
        }

    }
    /// <summary>
    /// 施法事件信息
    /// </summary>
    public class SpellEventInfo : EventArgs
    {

    }
    #endregion
}
//#region 示例事件
////发布器
//public class ExampleEventPublisher
//{
//    public delegate void Handler(ExampleEventInfo info);
//    //事件
//    private event Handler CallEvents;

//    /// <summary>
//    /// 触发事件
//    /// </summary>
//    /// <param name="info">事件信息</param>
//    public void OnTrigger(ExampleEventInfo info)
//    {
//        if (CallEvents != null)
//        {
//            CallEvents(info);
//        }
//    }

//    /// <summary>
//    /// 监听事件
//    /// </summary>
//    /// <param name="func">事件监听方法</param>
//    public void AddListener(Handler func)
//    {
//        CallEvents += func;
//    }

//    /// <summary>
//    /// 取消监听
//    /// </summary>
//    /// <param name="func">事件监听方法</param>
//    public void RemoveListener(Handler func)
//    {
//        CallEvents -= func;
//    }
//}

///// <summary>
///// 示例事件信息
///// </summary>
//public class ExampleEventInfo : EventArgs
//{
//    public UnitInfo Unit { get; private set; }
//    public int PreviousValue { get; private set; }
//    public int CurrentValue { get; private set; }
//    /// <summary>
//    /// 构造示例事件的传入参数
//    /// </summary>
//    /// <param name="unit">单位全面信息</param>
//    /// <param name="previousValue">变化前量</param>
//    /// <param name="currentValue">变化后量</param>
//    public ExampleEventInfo(UnitInfo unit, int previousValue, int currentValue)
//    {
//        Unit = unit;
//        PreviousValue = previousValue;
//        CurrentValue = currentValue;
//    }
//}

///// <summary>
///// 示例事件
///// </summary>
//public static ExampleEventPublisher ExampleEvent;

//#endregion