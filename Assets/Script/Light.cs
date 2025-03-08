using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public Transform stellar;  // 飞机的Transform
    private Vector3 relativePos; // 存储光源相对于飞机的初始相对位置
    
    public float rollSpeed = 360f;  // 翻滚速度（每秒旋转多少度）
    private bool isRolling = false;  // 是否正在翻滚
    // Start is called before the first frame update
    void Start()
    {
        // 记录光源相对于飞机的初始位置
        relativePos = transform.position - stellar.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 让光源跟随飞机的X和Y坐标
        transform.position = new Vector3(stellar.position.x + relativePos.x, stellar.position.y + relativePos.y, transform.position.z);
    }
}
