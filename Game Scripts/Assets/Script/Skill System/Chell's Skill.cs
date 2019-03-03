using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---2019.2.24 TEST---
public class DelaySkillMgr : SkillMgr
{
    protected DelaySkillMgr() : base(SkillName.延迟射击)
    {
    }

    public static DelaySkillMgr Instance
    {
        get; private set;
    }

    public static void BuildInstance()
    {
        Instance = Instance ?? new DelaySkillMgr();
    }


    public override void PlayerRelease(Skill skill)
    {
        base.PlayerRelease(skill);
    }

    public override void Release(Skill skill, out SkillEffectBase skillEffectBase, params object[] Params)
    {
        base.Release(skill, out skillEffectBase, Params);
    }
   
    public class DelaySkill : SkillEffectBase
    {
        Vector3 pos, dir;

        public DelaySkill(Skill skill)
        {
            this.skill = skill;
            skill.IsCasting = true;
            skill.skillEffectBase = this;
            Start();
        }

        public override void Start()
        {
            //base.Start();
            timer = 1.0f;
            fireball = Resources.Load<GameObject>("Fireball");
            //EventMgr.UpdateEvent.AddListener(Update);
        }

        GameObject fireball;
        float timer;

        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                FireBeam();
            }
        }

        void FireBeam()
        {
            pos = CameraCtrl.Instance.transform.position;
            dir = CameraCtrl.Instance.transform.forward;
            pos += dir * 1.2f;
            lock (GameDB.skillBuffer)
            {
                GameDB.skillBuffer.skill = skill;
                GameDB.skillBuffer.skillEffectBase = this;
                Object.Instantiate(fireball, pos, Quaternion.LookRotation(dir));
            }
        }
    }
}