using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class Unit
{
    /// <summary>
    /// 单位ID，-1表示未设置
    /// </summary>
    public int ID = -1;
    public UnitInfo unitInfo;

    /// <summary>
    /// 单位的名字
    /// </summary>
    public UnitName name;


    /// <summary>
    /// 是否存活。生命值归零触发死亡事件，事件结束后isAlive为false
    /// </summary>
    public bool isAlive = true;

    #region 单位属性
    public UnitData data;

    #region 最大速度 & 加速度
    //当前最大速度
    [SerializeField]
    private float maxV_current = -100;
    public float OriginalMaxV = 2;
    public float MaxV
    {
        get
        {
            return maxV_current;
        }
        set
        {
            //设置参数
            float pre = maxV_current;
            maxV_current = Mathf.Clamp(value, 0, GameDB.MAX_SPEED);
            Acceleration = maxV_current * GameDB.DAMPED_CONST;
            //构造事件参数
            EventMgr.MaxVChangeInfo eventInfo = new EventMgr.MaxVChangeInfo(unitInfo, pre, maxV_current);
            //触发事件
            EventMgr.maxVChangeEvent.OnTrigger(eventInfo);
        }
    }

    private float maxV_bonus = 0;
    public float MaxV_bonus
    {
        get
        {
            return maxV_bonus;
        }
        set
        {
            if (maxV_bonus != value)
            {
                //构造事件参数
                EventMgr.MaxVBonusChangeInfo eventInfo = new EventMgr.MaxVBonusChangeInfo(unitInfo, maxV_bonus, value);
                //设置变量
                maxV_bonus = value;
                MaxV = OriginalMaxV * Mathf.Clamp(1 + maxV_bonus, GameDB.MIN_BONUS_FOR_MAX_V, GameDB.MAX_BONUS_FOR_MAX_V);
                //触发事件
                EventMgr.maxVChangeBonusEvent.OnTrigger(eventInfo);
            }
        }
    }

    public float Acceleration { get; private set; }
    #endregion

    #region 最大角速度 & 角加速度
    //当前最大角速度
    private float maxTurningV_current = -100;
    public float OriginalMaxTurningV = 20;
    public float MaxTurningV
    {
        get
        {
            return maxTurningV_current;
        }
        set
        {
            //设置参数
            float pre = maxTurningV_current;
            maxTurningV_current = Mathf.Clamp(value, 0, GameDB.MAX_TURNING_V);
            AngularAcceleration = maxTurningV_current * GameDB.ANGULAR_DAMPED_CONST;
            //构造事件参数
            EventMgr.MaxTurningVChangeInfo eventInfo = new EventMgr.MaxTurningVChangeInfo(unitInfo, pre, maxTurningV_current);
            //触发事件
            EventMgr.maxTurningVChangeEvent.OnTrigger(eventInfo);
        }
    }

    private float maxTurningV_bonus = 0;
    public float MaxTurningV_bonus
    {
        get
        {
            return maxTurningV_bonus;
        }
        set
        {
            if (maxTurningV_bonus != value)
            {
                //构造事件参数
                EventMgr.MaxTurningVBonusChangeEventInfo eventInfo = new EventMgr.MaxTurningVBonusChangeEventInfo(unitInfo, maxTurningV_bonus, value);
                //设置变量
                maxTurningV_bonus = value;
                MaxTurningV = OriginalMaxTurningV * Mathf.Clamp(1 + maxTurningV_bonus, GameDB.MIN_BONUS_FOR_MAX_TURNING_V, GameDB.MAX_BONUS_FOR_MAX_TURNING_V);
                //触发事件
                EventMgr.MaxTurningVBonusChangeEvent.OnTrigger(eventInfo);
            }
        }
    }

    public float AngularAcceleration { get; private set; }
    #endregion

    #region 护盾值 & 护甲
    [SerializeField]
    private float sheildPoint_current = 10;
    public float SheildPoint
    {
        get
        {
            return sheildPoint_current;
        }
        set
        {
            float pre = Mathf.Clamp(value, 0, MaxShieldPoint);
            sheildPoint_current = pre;
            //设置参数
            EventMgr.SPChangeEventInfo info = new EventMgr.SPChangeEventInfo(unitInfo, pre, sheildPoint_current);
            //触发事件
            EventMgr.SPChangeEvent.OnTrigger(info);
        }
    }

    private float maxShieldPoint_current = 10;
    public float OriginalMaxSP = 10;
    public float MaxShieldPoint
    {
        get
        {
            return maxShieldPoint_current;
        }
        set
        {
            //设置参数
            float pre = maxShieldPoint_current;
            maxShieldPoint_current = Mathf.Clamp(value, 1f, GameDB.MAX_SHEILD_POINT);
            SheildPoint *= maxShieldPoint_current / pre;
            //设置参数
            EventMgr.MaxSPChangeEventInfo info = new EventMgr.MaxSPChangeEventInfo(unitInfo, pre, maxShieldPoint_current);
            //触发事件
            EventMgr.MaxSPChangeEvent.OnTrigger(info);
        }
    }
    private float maxSP_bonus = 0;
    //最大护盾加成，暂时还没有事件
    public float MaxSP_bonus
    {
        get
        {
            return maxSP_bonus;
        }
        set
        {
            if (maxSP_bonus != value)
            {
                //设置变量
                maxSP_bonus = value;
                MaxShieldPoint = (int)(OriginalMaxSP * Mathf.Clamp(1 + maxSP_bonus, GameDB.MIN_BONUS_FOR_MAX_SP, GameDB.MAX_BONUS_FOR_MAX_SP));
            }
        }
    }

    public FloatProperityWithBonus SPRegenerationRate = new FloatProperityWithBonus()
    {
        OriginalValue = 1f,
    };
    /// <summary>
    /// 护甲类型
    /// </summary>
    public ArmorType ArmorType = ArmorType.invincible;
    #endregion

    #region 魔法值
    #region
    //[SerializeField]
    //private float manaPoint_current = 100f;
    //public float ManaPoint
    //{
    //    get
    //    {
    //        return manaPoint_current;
    //    }
    //    set
    //    {
    //        manaPoint_current = Mathf.Clamp(value, 0f, MaxManaPoint);
    //    }
    //}

    //private float maxManaPoint_current = 100f;
    //public float OriginalMaxMP = 100f;
    //public float MaxManaPoint
    //{
    //    get
    //    {
    //        return maxManaPoint_current;
    //    }
    //    set
    //    {
    //        //设置参数
    //        float pre = maxManaPoint_current;
    //        maxManaPoint_current = Mathf.Clamp(value, 1f, GameDB.MAX_MANA_POINT);
    //        ManaPoint *= maxManaPoint_current / pre;
    //    }
    //}

    ////法力回复速度
    //private float mpRegenerationRate_current = 10f;
    //public float OriginalMPRegenerationRate = 10f;
    //public float MPRegenerationRate
    //{
    //    get
    //    {
    //        return mpRegenerationRate_current;
    //    }
    //    set
    //    {
    //        mpRegenerationRate_current = value;
    //    }
    //}

    //private float mpRegenerationRate_bonus = 0f;
    //public float MPRegenerationRate_bonus
    //{
    //    get
    //    {
    //        return mpRegenerationRate_bonus;
    //    }
    //    set
    //    {
    //        if (mpRegenerationRate_bonus != value)
    //        {
    //            mpRegenerationRate_bonus = value;
    //            MPRegenerationRate = (1 + value) * OriginalMPRegenerationRate;
    //        }
    //    }
    //}
    #endregion

    public FloatProperityWithBonus ManaPoint = new FloatProperityWithBonus()
    {
        minValue = 0f,
    };
    public FloatProperityWithBonus MaxManaPoint = new FloatProperityWithBonus()
    {
        minValue = 1f,
        minBonus = -1f,
        isEventTrigger = true,
    };
    void ClampMP(float prev_value, ref float value)
    {
        value = Mathf.Clamp(value, 0, MaxManaPoint.Value);
    }
    void SetMP(float prev_value, ref float value)
    {
        ManaPoint.Value *= value / prev_value;
    }
    public FloatProperityWithBonus MPRegenerationRate = new FloatProperityWithBonus()
    {
        OriginalValue = 10f,
    };
    #endregion

    #endregion


    private bool isInit = false;
    /// <summary>
    /// Unit类初始化（该方法由GameCtrl.UnitBirth()方法或Gamef.UnitBirth()调用）
    /// </summary>
    public void Init(UnitInfo unitInfo)
    {
        if (isInit)
            return;
        isInit = true;
        this.unitInfo = unitInfo;
        //读取单位数据并赋初值
        data = Gamef.LoadUnitData(name);
        OriginalMaxV = data.MaxVelocity;
        OriginalMaxTurningV = data.MaxTurningVelocity;
        OriginalMaxSP = data.MaxSheildPoint;
        MaxManaPoint.OriginalValue = data.MaxManaPoint;
        MPRegenerationRate.OriginalValue = data.ManaPointRegenerationRate;
        SPRegenerationRate.OriginalValue = data.ShieldPointRegenerationRate;
        //初始化速度
        maxV_current = OriginalMaxV;
        Acceleration = maxV_current * GameDB.DAMPED_CONST;
        //初始化转向速度
        maxTurningV_current = OriginalMaxTurningV;
        AngularAcceleration = maxTurningV_current * GameDB.ANGULAR_DAMPED_CONST;
        //初始化护盾
        maxShieldPoint_current = OriginalMaxSP;
        sheildPoint_current = OriginalMaxSP;
        SPRegenerationRate.Init(this);
        //初始化魔法值
        ManaPoint.setValueHandler = ClampMP;
        MaxManaPoint.setValueHandler = SetMP;
        MaxManaPoint.Init(this);
        ManaPoint.Init(this);
        MPRegenerationRate.Init(this);
    }
}
[System.Serializable]
public struct UnitTags
{
    public bool player;
    public bool elf;
    public bool devil;
}
[System.Serializable]
public class FloatProperityWithBonus
{
    #region 参数
    [HideInInspector] public float minValue = -1e7f;
    [HideInInspector] public float maxValue = 1e7f;
    [HideInInspector] public float minBonus = -1e5f;
    [HideInInspector] public float maxBonus = 1e5f;
    [HideInInspector] public bool isEventTrigger = false;
    [HideInInspector] public UnitCtrl unit;
    #endregion
    public delegate void Handler(float prev_value, ref float value);
    //调用事件前，可进行一系列处理
    public Handler setBonusHandler;
    public Handler setValueHandler;
    [SerializeField]
    private float value_current = 10f;
    public float OriginalValue = 10f;
    public float Value
    {
        get
        {
            return value_current;
        }
        set
        {
            float pre = value_current;
            value_current = Mathf.Clamp(value, minValue, maxValue);
            //自定义函数
            if (setValueHandler != null)
                setValueHandler(pre, ref value_current);
            if (isEventTrigger)
            {
                //触发事件
                EventMgr.FloatPropertiyChangeEvent.OnTrigger(new EventMgr.FloatProperityChangeEventInfo(unit.unitInfo, this, pre, value_current));
            }
        }
    }

    private float bonus = 0f;
    public float Bonus
    {
        get
        {
            return bonus;
        }
        set
        {
            if (bonus != value)
            {
                float prev_bonus = bonus;
                bonus = value;
                Value = (1 + Mathf.Clamp(value, minBonus, maxBonus)) * OriginalValue;
                if (setBonusHandler != null)
                    setBonusHandler(prev_bonus, ref bonus);
            }
        }
    }

    public void Init(Unit unit)
    {
        this.unit = unit.unitInfo.UnitCtrl;
        value_current = OriginalValue;
    }
}
