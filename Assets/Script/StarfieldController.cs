using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StarfieldController : MonoBehaviour {
    [Header("运动设置")]
    [Range(0, 10)] public float speed = 1f;
    [Range(0.5f, 5f)] public float density = 2f;

    [Header("视觉效果")]
    [Range(0, 1)] public float twinkleIntensity = 0.5f;
    [Range(0, 5)] public float twinkleSpeed = 1f;
    public Gradient colorPreset;

    private Material _material;
    private Texture2D _gradientTex;

    void Start() {
        _material = GetComponent<Renderer>().sharedMaterial;
        UpdateGradientTexture();
    }

    void Update() {
        // 更新Shader参数
        if(_material){
            _material.SetFloat("_Speed", speed);
            _material.SetFloat("_Density", density);
            _material.SetFloat("_TwinkleAmount", twinkleIntensity);
            _material.SetFloat("_TwinkleSpeed", twinkleSpeed);
        }
        
        // 每10帧更新渐变色
        if(Time.frameCount % 10 == 0){
            UpdateGradientTexture();
        }
    }

    void UpdateGradientTexture() {
        if(_gradientTex == null){
            _gradientTex = new Texture2D(128, 4, TextureFormat.RGBA32, false);
            _gradientTex.wrapMode = TextureWrapMode.Clamp;
        }

        // 根据预设生成渐变色
        Color[] pixels = new Color[128*4];
        for(int x=0; x<128; x++){
            for(int y=0; y<4; y++){
                float t = x/128f;
                Color baseColor = colorPreset.Evaluate(t);
                float noise = Mathf.PerlinNoise(x*0.1f, y*10f) * 0.3f;
                pixels[x + y*128] = baseColor + new Color(noise, noise, noise);
            }
        }
        
        _gradientTex.SetPixels(pixels);
        _gradientTex.Apply();
        _material.SetTexture("_ColorGradient", _gradientTex);
    }

#if UNITY_EDITOR
    void OnValidate() {
        UpdateGradientTexture();
    }
#endif
}