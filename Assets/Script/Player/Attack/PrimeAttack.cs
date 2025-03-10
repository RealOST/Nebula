using UnityEngine;

public class PrimeAttack : MonoBehaviour {
    public Transform firePoint;
    public BulletPool bulletPool;
    public float fireRate = 0.1f;
    private float _nextFireTime;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        var bullet = bulletPool.GetBullet();
        bullet.transform.position = firePoint.position;
        // bullet.transform.rotation = firePoint.rotation;
    }
}
