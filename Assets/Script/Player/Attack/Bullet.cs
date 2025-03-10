using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    private float _speed = 15f;
    private float _maxLifetime = 3f;
    private float _timer;

    public void SetPool(IObjectPool<GameObject> pool) => _pool = pool;

    private void OnEnable() => _timer = 0f;

    private void Update()
    {
        // 使用Translate实现非物理移动（更高效）
        transform.Translate(Vector3.right * _speed * Time.deltaTime, Space.World);
        
        // 超时自动回收
        _timer += Time.deltaTime;
        if (_timer >= _maxLifetime)
            _pool.Release(this.gameObject);
    }

    // 碰撞检测（可选，需添加Collider组件）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            _pool.Release(gameObject);
    }
}
