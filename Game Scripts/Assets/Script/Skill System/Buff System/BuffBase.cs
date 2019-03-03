using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffName
{
    Unset = 0,
    迅捷术 = 1,
    聚能 = 2,
    生命汲取 = 3,
}

public enum BuffType
{
    Unset = 0,

}

public class BuffBase
{
    public BuffData data;
    protected float startTime;
    public UnitCtrl target;
    public UnitCtrl caster;

    //buff加载（无关buff实现的基本信息的加载）
    public BuffBase(BuffName name, UnitCtrl target, UnitCtrl caster)
    {
        data = Gamef.LoadBuffData(name);
        this.target = target;
        this.caster = caster;
        Start();
    }

    //buff初始化（负责注册buff、添加buff效果等等）
    public virtual void Start()
    {
        startTime = Time.time;
        //注册
        BuffBase buff;
        //如果不是可叠加的buff，顶替掉原来的buff
        if (!data.IsSuperimposable)
        {
            if (target.ContainsBuff(data.BuffName, out buff))
            {
                target.LogOffBuff(buff);
            }
        }
        //如果可叠加，并且达到最大叠加层数，顶替最早的buff
        else
        {
            List<BuffBase> buffs;
            if (target.ContainsBuff(data.BuffName, out buffs))
            {
                //达到最大叠加层数，顶替最早的buff
                if (buffs.Count >= data.maxLayer)
                {
                    target.LogOffBuff(buffs[0]);
                }
            }
        }
        target.RegBuff(this);
    }

    //buff的实时效果（如果有的话）。由单位的Update调用，在注册buff时会将Update加入单位的BuffEvent。
    public virtual void Update()
    {
        if (Time.time - startTime >= data.Duration)
            End();
    }

    //buff结束，解除buff效果并注销buff
    public virtual void End()
    {
        //注销
        target.LogOffBuff(this);
    }

}

public class Buff迅捷术 : BuffBase
{
    public Buff迅捷术(BuffName name, UnitCtrl target, UnitCtrl caster) : base(name, target, caster)
    {
    }
    public override void Start()
    {
        base.Start();
        target.unit.MaxV_bonus += data.Effect;
    }

    public override void Update()
    {

    }

    public override void End()
    {
        base.End();
        target.unit.MaxV_bonus -= data.Effect;
    }
}

public class Buff聚能 : BuffBase
{
    public Buff聚能(BuffName name, UnitCtrl target, UnitCtrl caster) : base(name, target, caster)
    {
    }
    public override void Start()
    {
        base.Start();
        target.unit.SPRegenerationRate.Bonus += data.Effect;
    }

    public override void Update()
    {

    }

    public override void End()
    {
        base.End();
        target.unit.SPRegenerationRate.Bonus -= data.Effect;
    }
}

public class Buff生命汲取 : BuffBase
{
    public Buff生命汲取(BuffName name, UnitCtrl target, UnitCtrl caster) : base(name, target, caster)
    {
    }
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        Gamef.Damage(data.Effect * Time.deltaTime, DamageType.unset, caster, target);
        Gamef.Heal(data.Params[0] * Time.deltaTime, caster, caster);
    }

    public override void End()
    {
        base.End();
    }
}