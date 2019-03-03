using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Skill skill;
    SkillMgr.SkillEffectBase skillEffectBase;
    public float hp;
    bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
    }
    Transform target;
    [HideInInspector]
    public float damage;
    public int ID = -1;
    [Header("出生和死亡动画预制")]
    public GameObject BirthPrefab;
    public GameObject DeathPrefab;

    private void Awake()
    {
        skill = GameDB.skillBuffer.skill;
        skillEffectBase = GameDB.skillBuffer.skillEffectBase;
        hp = skill.data.MissileHP;
        damage = skill.data.Damage;
        if (gameObject.tag != "Missile")
            gameObject.tag = "Missile";
    }
    private void Start()
    {
        lock (GameDB.missilePool)
            Gamef.MissileBirth(this);
        if (BirthPrefab != null)
            Instantiate(BirthPrefab, transform.position, transform.rotation);
    }
    float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > skill.data.LifeSpan)
        {
            if (skill.data.IsAOE)
                skillEffectBase.Blast(this, null);
            gameObject.SetActive(false);
        }
        if (target != null)
            switch (skill.data.TrackingType)
            {
                case TrackingType.NoTracking:
                    break;
                case TrackingType.StrongTracking:
                    break;
                case TrackingType.WeakTracking:
                    break;
            }
    }

    private void FixedUpdate()
    {
        transform.Translate(skill.data.Speed * Vector3.forward * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        //忽略施法者本身
        if (obj == skill.caster.Obj)
            return;
        switch (obj.tag)
        {
            //撞到单位（或玩家）
            case "Player":
            case "Unit":
                UnitCtrl otherUnit = obj.GetComponent<UnitCtrl>();
                if (otherUnit == null)
                    Debug.Log("Cannot Find UnitCtrl Component");
                skillEffectBase.HitUnit(this, otherUnit);
                break;
            //撞到其他投掷物
            case "Missile":
                Missile otherMissile = obj.GetComponent<Missile>();
                if (otherMissile == null)
                    Debug.Log("Cannot Find Missile Component");
                skillEffectBase.HitMissile(this, otherMissile);
                break;
            //撞到其他物体（地形）
            default:
                skillEffectBase.HitTerrain(this);
                break;
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject, 1f);
    }

    private void OnDestroy()
    {
        lock (GameDB.missilePool)
            Gamef.MissileClear(ID);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive)
            return;
        hp -= damage;
        if (hp <= 0)
        {
            isAlive = false;
            if (DeathPrefab != null)
                Instantiate(DeathPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
