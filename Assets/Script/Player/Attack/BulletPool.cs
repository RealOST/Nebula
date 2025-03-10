using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int defaultCapacity = 20;
    public int maxSize = 100;

    private IObjectPool<GameObject> _pool;

    private void Awake()
    {
        // 初始化对象池
        _pool = new ObjectPool<GameObject>(
            CreatePooledBullet,    // 创建新子弹的回调
            OnTakeFromPool,       // 取出时的操作（激活、初始化）
            OnReturnedToPool,     // 放回时的操作（隐藏）
            OnDestroyPoolObject,  // 销毁时的操作
            true, defaultCapacity, maxSize
        );
    }

    private GameObject CreatePooledBullet()
    {
        var bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().SetPool(_pool);
        return bullet;
    }

    private void OnTakeFromPool(GameObject bullet) => bullet.SetActive(true);
    private void OnReturnedToPool(GameObject bullet) => bullet.SetActive(false);
    private void OnDestroyPoolObject(GameObject bullet) => Destroy(bullet);

    public GameObject GetBullet() => _pool.Get();
}