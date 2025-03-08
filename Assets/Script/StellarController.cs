using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StellarController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float horizontalLimit = 8f;  // 水平移动边界
    public float verticalLimit = 4f;   // 垂直移动边界

    [Header("倾斜设置")]
    public float maxTiltAngle = 30f;   // 最大倾斜角度
    public float tiltSpeed = 5f;       // 倾斜速度
    public float returnSpeed = 3f;     // 自动回正速度

    [Header("翻滚设置")]
    public float rollSpeed = 1080f;    // 每秒旋转度数（360=1秒/圈）
    private bool isRolling = false;
    public float rollCooldown = 0.5f;  // 翻滚冷却时间

    private Vector3 startRotation;
    private Camera mainCam;

    void Start()
    {
        startRotation = transform.eulerAngles;
        mainCam = Camera.main;
    }

    void Update()
    {
 
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleTilt();
        HandleRoll();
    }

    void HandleMovement()
    {
        // 基础移动
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 newPos = transform.position + 
                        new Vector3(horizontal, vertical, 0) * 
                        moveSpeed * Time.deltaTime;

        // 屏幕边界限制
        Vector3 viewPos = mainCam.WorldToViewportPoint(newPos);
        viewPos.x = Mathf.Clamp(viewPos.x, 0.1f, 0.9f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.1f, 0.9f);
        transform.position = mainCam.ViewportToWorldPoint(viewPos);
    }

    void HandleTilt()
    {
        if (isRolling) return; // 🚫 翻滚时禁用倾斜回正
        
        float verticalInput = Input.GetAxis("Vertical");
        Quaternion targetRotation;

        // 当有垂直输入时倾斜
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            float tiltAmount = verticalInput * maxTiltAngle;
            targetRotation = Quaternion.Euler(startRotation.x, startRotation.y, tiltAmount);
        }
        // 无输入时回正
        else
        {
            targetRotation = Quaternion.Euler(startRotation);
        }

        // 平滑旋转
        transform.rotation = Quaternion.Lerp(
            transform.rotation, 
            targetRotation, 
            (verticalInput != 0 ? tiltSpeed : returnSpeed) * Time.deltaTime
        );
    }

    void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            StartCoroutine(PerformRoll());
        }
    }

    System.Collections.IEnumerator PerformRoll()
    {
        isRolling = true;
        float targetRotation = 360f; // 目标总旋转量
        float rotatedAmount = 0f;    // 已旋转量
        
        Quaternion initialRot = transform.rotation;

        // 逐帧旋转直到完成360度
        while (rotatedAmount < targetRotation)
        {
            float delta = rollSpeed * Time.deltaTime;
            delta = Mathf.Min(delta, targetRotation - rotatedAmount);
            
            // 关键点：使用局部坐标系绕Z轴旋转
            transform.Rotate(Vector3.forward, delta, Space.Self);
            
            rotatedAmount += delta;
            yield return null;
        }

        // 精确对齐初始角度（避免浮点误差）
        transform.rotation = initialRot;
        isRolling = false;
    }
}
