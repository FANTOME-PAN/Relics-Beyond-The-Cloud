using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingAttack : MonoBehaviour {
    #region 参数
    public float m_uptimer = 0f;
    public float m_currLife = 0f;
    public float m_MaxLife = 10f;
    public float m_RotSpeed = 5f;
    public float m_speed = 10f;
    #endregion
    UnitCtrl enemyPlayer;

	// Update is called once per frame
	void Update () {
        // 0.5秒前只前进和减速不进行追踪
        if (m_uptimer < 0.5f)
        {
            m_uptimer += Time.deltaTime;
            m_speed -= 2 * Time.deltaTime;
            transform.position += transform.forward * m_speed * Time.deltaTime;
        }
        else
        {
            // 追踪敌人
            Vector3 target = (enemyPlayer.transform.position - transform.position).normalized;
            float a = Vector3.Angle(transform.forward, target) / m_RotSpeed;
            if (a > 0.1f || a < -0.1f)
                transform.forward = Vector3.Slerp(transform.forward, target, Time.deltaTime / a).normalized;
            else
            {
                m_speed += 2 * Time.deltaTime;
                transform.forward = Vector3.Slerp(transform.forward, target, 1).normalized;
            }

            transform.position += transform.forward * m_speed * Time.deltaTime;
        }
        m_currLife += Time.deltaTime;
        if (m_currLife > m_MaxLife)
        {
            // 超过生命周期爆炸（不同与击中敌人）
            Destroy(gameObject);
            //Destroy(Instantiate(m_Explosion, transform.position, Quaternion.identity), 1.2f);
        }
    }

    void TrackingShoot()
    {

    }
}
