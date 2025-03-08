using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    public Vector2 speedWeight; // 不同图层的移动权重
    private Transform cameraTrans;
    private Vector3 lastCameraPos;
    private float textUnitSizex;

    void Start() {
        cameraTrans = Camera.main.transform;
        lastCameraPos = cameraTrans.position;
        
        Sprite sprite = transform.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textUnitSizex = texture.width / sprite.pixelsPerUnit; // 纹理图宽度除像素宽度
    }

    void LateUpdate() {
        Vector3 offset = cameraTrans.position - lastCameraPos;
        transform.position += new Vector3(offset.x * speedWeight.x, offset.y * speedWeight.y, 0);
        lastCameraPos = cameraTrans.position;
        
        //背景循环
        if (Mathf.Abs(cameraTrans.position.x - transform.position.x )> textUnitSizex)
        {
            //偏移量
            float offsetPositionX = (cameraTrans.position.x - transform.position.x)%textUnitSizex;
            //移动背景图
            transform.position = new Vector3(cameraTrans.position.x+ offsetPositionX, transform.position.y);
            
        }
    }
}
