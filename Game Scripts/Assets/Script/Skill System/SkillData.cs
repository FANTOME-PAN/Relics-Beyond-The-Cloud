using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Data/Skill Data")]
public class SkillData : ScriptableObject
{
    //伤害，射程，AOE，持续时间，花费
    //AOE包括内层、中层、外层的爆炸系数和爆炸半径
    [Header("Description")]
    [TextArea(1, 3)]
    public string Description = "这里可以输入说明";
    [Header("Basic Info")]
    public string Name;
    public SkillName SkillName;
    public float Damage;
    //效果，比如速度加成、生命恢复之类的
    public float[] Params;
    //最大射程
    public float Range = 50f;
    //投掷物飞行速度
    public float Speed = 15f;
    //存在时间
    public float LifeSpan = 4f;
    //施法时间
    public float SpellTime = 0.2f;
    public float ManaCost;
    //投掷物的生命值
    public float MissileHP = 5;
    [Header("AOE 效果")]
    //AOE
    public bool IsAOE = false;
    public float InnerBlastRadius;
    public float MiddleBlastRadius;
    public float OuterBlastRadius;
    public float InnerDamageCoefficient = 1f;
    public float MiddleDamageCoefficient = 0.5f;
    public float OuterDamageCoefficient = 0.2f;
    [Header("特殊效果")]
    public TrackingType TrackingType = TrackingType.NoTracking;
    public bool IsPassive = false;
    public BuffName[] Buffs;
}