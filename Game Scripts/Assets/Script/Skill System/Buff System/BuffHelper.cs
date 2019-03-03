using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffHelper
{
    private delegate void Handler(BuffName name, UnitCtrl target, UnitCtrl caster);
    private static Handler[] handlers = new Handler[]
    {
        null,//0
        AddBuff_Buff迅捷术,//1
        AddBuff_Buff聚能,//2
        AddBuff_Buff生命汲取,//3
    };
    #region Interfaces
    /// <summary>
    /// 给目标单位增加buff效果
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="target">目标单位</param>
    /// <param name="caster">施法单位</param>
    public static void AddBuff(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        handlers[(int)name](name, target, caster);
    }

    /// <summary>
    /// 结束目标单位的buff效果
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="target">目标单位</param>
    public static void RemoveBuff(BuffName name, UnitCtrl target)
    {
        BuffBase buff;
        if (target.ContainsBuff(name, out buff))
            buff.End();
    }
    #endregion

    private static void AddBuff_Buff迅捷术(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        new Buff迅捷术(name, target, caster);
    }
    private static void AddBuff_Buff聚能(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        new Buff聚能(name, target, caster);
    }
    private static void AddBuff_Buff生命汲取(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        new Buff生命汲取(name, target, caster);
    }
}
