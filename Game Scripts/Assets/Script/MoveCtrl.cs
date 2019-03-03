using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnitCtrl))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SRB))]
public class MoveCtrl : MonoBehaviour
{
    //public GameObject CastPrefab;

    private UnitCtrl unitCtrl;
    private Unit unit;
    private Rigidbody rigbody;
    private SRB srb;
    public Vector3 charaUp = Vector3.up;

    public Transform chara;
    public Transform eye;
    public Transform glass;
    public float speed = 0f;
    public float angularSpeed = 0f;

    private void Awake()
    {
        EventMgr.UnitBirthEvent.AddListener(Init);
        srb = GetComponent<SRB>();
    }

    private float v;
    private float h;
    private float ac;
    private void Update()
    {
        v = /*Input.GetAxis("Vertical")*/InputMgr.GetVerticalAxis();
        h = /*Input.GetAxis("Horizontal")*/InputMgr.GetHorizontalAxis();
        ac = Input.GetKey(InputMgr.AccelerationKey) ? 1f : 0f;
        eye.position = glass.position;
        eye.localEulerAngles = Vector3.forward * chara.localEulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (InputMgr.MobileControlKeyEnable)
        {
            Turning();
            Accerlerate();
        }
        DampedDrag();
        Balance();

        speed = rigbody.velocity.magnitude;
        angularSpeed = srb.angularVelocity.magnitude * Mathf.Rad2Deg;
    }
    /// <summary>
    /// 转向。即更新转向速度angularVelocity
    /// </summary>
    private void Turning()
    {
        if (!InputMgr.MobileControlKeyEnable) return;

        float tpH = h, tpV = v;

        srb.AddTorque(tpH * unit.AngularAcceleration * Mathf.Deg2Rad * Vector3.up, SRB.ForceMode.Acceleration);
        //if (Input.GetKey(InputMgr.ForwardKey))
        if ((tpV > 0 ? tpV : -tpV) > 1e-7f)
        {
            float tpx = transform.localEulerAngles.x > 180f ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x;
            float tmp = tpV * GameDB.PULL_UP_CONST * unit.MaxTurningV * Mathf.Deg2Rad;
            tpx += tmp * Time.fixedDeltaTime;
            if ((tpx > 0 ? tpx : -tpx) < GameDB.MAX_ROT_X)
                srb.angularVelocity.x = tmp;
            else
            {
                srb.angularVelocity.x = 0f;
                srb.XRot.eulerAngles = tpx > 0f ? Vector3.right * GameDB.MAX_ROT_X : Vector3.right * (360f - GameDB.MAX_ROT_X);
            }
        }
        else
        {
            float tpx = transform.localEulerAngles.x > 180f ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x;
            float tmp = (tpx > 0f ? -1f : 1f) * GameDB.X_AXIS_BALANCING_CONST * unit.MaxTurningV * Mathf.Deg2Rad;
            tpx += tmp * Time.fixedDeltaTime;
            if ((tpx > 0f && tmp > 0f) || (tpx < 0f && tmp < 0f))
            {
                srb.angularVelocity.x = 0f;
                srb.XRot.eulerAngles = Vector3.zero;
            }
            else
            {
                srb.angularVelocity.x = tmp;
            }

        }
        //srb.AddTorque(tpV * Vector3.right * GameDB.RESTITUTION_ROT_X_AXIS * 0.25f * Mathf.PI, SRB.ForceMode.Acceleration);
    }

    /// <summary>
    /// 加速。即更新速度Velocity
    /// </summary>
    private void Accerlerate()
    {
        rigbody.velocity += transform.forward * ac * unit.Acceleration * Time.fixedDeltaTime;
    }

    /// <summary>
    /// 产生阻尼，包括运动阻尼和角阻尼
    /// </summary>
    private void DampedDrag()
    {
        rigbody.velocity -= rigbody.velocity * GameDB.DAMPED_CONST * Time.fixedDeltaTime;
        srb.AddTorque(-srb.angularVelocity * GameDB.ANGULAR_DAMPED_CONST, SRB.ForceMode.Acceleration);
    }

    private void Balance()
    {
    }


    private bool isInit = false;
    private void Init(EventMgr.UnitBirthEventInfo info)
    {
        if (isInit)
            return;
        if (info.Unit.Obj != gameObject)
            return;
        unitCtrl = info.Unit.UnitCtrl;
        unit = unitCtrl.unit;
        rigbody = unitCtrl.rigbody;
        isInit = true;
    }
}
