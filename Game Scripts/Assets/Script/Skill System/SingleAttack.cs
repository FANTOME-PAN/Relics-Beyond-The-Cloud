//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SingleAttack : SkillEffectBase {

//    Vector3 pos, dir;
//    public SingleAttack(Skill skill, Vector3 position, Vector3 direction)
//    {
//        this.skill = skill;
//        pos = position;
//        dir = direction;
//        Start();
//    }

//    public override void Start()
//    {
//        Object.Instantiate(GameDB.Instance.Bullet, pos, Quaternion.LookRotation(dir));
//    }


//    //   // Use this for initialization
//    //   void Start () 
//    //   {

//    //}

//    //   // 按下 fire 攻击
//    //   void Update()
//    //   {
//    //       if (Input.GetButtonDown("Fire1"))
//    //       {
//    //           SingleShoot();
//    //       }
//    //   }

//}
