using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    #region 参数
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public bool clampHorizontalRotation = true;
    public float MinX = -90f;
    public float MaxX = 90f;
    public float MinY = -90f;
    public float MaxY = 90f;
    public bool smooth = true;
    public float smoothTime = 15f;
    public bool lockCursor = true;

    Transform cameraX;
    Transform cameraY;
    Quaternion cameraTargetYRot;
    Quaternion cameraTargatXRot;
    #endregion
    public static CameraCtrl Instance
    {
        get; private set;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //辅助计算x轴和y轴旋转
        GameObject emptyObj = GameDB.Instance.EmptyObject;
        cameraX = Instantiate(emptyObj).transform;
        cameraY = Instantiate(emptyObj).transform;
        //cameraX = transform.GetChild(0);
        //cameraY = transform;
        cameraTargatXRot = cameraX.localRotation;
        cameraTargetYRot = cameraY.localRotation;

        lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        CheckInput();
        UpdateCameraRotation();
    }

    private float xRot, yRot;
    private void UpdateCameraRotation()
    {
        xRot = Input.GetAxis("Mouse Y") * XSensitivity;
        yRot = Input.GetAxis("Mouse X") * YSensitivity;
        cameraTargatXRot *= Quaternion.Euler(-xRot, 0f, 0f);
        cameraTargetYRot *= Quaternion.Euler(0f, yRot, 0f);

        if (clampVerticalRotation)
        {
            cameraTargatXRot = ClampRotationAroundXAxis(cameraTargatXRot);
        }
        if (clampHorizontalRotation)
        {
            cameraTargetYRot = ClampRotationAroundYAxis(cameraTargetYRot);
        }

        if (smooth)
        {
            cameraX.localRotation = Quaternion.Slerp(cameraX.localRotation, cameraTargatXRot, Time.deltaTime * smoothTime);
            cameraY.localRotation = Quaternion.Slerp(cameraY.localRotation, cameraTargetYRot, Time.deltaTime * smoothTime);
        }
        else
        {
            cameraX.localRotation = cameraTargatXRot;
            cameraY.localRotation = cameraTargetYRot;
        }
        transform.localEulerAngles = new Vector3(cameraX.localEulerAngles.x, cameraY.localEulerAngles.y, 0f);
    }

    private void CheckInput()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && lockCursor)
        {
            lockCursor = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonUp(0) && !lockCursor)
        {
            lockCursor = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = 2f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinX, MaxX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    private Quaternion ClampRotationAroundYAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleY = 2f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleY = Mathf.Clamp(angleY, MinY, MaxY);

        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        return q;
    }

}
