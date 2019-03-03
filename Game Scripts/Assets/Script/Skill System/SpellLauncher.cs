using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
//注意，施法中的技能（除被动技能）无法被替换
//这个类表示玩家技能
public class Skill
{
    [SerializeField]
    private SkillName name = SkillName.unset;
    public SkillName Name
    {
        get { return name; }
        set
        {
            if (value == SkillName.unset)
            {
                name = SkillName.unset;
                return;
            }
            //施法中的技能（除被动技能）无法被替换
            if (IsCasting && data != null && !data.IsPassive)
                return;
            if (name != value || data == null)
            {
                //当被动技能被替换时，终止被动技能
                if (skillEffectBase != null && data != null && data.IsPassive)
                    skillEffectBase.End();
                data = Gamef.LoadSkillData(value);
                name = value;
                //如果新的技能是被动，发动技能
                if (data != null && data.IsPassive)
                    Spell();
            }
        }
    }
    public SkillMgr.SkillEffectBase skillEffectBase;
    public SkillData data;
    public UnitInfo caster;
    public bool IsCasting = false;
    FloatProperityWithBonus mp;


    //施法，即调用launcher
    public void Spell(params object[] Params)
    {
        if (!IsCasting && mp.Value >= data.ManaCost)
        {
            mp.Value -= data.ManaCost;
            SkillMgr.ReleaseSkill(this, Params);
        }
    }

    public void Init(UnitInfo caster, SkillName name)
    {
        this.caster = caster;
        mp = caster.UnitCtrl.unit.ManaPoint;
        Name = name;
    }

}

public enum SkillName
{
    unset = 0,
    SingleAttack = 1,
    多重火球 = 2,
    黑魔爆 = 3,
    迅捷术 = 4,
    聚能护盾 = 5,
    延迟射击 = 6,
}


public partial class GameCtrl
{
    #region 玩家施法
    private void BindHotKey4Skill()
    {
        //绑定施法快捷键
        InputMgr.BindHotKey(Shift2Skill1, KeyCode.Alpha1);
        InputMgr.BindHotKey(Shift2Skill2, KeyCode.Alpha2);
        InputMgr.BindHotKey(Shift2Skill3, KeyCode.Alpha3);
        InputMgr.BindHotKey(Shift2Skill4, KeyCode.Alpha4);

        InputMgr.BindHotKey(Shift2NextSkill, KeyCode.E);
        InputMgr.BindHotKey(Shift2PrevSkill, KeyCode.Q);
        //把检测技能按键输入加入Update事件
        EventMgr.UpdateEvent.AddListener(CheckInput4Skill);
    }
    private void CheckInput4Skill()
    {
        if (!InputMgr.HotKeyEnabled)
            return;
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Shift2NextSkill();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            Shift2PrevSkill();
        }
        if (Input.GetMouseButtonDown(0))
        {
            SkillMgr.PlayerReleaseCurrentSkill();
        }
    }
    //按下1, 切换为技能1 
    private void Shift2Skill1()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.CurrentIndex = 0;
    }
    //按下2, 切换为技能2
    private void Shift2Skill2()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.CurrentIndex = 1;
    }
    //按下3, 切换为技能3
    private void Shift2Skill3()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.CurrentIndex = 2;
    }
    //按下4, 切换为技能4
    private void Shift2Skill4()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.CurrentIndex = 3;
    }
    //按下E或滚轮, 切换到下一个技能
    private void Shift2NextSkill()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.NextSkill();
    }
    //按下Q或滚轮, 切换到上一个技能
    private void Shift2PrevSkill()
    {
        if (PlayerChara == null)
            return;
        PlayerChara.UnitCtrl.skillTable.PrevSkill();
    }
    #endregion
}