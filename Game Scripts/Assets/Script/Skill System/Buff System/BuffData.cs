using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Data/Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("Buff 描述")]
    [TextArea(1, 3)]
    public string Desc = "这里可以输入描述";
    [Header("Basic Info")]
    /// <summary>
    /// buff名称
    /// </summary>
    public BuffName BuffName = BuffName.Unset;
    /// <summary>
    /// buff类型，用于考虑叠加和顶替关系
    /// </summary>
    public BuffType BuffType = BuffType.Unset;
    /// <summary>
    /// 是否是debuff
    /// </summary>
    public bool IsDebuff = false;
    public float Duration = 0f;
    //效果。比如增加速度的百分比、每秒造成的伤害等等
    public float Effect = 0f;
    //额外浮点型参数
    public float[] Params;
    [Header("特殊信息")]
    /// <summary>
    /// 是否可叠加
    /// </summary>
    public bool IsSuperimposable = false;
    /// <summary>
    /// 最大叠加层数（要求可叠加）
    /// </summary>
    public int maxLayer = 1;
    /// <summary>
    /// 是否考虑同类型buff优先级（如不考虑，则会顶替）
    /// </summary>
    public bool ConsiderPriority = false;
    /// buff优先级（如果考虑优先级）
    /// </summary>
    public int Priority = 0;
    /// <summary>
    /// 驱散优先级（越高，越难以驱散）
    /// </summary>
    public int DispelPriority = 0;

    [Header("On GUI")]
    /// <summary>
    /// 显示在UI的buff名字
    /// </summary>
    public string UIName;
    /// <summary>
    /// 显示在UI的buff简介
    /// </summary>
    public string UIDesc;
}
