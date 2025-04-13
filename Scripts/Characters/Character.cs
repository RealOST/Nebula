using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Character : MonoBehaviour {
    [Header("---- DEATH ----")]
    [SerializeField] private GameObject deathVFX; // 死亡特效预制体引用

    [SerializeField] private AudioData[] deathSFX; // 死亡音效数据集

    [Header("---- HEALTH ----")]
    [SerializeField] protected float maxHealth; // 最大生命值  

    [SerializeField] private bool showOnHeadHealthBar = true;
    [SerializeField] private StatsBar onHeadHealthBar;

    protected float health; // 运行时动态生命值

    protected virtual void OnEnable() {
        health = maxHealth;

        if (showOnHeadHealthBar)
            ShowOnHeadHealthBar();
        else
            HideOnHeadHeadlthBar();
    }

    public void ShowOnHeadHealthBar() {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    public void HideOnHeadHeadlthBar() {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    // 伤害处理逻辑  
    public virtual void TakeDamage(float damage) {
        if (health == 0f) return; // 死亡状态拦截

        health -= damage;

        if (showOnHeadHealthBar) onHeadHealthBar.UpdateStats(health, maxHealth);

        if (health <= 0f) Die(); // 生命值耗尽触发死亡
    }

    // 死亡行为抽象层
    public virtual void Die() {
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX); // 音频系统接口调用
        PoolManager.Release(deathVFX, transform.position); // 对象池特效生成
        gameObject.SetActive(false); // 返回对象池可用状态
        Camera.main.DOShakePosition(0.2f, 0.1f, 10, 90, true);
    }

    public virtual void RestoreHealth(float value) {
        if (health == maxHealth) return;

        // health += value;
        // health = Mathf.Clamp(health, 0f, maxHealth);
        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar) onHeadHealthBar.UpdateStats(health, maxHealth);
    }

    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent) {
        while (health < maxHealth) {
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOverTime(WaitForSeconds waitTime, float percent) {
        while (health > 0) {
            yield return waitTime;

            TakeDamage(maxHealth * percent);
        }
    }
}