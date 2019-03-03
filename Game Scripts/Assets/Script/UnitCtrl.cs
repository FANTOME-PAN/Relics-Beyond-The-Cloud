using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UnitCtrl : MonoBehaviour
{
    public Rigidbody rigbody;
    public Transform BulletSpawnPoint;
    public Unit unit;
    public UnitInfo unitInfo;
    public Canvas unitCanvas;
    public Transform camera;

    #region 生命周期
    private void Awake()
    {
        rigbody = GetComponent<Rigidbody>();
        //加入监听SP变化事件
        EventMgr.SPChangeEvent.AddListener(SPEvent);
    }

    private void Start()
    {
        //注册单位
        lock (GameDB.unitPool)
            Gamef.UnitBirth(this);
        if (unit.data.IsCaster)
        {
            //将技能施法者设置为自己, 初始化技能
            for (int i = 0; i < 4; i++)
            {
                skillTable.skills[i].Init(unitInfo, unit.data.skills[i]);
            }
            skillTable.CurrentIndex = 0;
        }
        //测试用
        if (unit.name == UnitName.Player)
        {
            GameCtrl.Instance.PlayerChara = unitInfo;
            StartCoroutine(DisplayProperity());
        }
    }

    private void Update()
    {
        //回复 护盾值 和 魔法值
        unit.ManaPoint.Value += unit.MPRegenerationRate.Value * Time.deltaTime;
        unit.SheildPoint += unit.SPRegenerationRate.Value * Time.deltaTime;

        //触发buff效果
        if (BuffEvent != null)
            BuffEvent();

        //单位画布面对摄像机
        if (unitCanvas != null)
            unitCanvas.transform.forward = camera.position - unitCanvas.transform.position;
    }

    IEnumerator DisplayProperity()
    {
        while (true)
        {
            DisplayPlayerProperity.Instance.SetText(unit.SheildPoint, unit.MaxShieldPoint, unit.ManaPoint.Value, unit.MaxManaPoint.Value);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDisable()
    {
        //删除物体
        Destroy(gameObject, 1f);
    }

    private void OnDestroy()
    {
        //注销单位
        lock (GameDB.unitPool)
            Gamef.UnitClear(this);
    }
    #endregion

    #region 技能
    [System.Serializable]
    public class SkillTable
    {
        //技能数组
        public Skill[] skills = new Skill[4];
        private int _currentIndex = 0;
        //当前技能序号
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                //施法中禁止切换主动技能
                if (CurrentSkill.IsCasting && !CurrentSkill.data.IsPassive)
                {
                    Debug.Log("Skill is casting !");
                    return;
                }
                if (skills[value & 0x3].Name == SkillName.unset)
                {
                    Debug.Log("空技能");
                    return;
                }
                _currentIndex = value & 0x3;
                //
                if (CurrentSkill.caster.UnitCtrl.unit.name == UnitName.Player)
                    DisplaySkillName.Instance.SetText(CurrentSkill.data.Name);
                Debug.Log("Shift to Skill " + (_currentIndex + 1));
            }
        }
        //当前技能
        public Skill CurrentSkill
        {
            get { return skills[_currentIndex]; }
        }
        //上一个技能
        public void PrevSkill()
        {
            CurrentIndex--;
        }
        //下一个技能
        public void NextSkill()
        {
            CurrentIndex++;
        }
    }
    public SkillTable skillTable = new SkillTable();
    /// <summary>
    /// 玩家施法, 一般点击施法
    /// </summary>
    public void Spell(params object[] Params)
    {
        if (skillTable.CurrentSkill == null || skillTable.CurrentSkill.data.IsPassive)
        {
            Debug.Log("该技能栏为空 或 该技能为被动技能，不可主动释放");
            return;
        }
        skillTable.CurrentSkill.Spell(Params);
    }

    #endregion

    #region 生命值
    /// <summary>
    /// 单位受伤
    /// </summary>
    /// <param name="amount">伤害值</param>
    public void TakeDamage(float amount)
    {
        if (amount < 0)
        {
            Debug.Log("这已经不是挠痒痒的伤害了");
        }
        //减少护盾值
        unit.SheildPoint -= amount;
    }

    /// <summary>
    /// 单位回复护盾护盾
    /// </summary>
    /// <param name="amount">回复量</param>
    public void BeHealed(float amount)
    {
        if (amount < 0)
        {
            Debug.Log("你真是口毒奶");
        }
        //回复护盾值
        unit.SheildPoint += amount;
    }

    /// <summary>
    /// 护盾值事件，包括因为护盾损失和恢复在内的一切效果的显现等。
    /// 考虑以后加入Death类，作为静态函数，统一处理。
    /// </summary>
    /// <param name="info"></param>
    private void SPEvent(EventMgr.SPChangeEventInfo info)
    {
        //只负责实现当前挂载单位的效果
        if (info.Unit.UnitCtrl != this) return;
        //SP过低，死亡
        if (info.CurrentValue <= 0)
            SimpleDeath();
    }

    private void SimpleDeath()
    {
        if (!unit.isAlive)
            return;
        unit.isAlive = false;
        //清空所有buff
        while (buffs.Count > 0)
            LogOffBuff(buffs[0]);
        Debug.Log(gameObject.name + " has died.");
        gameObject.SetActive(false);
    }
    #endregion
}
