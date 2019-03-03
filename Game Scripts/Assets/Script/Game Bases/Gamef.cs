using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Gamef
{
    #region 游戏机制
    /// <summary>
    /// 对单位造成伤害
    /// </summary>
    /// <param name="rawDamage">原始伤害值</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="dealer">输出者</param>
    /// <param name="receiver">接受者</param>
    public static void Damage(float rawDamage, DamageType damageType, UnitInfo dealer, UnitInfo receiver)
    {
        float damage = DamageCalculate(rawDamage, damageType, receiver.UnitCtrl.unit.ArmorType);
        receiver.UnitCtrl.TakeDamage(damage);
        //触发事件
    }

    /// <summary>
    /// 对单位造成伤害
    /// </summary>
    /// <param name="rawDamage">原始伤害值</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="dealer">输出者</param>
    /// <param name="receiver">接受者</param>
    public static void Damage(float rawDamage, DamageType damageType, UnitCtrl dealer, UnitCtrl receiver)
    {
        float damage = DamageCalculate(rawDamage, damageType, receiver.unit.ArmorType);
        receiver.TakeDamage(damage);
        //触发事件
    }

    /// <summary>
    /// 治疗单位
    /// </summary>
    /// <param name="amount">治疗量</param>
    /// <param name="dealer">输出者</param>
    /// <param name="receiver">接受者</param>
    public static void Heal(float amount, UnitInfo dealer, UnitInfo receiver)
    {
        receiver.UnitCtrl.BeHealed(amount);
        //触发事件
    }

    /// <summary>
    /// 治疗单位
    /// </summary>
    /// <param name="amount">治疗量</param>
    /// <param name="dealer">输出者</param>
    /// <param name="receiver">接受者</param>
    public static void Heal(float amount, UnitCtrl dealer, UnitCtrl receiver)
    {
        receiver.BeHealed(amount);
        //触发事件
    }
    
    /// <summary>
    /// 给目标单位增加buff效果
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="target">目标单位</param>
    /// <param name="caster">施法单位</param>
    public static void AddBuff(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        BuffHelper.AddBuff(name, target, caster);
    }

    /// <summary>
    /// 结束目标单位的buff效果
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="target">目标单位</param>
    public static void RemoveBuff(BuffName name, UnitCtrl target)
    {
        BuffHelper.RemoveBuff(name, target);
    }


    /// <summary>
    /// 将单位加入单位池
    /// </summary>
    /// <param name="unitCtrl">单位的UnitCtrl组件</param>
    public static void UnitBirth(UnitCtrl unitCtrl)
    {
        GameDB.Instance.UnitBirth(unitCtrl);
        //构造并触发事件
        EventMgr.UnitBirthEventInfo info = new EventMgr.UnitBirthEventInfo(unitCtrl.unit.unitInfo);
        EventMgr.UnitBirthEvent.OnTrigger(info);
    }

    /// <summary>
    /// 将单位移除单位池
    /// </summary>
    /// <param name="unitID">单位ID</param>
    public static void UnitClear(int unitID)
    {
        UnitInfo unitInfo = GameDB.Instance.GetUnit(unitID);
        GameDB.Instance.UnitClear(unitID);
        //构造并触发事件
        EventMgr.UnitClearEventInfo info = new EventMgr.UnitClearEventInfo(unitInfo);
        EventMgr.UnitClearEvent.OnTrigger(info);
    }
    /// <summary>
    /// 将单位移除单位池
    /// </summary>
    /// <param name="unitCtrl">单位控制组件</param>
    public static void UnitClear(UnitCtrl unitCtrl)
    {
        UnitClear(unitCtrl.unit.ID);
    }
    /// <summary>
    /// 将单位移除单位池
    /// </summary>
    /// <param name="info">单位信息</param>
    public static void UnitClear(UnitInfo info)
    {
        UnitClear(info.UnitCtrl.unit.ID);
    }

    /// <summary>
    /// 通过ID获得单位信息
    /// </summary>
    /// <param name="unitID">单位ID</param>
    /// <returns>单位信息</returns>
    public static UnitInfo GetUnit(int unitID)
    {
        return GameDB.unitPool.GetUnit(unitID);
    }

    /// <summary>
    /// 将投掷物加入池
    /// </summary>
    /// <param name="missile">投掷物组件</param>
    public static void MissileBirth(Missile missile)
    {
        GameDB.Instance.MissileBirth(missile);
    }
    /// <summary>
    /// 将投掷物移出池
    /// </summary>
    /// <param name="id">投掷物ID</param>
    public static void MissileClear(int id)
    {
        GameDB.Instance.MissileClear(id);
    }
    /// <summary>
    /// 通过ID获取投掷物
    /// </summary>
    /// <param name="id">投掷物ID</param>
    /// <returns></returns>
    public static Missile GetMissile(int id)
    {
        return GameDB.missilePool.GetMissile(id);
    }

    /// <summary>
    /// 计算单位实际受伤
    /// </summary>
    /// <param name="rawDamage">原伤害量</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="armorType">护甲类型</param>
    /// <returns></returns>
    public static float DamageCalculate(float rawDamage, DamageType damageType, ArmorType armorType)
    {
        return rawDamage;
    }

    #endregion

    #region 游戏指令
    public static class Command
    {
        /// <summary>
        /// 命令单位释放当前技能
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="Params">技能参数</param>
        public static void UnitReleaseCurrentSkill(UnitCtrl unit, params object[] Params)
        {
            if (unit != null)
                unit.Spell(Params);
            else
                Debug.Log("不能命令 null 单位施法");
        }
        /// <summary>
        /// 命令单位切换技能
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="index">技能序号(1,2,3,4)</param>
        public static void UnitShiftSkill(UnitCtrl unit, int index)
        {
            unit.skillTable.CurrentIndex = index - 1;
        }

        /// <summary>
        /// 令单位更换当前技能为目标技能
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="skill">目标技能</param>
        public static void UnitReplaceCurrentSkill(UnitCtrl unit, SkillName skill)
        {
            unit.skillTable.CurrentSkill.Name = skill;
        }
        /// <summary>
        /// 尝试获取单位当前技能的技能效果类
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="skillEffectBase">技能效果基类</param>
        /// <returns></returns>
        public static bool TryGetUnitSkillEffectBase(UnitCtrl unit, out SkillMgr.SkillEffectBase skillEffectBase)
        {
            skillEffectBase = unit.skillTable.CurrentSkill.skillEffectBase;
            return skillEffectBase != null;
        }
        /// <summary>
        /// 尝试获取单位目标技能栏的技能效果类
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="index">技能序号</param>
        /// <param name="skillEffectBase">技能效果基类</param>
        /// <returns></returns>
        public static bool TryGetUnitSkillEffectBase(UnitCtrl unit, int index, out SkillMgr.SkillEffectBase skillEffectBase)
        {
            skillEffectBase = unit.skillTable.skills[index].skillEffectBase;
            return skillEffectBase != null;
        }
    }
    #endregion

    #region 函数曲线
    ///// <summary>
    ///// 反正切函数（曲线优化）
    ///// </summary>
    ///// <param name="t">正切值</param>
    ///// <returns>对应的角度</returns>
    //public static float Atan(float t)
    //{
    //    if (t < 80.5f)
    //        return GameDB.AtanCurve.Evaluate(t);
    //    return Mathf.Atan(t) * Mathf.Rad2Deg;
    //}
    #endregion

    #region 存取数据
    /// <summary>
    /// 读取单位数据
    /// </summary>
    /// <param name="name">单位名称</param>
    /// <returns>单位数据</returns>
    public static UnitData LoadUnitData(UnitName name)
    {
#if DEBUG
        if (name < 0 || (int)name >= GameDB.Instance.unitDataPath.paths.Length)
        {
            Debug.Log("单位名 name 没有对应路径");
            return null;
        }
#endif
        return Resources.Load<UnitData>(GameDB.Instance.unitDataPath.paths[(int)name]);
    }

    /// <summary>
    /// 读取技能数据
    /// </summary>
    /// <param name="name">技能名称</param>
    /// <returns>技能数据</returns>
    public static SkillData LoadSkillData(SkillName name)
    {
        Debug.Log("Load " + name);
        return Resources.Load<SkillData>(GameDB.Instance.skillDataPath.paths[(int)name]);
    }

    /// <summary>
    /// 读取技能数据
    /// </summary>
    /// <param name="name">技能名称</param>
    /// <returns>技能数据</returns>
    public static BuffData LoadBuffData(BuffName name)
    {
        Debug.Log("Load " + name);
        return Resources.Load<BuffData>(GameDB.Instance.buffDataPath.paths[(int)name]);
    }
    #endregion
}
