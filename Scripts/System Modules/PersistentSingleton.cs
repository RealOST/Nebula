using System;
using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        // 双重实例校验机制
        if (Instance == null)
            // 类型安全转换赋值
            Instance = this as T;
        else if (Instance != this)
            // 冗余实例销毁策略
            Destroy(gameObject);

        // 激活场景切换持久化
        DontDestroyOnLoad(gameObject);
    }
}