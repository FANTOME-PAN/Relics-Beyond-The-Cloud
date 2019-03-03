using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTracking : MonoBehaviour
{
    public float rotSpeed = 45f;
    public float rotConst = 2f;
    public float speed = 2f;
    public Transform target;
    public bool IsChellTracking = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    Vector3 dir, rotAxis;
    void Update()
    {
        if (target == null) return;
        if (IsChellTracking)
        {
            dir = (target.position - transform.position).normalized;
            rotAxis = Vector3.zero;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotConst * Time.deltaTime);
        }
        else
        {
            dir = (target.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            
            if (angle > 180f - 1e-4f)
            {
                if (angle > rotSpeed * Time.deltaTime)
                    angle = rotSpeed * Time.deltaTime;
                rotAxis = transform.up;
                transform.Rotate(rotAxis, angle, Space.World);
            }
            else if (angle > 1e-4f)
            {
                if (angle > rotSpeed * Time.deltaTime)
                    angle = rotSpeed * Time.deltaTime;
                rotAxis = Vector3.Cross(transform.forward, dir).normalized;
                transform.Rotate(rotAxis, angle, Space.World);
            }
            else
                rotAxis = transform.up;
        }
#if DEBUG
        Debug.DrawRay(transform.position, 5f * rotAxis, Color.green, Time.deltaTime);
        Debug.DrawLine(transform.position, target.position, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position, 5f * transform.forward, Color.blue, Time.deltaTime);
#endif
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);
    }
    Vector3 precastV;
    private void PrecastDir()
    {
        Vector3 
        dir = (target.position - transform.position).normalized;
        
    }
}
