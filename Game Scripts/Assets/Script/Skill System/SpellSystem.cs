using System;
using System.Collections.Generic;
using UnityEngine;
//player -> skill table -> skill -> launcher -> skill effect base (-> missile -> skill effect base)
//SkillMgr.PlayerRelease -> SkillMgr.Release -> SkillMgr.SkillEffectBase (...)
public class SkillMgr
{
    public const int MAX_SKILL_NUM = 100;
    public delegate void SkillReleaseHandler(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params);
    public delegate void PlayerInputHandler(Skill skill);
    private static SkillReleaseHandler[] releaseHandlers = new SkillReleaseHandler[MAX_SKILL_NUM];
    private static PlayerInputHandler[] playerInputOnStartHandlers = new PlayerInputHandler[MAX_SKILL_NUM];
    private static Action[] buildPathHandlers = new Action[MAX_SKILL_NUM];
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skill">技能</param>
    /// <param name="Params">参数</param>
    public static void ReleaseSkill(Skill skill, params object[] Params)
    {
        if (skill.Name == SkillName.unset)
        {
            return;
        }
        SkillEffectBase skillEffectBase;
        if (releaseHandlers[(int)skill.Name] == null)
        {
            Debug.Log(skill.Name.ToString() + ": 没有启动器");
            return;
        }
        releaseHandlers[(int)skill.Name](skill, out skillEffectBase, Params);
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skill">技能</param>
    /// <param name="skillEffectBase">技能效果基类</param>
    /// <param name="Params">参数</param>
    public static void ReleaseSkill(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        skillEffectBase = null;
        if (skill.Name != SkillName.unset)
        {
            Debug.LogError("启动器调用错误：技能名称不匹配");
            return;
        }
        if (releaseHandlers[(int)skill.Name] == null)
        {
            Debug.Log(skill.Name.ToString() + ": 没有启动器");
            return;
        }
        releaseHandlers[(int)skill.Name](skill, out skillEffectBase, Params);
    }

    /// <summary>
    /// 玩家施法从此调用
    /// </summary>
    /// <param name="skill"></param>
    public static void PlayerReleaseCurrentSkill()
    {
        Skill skill = GameCtrl.Instance.PlayerChara.UnitCtrl.skillTable.CurrentSkill;
        if (skill.Name == SkillName.unset)
        {
            return;
        }
        if (releaseHandlers[(int)skill.Name] == null)
        {
            Debug.Log(skill.Name.ToString() + ": 没有玩家调用方法");
            return;
        }
        playerInputOnStartHandlers[(int)skill.Name](skill);
    }

    /// <summary>
    /// 建立SkillDataPath
    /// </summary>
    public static void BuildSkillDataPath()
    {
        GameDB.Instance.skillDataPath.paths = new string[MAX_SKILL_NUM];
        foreach (Action action in buildPathHandlers)
        {
            if (action != null)
                action();
        }
    }

    /// <summary>
    /// 初始化所有技能
    /// </summary>
    public static void InitSkills()
    {
        MultipleFireballsMgr.BuildInstance();
        BlackMagicBlastMgr.BuildInstance();
        CelerityMgr.BuildInstance();
        Skill聚能护盾Mgr.BuildInstance();
        DelaySkillMgr.BuildInstance();
        SkillTestMgr.BuildInstance();
    }


    #region 实例部分
    protected SkillMgr(SkillName skillName)
    {
        this.skillName = skillName;
        Init();
    }
    protected SkillName skillName;

    //代替launcher功能
    public virtual void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        skillEffectBase = null;
        if (skill.Name != skillName)
        {
            Debug.LogError("启动器调用错误：技能名称不匹配");
            return;
        }
    }

    //玩家通过Input施法
    public virtual void PlayerRelease(Skill skill)
    {
        if (skill.Name != skillName)
        {
            Debug.LogError("玩家施法调用错误：技能名称不匹配");
            return;
        }
    }
    //初始化
    public void Init()
    {
        playerInputOnStartHandlers[(int)skillName] = PlayerRelease;
        releaseHandlers[(int)skillName] = Release;
        buildPathHandlers[(int)skillName] = BuildPath;
    }
    //
    public void BuildPath()
    {
        GameDB.Instance.skillDataPath.paths[(int)skillName] = "Skill/" + skillName.ToString() + "数据";
    }
    /// <summary>
    /// 技能效果基类
    /// </summary>
    public class SkillEffectBase
    {

        protected Skill skill;

        #region 生命周期
        public virtual void Start()
        {
            //
            skill.IsCasting = true;
            skill.skillEffectBase = this;
            EventMgr.UpdateEvent.AddListener(Update);
        }

        public virtual void Update()
        {
            //
            End();
        }

        public virtual void End()
        {
            EventMgr.UpdateEvent.RemoveListener(Update);
            skill.IsCasting = false;
            skill.skillEffectBase = null;
        }
        //投掷物撞到地形
        public virtual void HitTerrain(Missile self)
        {
            if (skill.data.IsAOE)
                Blast(self, null);
            self.TakeDamage((int)1e7);
        }
        //投掷物撞到其他投掷物
        public virtual void HitMissile(Missile self, Missile other)
        {
            self.TakeDamage(other.damage);
            if (skill.data.IsAOE && self.hp <= 0)
                Blast(self, other.gameObject);
        }
        //投掷物撞到单位
        public virtual void HitUnit(Missile self, UnitCtrl unit)
        {
            if (skill.data.IsAOE)
                Blast(self, null);
            self.TakeDamage((int)1e7);
        }
        //AOE爆炸
        protected enum BlastType
        {
            Inner, Middle, Outer,
        }

        protected delegate void UnitHandler(UnitCtrl unit, BlastType blastType);
        protected delegate void MissileHandler(Missile missile, BlastType blastType);
        protected UnitHandler unitHandler;
        protected MissileHandler missileHandler;

        public virtual void Blast(Missile self, GameObject exception)
        {
            List<UnitInfo> units = GameDB.unitPool.UnitList;
            List<Missile> missiles = GameDB.missilePool.MissileList;
            float dis, damage = skill.data.Damage;
            BlastType type;
            //对单位造成AOE伤害
            lock (GameDB.unitPool)
                foreach (UnitInfo other in units)
                {
                    if (!other.UnitCtrl.unit.isAlive)
                        continue;
                    dis = (other.Transform.position - self.transform.position).magnitude;
                    if (dis > skill.data.OuterBlastRadius)
                    {
                        //do nothing
                    }
                    else
                    {
                        if (dis > skill.data.MiddleBlastRadius)
                        {
                            //外圈伤害
                            damage = skill.data.Damage * skill.data.OuterDamageCoefficient;
                            type = BlastType.Outer;
                        }
                        else
                        {
                            if (dis > skill.data.InnerBlastRadius)
                            {
                                //中圈伤害
                                damage = skill.data.Damage * skill.data.MiddleDamageCoefficient;
                                type = BlastType.Middle;
                            }
                            else
                            {
                                //内圈伤害
                                damage = skill.data.Damage * skill.data.InnerDamageCoefficient;
                                type = BlastType.Inner;
                            }
                        }
                        Gamef.Damage(damage, DamageType.unset, skill.caster, other);
                        if (unitHandler != null)
                            unitHandler(other.UnitCtrl, type);
                    }
                }
            //对投掷物造成AOE伤害
            lock (GameDB.missilePool)
                foreach (Missile other in missiles)
                {
                    if (!other.IsAlive)
                        continue;
                    //忽略碰撞到的投掷物
                    if (other.gameObject == exception)
                        continue;
                    dis = (other.transform.position - self.transform.position).magnitude;
                    if (dis > skill.data.OuterBlastRadius)
                    {
                        //do nothing
                    }
                    else
                    {
                        if (dis > skill.data.MiddleBlastRadius)
                        {
                            //外圈伤害
                            damage = skill.data.Damage * skill.data.OuterDamageCoefficient;
                            type = BlastType.Outer;
                        }
                        else
                        {
                            if (dis > skill.data.InnerBlastRadius)
                            {
                                //中圈伤害
                                damage = skill.data.Damage * skill.data.MiddleDamageCoefficient;
                                type = BlastType.Middle;
                            }
                            else
                            {
                                //内圈伤害
                                damage = skill.data.Damage * skill.data.InnerDamageCoefficient;
                                type = BlastType.Inner;
                            }
                        }
                        other.TakeDamage(damage);
                        if (missileHandler != null)
                            missileHandler(other, type);
                    }
                }
        }

        #endregion
    }
    #endregion
}

public enum TrackingType
{
    NoTracking,
    WeakTracking,
    StrongTracking,
}


public class MultipleFireballsMgr : SkillMgr
{
    protected MultipleFireballsMgr() : base(SkillName.多重火球)
    {
    }

    public static MultipleFireballsMgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new MultipleFireballsMgr();
    }


    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);

        Gamef.Command.UnitReleaseCurrentSkill(GameCtrl.PlayerUnit, CameraCtrl.Instance.transform);
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);
        //收集信息
        if (Params.Length == 1 && Params[0] is Transform)
            skillEffectBase = new MultipleFireballs(skill, Params[0] as Transform);
    }

    public class MultipleFireballs : SkillEffectBase
    {
        Vector3 pos, dir;
        Transform fwd;
        public MultipleFireballs(Skill skill, Transform fwd)
        {
            this.skill = skill;
            skill.IsCasting = true;
            skill.skillEffectBase = this;
            this.fwd = fwd;
            Start();
        }

        public override void Start()
        {
            timer = dur;
            fireball = Resources.Load<GameObject>("Fireball");
            EventMgr.UpdateEvent.AddListener(Update);
        }
        float dur = 0.2f;
        float timer = 0.5f;
        int cnt = 0;
        GameObject fireball;
        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                if (cnt < 3)
                {
                    cnt++;
                    timer += dur;
                    Dartle();
                }
                else
                {
                    EventMgr.UpdateEvent.RemoveListener(Update);
                    timer = 1e5f;
                    End();
                }
            }
        }

        public override void End()
        {
            skill.IsCasting = false;
            skill.skillEffectBase = null;
        }

        public override void HitUnit(Missile self, UnitCtrl unit)
        {
            if (unit != null)
                Gamef.Damage(skill.data.Damage, DamageType.unset, skill.caster, unit.unitInfo);
            else Debug.Log("Hit unit without UnitCtrl component");
            self.TakeDamage((int)1e7);
        }

        void Dartle()
        {
            pos = fwd.position;
            dir = fwd.forward;
            pos += dir * 1.2f;
            lock (GameDB.skillBuffer)
            {
                GameDB.skillBuffer.skill = skill;
                GameDB.skillBuffer.skillEffectBase = this;
                UnityEngine.Object.Instantiate(fireball, pos, Quaternion.LookRotation(dir));
            }
        }
    }
}

public class BlackMagicBlastMgr : SkillMgr
{
    protected BlackMagicBlastMgr() : base(SkillName.黑魔爆)
    {
    }

    public static BlackMagicBlastMgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new BlackMagicBlastMgr();
    }

    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);
        Vector3 pos = CameraCtrl.Instance.transform.position;
        Vector3 dir = CameraCtrl.Instance.transform.forward;
        pos += dir * 1.5f;

        Gamef.Command.UnitReleaseCurrentSkill(GameCtrl.Instance.PlayerChara.UnitCtrl, pos, dir);
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);

        //收集信息
        if (Params.Length == 2 && Params[0] is Vector3 && Params[1] is Vector3)
        {
            Vector3 pos = (Vector3)Params[0];
            Vector3 dir = (Vector3)Params[1];

            skillEffectBase = new BlackMagicBlast(skill, pos, dir);
        }
    }

    public class BlackMagicBlast : SkillEffectBase
    {
        Vector3 pos, dir;
        public BlackMagicBlast(Skill skill, Vector3 position, Vector3 direction)
        {
            this.skill = skill;
            pos = position;
            skill.IsCasting = true;
            skill.skillEffectBase = this;
            dir = direction;
            //仅测试用
            //unitHandler = AddBuff2Enermy;
            Start();
        }

        public override void Start()
        {
            lock (GameDB.skillBuffer)
            {
                GameDB.skillBuffer.skill = skill;
                GameDB.skillBuffer.skillEffectBase = this;
                UnityEngine.Object.Instantiate(Resources.Load<GameObject>("BlackMagicBlast"), pos, Quaternion.LookRotation(dir));
            }
            EventMgr.UpdateEvent.AddListener(Update);
        }
        float timer = 0f;
        public override void Update()
        {
            timer += Time.deltaTime;
            if (timer >= skill.data.SpellTime)
            {
                EventMgr.UpdateEvent.RemoveListener(Update);
                skill.IsCasting = false;
                skill.skillEffectBase = null;
            }
        }
        //仅测试用
        void AddBuff2Enermy(UnitCtrl unit, BlastType type)
        {
            BuffHelper.AddBuff(BuffName.生命汲取, unit, skill.caster.UnitCtrl);
        }
    }
}

public class CelerityMgr : SkillMgr
{
    protected CelerityMgr() : base(SkillName.迅捷术)
    {
    }

    public static CelerityMgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new CelerityMgr();
    }
    Skill skill;
    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);
        this.skill = skill;
        Gamef.Command.UnitReleaseCurrentSkill(GameCtrl.PlayerUnit);
        EventMgr.UpdateEvent.AddListener(PlayerUpdate);
    }

    private void PlayerUpdate()
    {
        if (skill.caster.UnitCtrl.unit.isAlive && Input.GetMouseButton(0) && skill.IsCasting)
        {
            //do nothing
        }
        else
        {
            EventMgr.UpdateEvent.RemoveListener(PlayerUpdate);
            if (skill.IsCasting)
                skill.skillEffectBase.End();
        }
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);
        //收集信息
        if (Params.Length == 0)
            skillEffectBase = new Celerity(skill);
        // new test1(...)
    }

    public class Celerity : SkillEffectBase
    {
        public Celerity(Skill skill)
        {
            this.skill = skill;
            skill.IsCasting = true;
            skill.skillEffectBase = this;
            mp = skill.caster.UnitCtrl.unit.ManaPoint;
            Start();
        }

        public override void Start()
        {
            //skill.caster.UnitCtrl.unit.MaxV_bonus += skill.data.Params[0];
            Gamef.AddBuff(skill.data.Buffs[0], skill.caster.UnitCtrl, skill.caster.UnitCtrl);
            EventMgr.UpdateEvent.AddListener(Update);
        }
        FloatProperityWithBonus mp;
        public override void Update()
        {
            mp.Value -= skill.data.Params[1] * Time.deltaTime;
            if (mp.Value < 1e-5f)
            {
                End();
            }
        }

        public override void End()
        {
            base.End();
            Gamef.RemoveBuff(skill.data.Buffs[0], skill.caster.UnitCtrl);
        }
    }

}

public class Skill聚能护盾Mgr : SkillMgr
{
    protected Skill聚能护盾Mgr() : base(SkillName.聚能护盾)
    {
    }

    public static Skill聚能护盾Mgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new Skill聚能护盾Mgr();
    }

    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);
        Gamef.Command.UnitReleaseCurrentSkill(GameCtrl.PlayerUnit);
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);
        skillEffectBase = new Skill聚能护盾(skill);
    }

    public class Skill聚能护盾 : SkillEffectBase
    {
        public Skill聚能护盾(Skill skill)
        {
            this.skill = skill;
            unit = skill.caster.UnitCtrl.unit;
            Start();
        }
        Unit unit;
        bool isActivated = false;
        public override void Update()
        {
            if (!isActivated && unit.SheildPoint < skill.data.Params[0] * unit.MaxShieldPoint)
            {
                isActivated = true;
                Gamef.AddBuff(skill.data.Buffs[0], skill.caster.UnitCtrl, skill.caster.UnitCtrl);
            }
            else if (isActivated && unit.SheildPoint > skill.data.Params[0] * unit.MaxShieldPoint)
            {
                isActivated = false;
                Gamef.RemoveBuff(skill.data.Buffs[0], skill.caster.UnitCtrl);
            }
        }

    }
}

public class SkillTestMgr : SkillMgr
{
    protected SkillTestMgr() : base(SkillName.unset)
    {
    }

    public static SkillTestMgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new SkillTestMgr();
    }

    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);
    }

    public class SkillTest : SkillEffectBase
    {

    }
}
