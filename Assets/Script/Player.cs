using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rollSpeed = 200f;  // 飞机翻滚的速度（左右）
    private CharacterController controller;
    private Camera mainCamera;
    
    [Header("屏幕边距")]
    public float horizontalMargin = 0.1f;
    public float verticalMargin = 0.1f;
    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
        // 获取主摄像机
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // 计算原始移动向量
        Vector3 move = new Vector3(horizontal, vertical, 0);
        // 计算预期位置
        Vector3 expectedPosition = transform.position + move * moveSpeed * Time.deltaTime;
        // 将预期位置限制在屏幕范围内
        Vector3 clampedPosition = ClampPosition(expectedPosition);
        // 计算修正后的移动向量
        Vector3 finalMove = clampedPosition - transform.position;
        controller.Move(finalMove);
        
        float roll = vertical * rollSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward * -roll); // 左右翻滚
    }
    
    void OnDrawGizmos() {
        if (!mainCamera) return;
    
        Vector3[] corners = new Vector3[4];
        float depth = -mainCamera.transform.position.z;
        corners[0] = mainCamera.ViewportToWorldPoint(new Vector3(horizontalMargin, verticalMargin, depth));
        corners[1] = mainCamera.ViewportToWorldPoint(new Vector3(1-horizontalMargin, verticalMargin, depth));
        corners[2] = mainCamera.ViewportToWorldPoint(new Vector3(1-horizontalMargin, 1-verticalMargin, depth));
        corners[3] = mainCamera.ViewportToWorldPoint(new Vector3(horizontalMargin, 1-verticalMargin, depth));

        Gizmos.color = Color.green;
        for (int i = 0; i < 4; i++) {
            Gizmos.DrawLine(corners[i], corners[(i+1)%4]);
        }
    }
    
    Vector3 ClampPosition(Vector3 targetPosition) {
        // 将目标位置转换为视口坐标
        Vector3 viewPos = mainCamera.WorldToViewportPoint(targetPosition);
    
        // 水平方向限制
        viewPos.x = Mathf.Clamp(viewPos.x, horizontalMargin, 1 - horizontalMargin);
    
        // 垂直方向限制
        viewPos.y = Mathf.Clamp(viewPos.y, verticalMargin, 1 - verticalMargin);
    
        // 转换回世界坐标
        return mainCamera.ViewportToWorldPoint(viewPos);
    }
    
}
