using System.Collections;
using UnityEditor.Searcher;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character {
    #region FIELDS

    [SerializeField] private StatsBar_HUD statsBar_HUD;
    [SerializeField] private bool regenerateHealth = true;
    [SerializeField] private float healthRegenerateTime;
    [SerializeField] [Range(0f, 1f)] private float healthRegeneratePercent;

    [Header("---- INPUT ----")]
    [SerializeField] private PlayerInput input;

    [Header("---- MOVE ----")]
    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float accelerationTime = 3f;
    [SerializeField] private float decelerationTime = 3f;
    [SerializeField] private float moveRotationAngle = 50f;


    [Header("---- FIRE ----")]
    [SerializeField] private GameObject[] projectiles;

    [SerializeField] private GameObject projectileOverdrive;
    [SerializeField] private ParticleSystem muzzleVFX;
    [SerializeField] private Transform muzzleMiddle;
    [SerializeField] private Transform muzzleTop;
    [SerializeField] private Transform muzzleBottom;
    [SerializeField] private AudioData projectileLaunchSFX;
    [SerializeField] [Range(0, 4)] private int weaponPower = 0;
    [SerializeField] private float fireInterval = 0.2f;

    [Header("---- DODGE ----")]
    [SerializeField] private AudioData dodgeSFX;

    [SerializeField] [Range(0, 100)] private int dodgeEnergyCost = 25;
    [SerializeField] private float maxRoll = 720f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 dodgeScale = new(0.5f, 0.5f, 0.5f);

    [Header("---- OVERDRIVE ----")]
    [SerializeField] private int overdriveDodgeFactor = 2;

    [SerializeField] private float overdriveSpeedFactor = 1.2f;
    [SerializeField] private float overdriveFireFactor = 1.2f;

    private bool isDodge = false;
    private bool isOverdriving = false;

    private readonly float SlowMotionDuration = 0.4f;
    private readonly float InvincibleTime = 1f;
    private float paddingX;
    private float paddingY;
    private float currentRoll;
    private float dodgeDuration;
    private float t;

    private Vector2 moveDirection;
    private Vector2 previousVelocity;

    private Quaternion previousRotation;

    private WaitForSeconds waitForFireInterval => new(fireInterval);
    private WaitForSeconds waitForOverdriveFireInterval => new(fireInterval / overdriveFireFactor);
    private WaitForSeconds waitHealthRegenerateTime => new(healthRegenerateTime);
    private WaitForSeconds waitInvincibleTime => new(InvincibleTime);

    private Coroutine moveCoroutine;
    private Coroutine healthRegenerateCoroutine;

    private new Rigidbody2D rigidbody;

    private new Collider2D collider;

    private MissileSystem missile;

    private MeshTrail meshTrail;

    #endregion

    #region PROPERTIES

    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 4;

    #endregion

    #region UNITY EVENT FUNCTIONS

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();
        meshTrail = GetComponent<MeshTrail>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;
    }

    protected override void OnEnable() {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }


    private void OnDisable() {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }

    private void Start() {
        statsBar_HUD.Initialize(health, maxHealth);

        input.EnableGameplayInput();
    }

    #endregion

    #region HEALTH

    // 增强型受击处理方法
    public override void TakeDamage(float damage) {
        base.TakeDamage(damage); // 执行基础生命值计算
        // GetComponent<Shield>().showShield();
        PowerDown(); // 武器能量衰退策略
        statsBar_HUD.UpdateStats(health, maxHealth); // HUD组件实时更新
        TimeController.Instance.BulletTime(SlowMotionDuration); // 受击后时间减速
        Camera.main.DOShakePosition(0.1f, 0.1f, 10, 90, true); // 视觉震动反馈（使用DOTween插件）

        // 存活状态检测
        if (gameObject.activeSelf) {
            Move(moveDirection); // 受击惯性保持
            StartCoroutine(InvincibleCoroutine()); // 激活无敌状态

            // 生命恢复系统重启机制
            if (regenerateHealth) {
                if (healthRegenerateCoroutine != null) StopCoroutine(healthRegenerateCoroutine); // 中断现有恢复进程

                healthRegenerateCoroutine =
                    StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value) {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die() {
        GameManager.onGameOver.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    private IEnumerator InvincibleCoroutine() {
        collider.isTrigger = true;

        yield return waitInvincibleTime;

        collider.isTrigger = false;
    }

    #endregion

    #region MOVE

    // 运动控制入口方法
    private void Move(Vector2 moveInput) {
        // rigidbody.velocity = moveInput * moveSpeed;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine); // 中断现有运动进程

        moveDirection = moveInput.normalized; // 输入向量单位化处理
        // var moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);
        // moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, moveRotation));
        moveCoroutine = StartCoroutine(
            MoveCoroutine(
                accelerationTime,
                moveDirection * moveSpeed,
                Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right) // 俯仰角动态计算
            )
        );
        // StartCoroutine(nameof(MoveRangeLimitationCoroutine));
    }

    // 运动状态终止方法
    private void StopMove() {
        // rigidbody.velocity = Vector2.zero;
        moveDirection = Vector2.zero; // 目标速度归零
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(
            MoveCoroutine(
                decelerationTime,
                moveDirection,
                Quaternion.identity // 姿态复位
            )
        );
        // StopCoroutine(nameof(MoveRangeLimitationCoroutine));
    }

    // 运动插值协程核心
    private IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation) {
        t = 0f;
        previousVelocity = rigidbody.velocity; // 捕获初始速度状态
        previousRotation = transform.rotation; // 捕获初始旋转状态

        while (t < 1f) {
            t += Time.fixedDeltaTime / time; // 基于物理时间的标准化进度
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t); // 速度矢量插值
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t); // 旋转四元数插值

            yield return new WaitForFixedUpdate(); // 同步物理更新周期
        }
    }

    // private IEnumerator MoveRangeLimitationCoroutine() {
    //     while (true) {
    //         transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
    //
    //         yield return null;
    //     }
    // }

    // 屏幕边界约束系统
    private void Update() {
        // 通过视口管理系统进行运动范围约束
        transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
    }

    #endregion

    #region FIRE

    // 武器激活入口
    private void Fire() {
        muzzleVFX.Play(); // 激活枪口粒子特效
        StartCoroutine(nameof(FireCoroutine)); // 启动射击时序控制
    }

    // 武器停火控制
    private void StopFire() {
        muzzleVFX.Stop(); // 终止枪口视觉效果
        StopCoroutine(nameof(FireCoroutine)); // 中止射击进程
    }

    // 武器时序控制核心
    private IEnumerator FireCoroutine() {
        while (true) {
            // 状态驱动设计模式
            switch (weaponPower) {
                case 0: // 基础单发模式
                    ReleaseProjectile(0, muzzleMiddle);
                    break;
                case 1: // 双发模式
                    ReleaseProjectile(0, muzzleTop);
                    ReleaseProjectile(0, muzzleBottom);
                    break;
                case 2: // 三向散射配置
                    ReleaseProjectile(0, muzzleMiddle);
                    ReleaseProjectile(1, muzzleTop);
                    ReleaseProjectile(2, muzzleBottom);
                    break;
                case 3: // 五向弹幕阵列
                    ReleaseProjectile(0, muzzleMiddle);
                    ReleaseProjectile(1, muzzleTop);
                    ReleaseProjectile(2, muzzleBottom);
                    ReleaseProjectile(3, muzzleTop);
                    ReleaseProjectile(4, muzzleBottom);
                    break;
                case 4: // 全火力覆盖模式
                    ReleaseProjectile(0, muzzleMiddle);
                    ReleaseProjectile(1, muzzleTop);
                    ReleaseProjectile(2, muzzleBottom);
                    ReleaseProjectile(3, muzzleTop);
                    ReleaseProjectile(4, muzzleBottom);
                    ReleaseProjectile(5, muzzleTop);
                    ReleaseProjectile(6, muzzleBottom);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX); // 随机化射击音效

            // 根据状态动态切换射击节奏
            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
        }
    }

    // 弹道生成模块
    private void ReleaseProjectile(int projectileIndex, Transform muzzleTransform) {
        // 根据强化状态切换弹幕类型，并通过对象池系统实例化
        PoolManager.Release(isOverdriving ? projectileOverdrive : projectiles[projectileIndex],
            muzzleTransform.position);
    }

    #endregion

    #region DODGE

    // 闪避过程时序控制器
    private void Dodge() {
        if (isDodge || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return; // 状态与能量双重验证

        StartCoroutine(DodgeCoroutine()); // 启动复合闪避流程
    }

    // 闪避过程时序控制器
    private IEnumerator DodgeCoroutine() {
        isDodge = true; // 状态机锁定
        AudioManager.Instance.PlayRandomSFX(dodgeSFX); // 音频反馈

        // 能量管理系统交互
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        // 物理交互状态切换
        collider.isTrigger = true; // 进入相位碰撞模式

        // 运动参数初始化，让玩家沿着X轴旋转
        currentRoll = 0f;

        // 子弹时间特效（由TimeController实现）
        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);

        // 动态形体变换参数
        float rollDirection = rigidbody.velocity.y >= 0 ? 1 : -1; // 速度方向判定，记录翻滚方向

        // 运动插值核心循环
        while (currentRoll < maxRoll) {
            currentRoll += rollSpeed * Time.deltaTime; // 增量计算

            // 三维空间变换矩阵操作
            transform.rotation = Quaternion.AngleAxis(currentRoll * rollDirection, Vector3.right); // 轴向旋转
            // 二次贝塞尔曲线插值
            transform.localScale = BezierCurve.QuadraticPoint(
                Vector3.one, // 初始比例
                Vector3.one, // 最终比例
                dodgeScale, // 控制点
                currentRoll / maxRoll // 标准化进度
            );

            meshTrail.Spawn();

            yield return null; // 帧同步更新
        }

        // 状态复位
        collider.isTrigger = false;
        isDodge = false;
    }

    #endregion

    #region OVERDRIVE

    private void Overdrive() {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();
    }

    private void OverdriveOn() {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);
    }

    private void OverdriveOff() {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion

    #region MISSILE

    private void LaunchMissile() {
        missile.Launch(muzzleMiddle);
    }

    public void PickUpMissile() {
        missile.PickUp();
    }

    #endregion

    #region WEAPON POWER

    public void PowerUp() {
        weaponPower = Mathf.Min(weaponPower + 1, 4);
    }

    private void PowerDown() {
        //* 写法1
        // weaponPower--;
        // weaponPower = Mathf.Clamp(weaponPower, 0, 2);
        //* 写法2
        // weaponPower = Mathf.Max(weaponPower - 1, 0);
        //* 写法3
        // weaponPower = Mathf.Clamp(weaponPower, --weaponPower, 0);
        //* 写法4
        weaponPower = Mathf.Max(--weaponPower, 0);
    }

    #endregion
}