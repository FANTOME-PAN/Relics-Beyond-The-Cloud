using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UnitCtrl : MonoBehaviour
{
    public List<BuffBase> buffs = new List<BuffBase>();
    private event Action BuffEvent;

    /// <summary>
    /// 注册buff，给单位增加buff的时候需要先注册
    /// </summary>
    /// <param name="buff">注册对象，必须是挂载在单位上的</param>
    /// <returns>true表示成功注册</returns>
    public bool RegBuff(BuffBase buff)
    {
        lock (buffs)
            buffs.Add(buff);
        BuffEvent += buff.Update;
        Debug.Log(gameObject.name + " register buff : " + buff.data.BuffName.ToString());
        return true;
    }
    /// <summary>
    /// 注销buff，在buff失效时需要注销
    /// </summary>
    /// <param name="buff">注销对象</param>
    /// <returns>true表示成功注销</returns>
    public bool LogOffBuff(BuffBase buff)
    {
        lock (buffs)
            if (buffs.Contains(buff))
            {
                buffs.Remove(buff);
                BuffEvent -= buff.Update;
                Debug.Log(gameObject.name + " log off buff : " + buff.data.BuffName.ToString());
                return true;
            }
        return false;
    }
    /// <summary>
    /// 注销所有同名buff，在buff失效时需要注销
    /// </summary>
    /// <param name="name">注销buff名称</param>
    /// <returns>true表示成功注销，false表示单位没有该名称的buff</returns>
    public bool LogOffBuff(BuffName name)
    {
        bool res = false;
        lock (buffs)
            buffs.RemoveAll(delegate (BuffBase buff)
            {
                if (buff.data.BuffName == name)
                {
                    res = true;
                    Debug.Log(gameObject.name + " log off buff : " + buff.data.BuffName.ToString());
                    return true;
                }
                return false;
            });
        return res;
    }
    /// <summary>
    /// 检测是否存在buff
    /// </summary>
    /// <param name="name">buff类型</param>
    /// <returns></returns>
    public bool ContainsBuff(BuffName name)
    {
        BuffBase tmp;
        return ContainsBuff(name, out tmp);
    }

    /// <summary>
    /// 检测是否存在buff。一般用于可叠加buff。
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="buffList">目标buff列表</param>
    /// <param name="count">buff数量</param>
    /// <returns>是否存在buff</returns>
    public bool ContainsBuff(BuffName name, out List<BuffBase> buffList, out int count)
    {
        bool res = ContainsBuff(name, out buffList);
        count = buffList.Count;
        return res;
    }

    /// <summary>
    /// 检测是否存在buff。一般用于可叠加buff。
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="buffList">目标buff列表</param>
    /// <returns>是否存在buff</returns>
    public bool ContainsBuff(BuffName name, out List<BuffBase> buffList)
    {
        buffList = new List<BuffBase>();
        lock (buffs)
            foreach (BuffBase buff in buffs)
            {
                if (buff.data.BuffName == name)
                {
                    buffList.Add(buff);
                }
            }
        return buffList.Count > 0;
    }

    /// <summary>
    /// 检测是否存在buff。可叠加buff返回第一个。
    /// </summary>
    /// <param name="name">buff名称</param>
    /// <param name="buffBase">相应的BuffBase对象</param>
    /// <returns>是否存在buff</returns>
    public bool ContainsBuff(BuffName name, out BuffBase buffBase)
    {
        buffBase = null;
        lock (buffs)
            foreach (BuffBase item in buffs)
            {
                if (item.data.BuffName == name)
                {
                    buffBase = item;
                    return true;
                }
            }
        return false;
    }
    //???????
    //???????
    //同类顶替？？？？怎么弄的？？？
    /// <summary>
    /// 检测是否存在buff类型。
    /// </summary>
    /// <param name="type">buff类型</param>
    /// <param name="buffBase">相应的BuffBase对象</param>
    /// <returns>是否存在buff</returns>
    public bool ContainsBuffType(BuffType type, out BuffBase buffBase)
    {
        buffBase = null;
        lock (buffs)
            foreach (BuffBase item in buffs)
            {
                if (item.data.BuffType == type)
                {
                    buffBase = item;
                    return true;
                }
            }
        return false;
    }
}
