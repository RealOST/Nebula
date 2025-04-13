using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 泛型约束：T必须是UnityEngine.Component派生类型
public class Singleton<T> : MonoBehaviour where T : Component {
    // 静态只读实例引用，私有化赋值操作
    public static T Instance { get; private set; }

    // 虚拟方法允许派生类扩展Awake逻辑
    protected virtual void Awake() {
        Instance = this as T;
    }
}