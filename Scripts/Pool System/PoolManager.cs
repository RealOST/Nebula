using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PoolManager : MonoBehaviour {
    [SerializeField] private Pool[] enemyPools;
    [SerializeField] private Pool[] playerProjectilePools;
    [SerializeField] private Pool[] enemyProjectilePools;
    [SerializeField] private Pool[] vFXPools;
    [SerializeField] private Pool[] lootItemPools;
    [SerializeField] private Pool[] meshTrailPools;

    private static Dictionary<GameObject, Pool> dictionary;

    private void Awake() {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(enemyPools);
        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(vFXPools);
        Initialize(lootItemPools);
        Initialize(meshTrailPools);
    }

#if UNITY_EDITOR
    private void OnDestroy() {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
        CheckPoolSize(lootItemPools);
        CheckPoolSize(meshTrailPools);
    }

    private void CheckPoolSize(Pool[] pools) {
        foreach (var pool in pools)
            if (pool.RuntimeSize > pool.Size)
                Debug.LogWarning(string.Format("Pool: {0} has a runtime size {1} than its initial size {2}",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size));
    }
#endif

    private void Initialize(Pool[] pools) {
        foreach (var pool in pools) {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab)) {
                Debug.LogError("Same prefab in multiple pools! Prefab: " + pool.Prefab.name);

                continue;
            }
#endif

            dictionary.Add(pool.Prefab, pool);

            var poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent.parent);
        }
    }

    /// <summary>
    /// <para>Return a specified <paramref name="prefab"/> gameObject in the pool.</para>
    /// <para>根据传入的<paramref name="prefab"/>参数，返回对象池中预备好的游戏对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameObject in the pool.</para>
    /// <para>对象池中预备好的游戏对象。</para>
    /// </returns>
    public static GameObject Release(GameObject prefab) {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab)) {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].PreparedObject();
    }

    /// <summary>
    /// <para>Return a specified <paramref name="prefab"/> gameObject in the pool.</para>
    /// <para>根据传入的<paramref name="prefab"/>参数，返回对象池中预备好的游戏对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>指定释放位置。</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameObject in the pool.</para>
    /// <para>对象池中预备好的游戏对象。</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector3 position) {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab)) {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].PreparedObject(position);
    }

    /// <summary>
    /// <para>Return a specified <paramref name="prefab"/> gameObject in the pool.</para>
    /// <para>根据传入的<paramref name="prefab"/>参数，返回对象池中预备好的游戏对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>指定释放位置。</para>
    /// </param>
    /// <param name="rotation">
    /// <para>Specified rotation.</para>
    /// <para>指定的旋转值。</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameObject in the pool.</para>
    /// <para>对象池中预备好的游戏对象。</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation) {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab)) {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].PreparedObject(position, rotation);
    }

    /// <summary>
    /// <para>Return a specified <paramref name="prefab"/> gameObject in the pool.</para>
    /// <para>根据传入的<paramref name="prefab"/>参数，返回对象池中预备好的游戏对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>指定释放位置。</para>
    /// </param>
    /// <param name="rotation">
    /// <para>Specified rotation.</para>
    /// <para>指定的旋转值。</para>
    /// </param>
    /// <param name="localScale">
    /// <para>Specified scale.</para>
    /// <para>指定的缩放值。</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameObject in the pool.</para>
    /// <para>对象池中预备好的游戏对象。</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale) {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab)) {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}