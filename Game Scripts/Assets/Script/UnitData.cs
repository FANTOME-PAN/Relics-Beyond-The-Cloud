using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitName
{
    Unset = 0,
    Player = 1,
    Capsule = 2,
}

[CreateAssetMenu(menuName = "My Data/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    /// <summary>
    /// 单位复数标签
    /// </summary>
    public UnitTags tags = new UnitTags();
    public float MaxVelocity;
    public float MaxTurningVelocity;
    public float MaxSheildPoint = 5f;
    public float ShieldPointRegenerationRate;
    public ArmorType ArmorType;
    public float MaxManaPoint;
    public float ManaPointRegenerationRate;

    [Header("Skill Table")]
    public bool IsCaster = false;
    public SkillName[] skills = new SkillName[4];
}
