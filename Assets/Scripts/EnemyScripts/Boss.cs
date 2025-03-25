using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private AudioClip damage_sound;
    [SerializeField] private AudioClip limb_destroyed_sound;
    [SerializeField] private AudioClip boss_death_sound;

    public int bossMaxHealth = 50;
    public int bossScoreValue = 100;
    public float moveSpeed = 1.5f;
    public float bulletSpeed = 8f;
    public GameObject enemyBulletPrefab;
    public Vector3 origin;

    [System.Serializable]
    public class BossLimb
    {
        public GameObject limbObject;
        public int limbHealth;
        public int limbScoreValue;
        public bool isDestroyed = false;
        public GameObject bulletSpawnPoint;
        public float fireRate = 2f;
        public float nextFireTime = 0f;
        public bool canFire = true;
    }

    public List<BossLimb> limbs = new List<BossLimb>();
    public bool requireAllLimbsDestroyed = false;

    public enum MovementPattern
    {
        Stationary,
        HorizontalMovement,
        VerticalMovement,
        CustomPath
    }
    public MovementPattern movementPattern;
    public Vector2 movementRange = new Vector2(3f, 2f);
    public Transform[] customPathPoints;
   
    private HealthSystem healthSystem;
    private SpriteRenderer spriteRenderer;
    private Vector3 target;
    private Vector3 startPosition;
    private bool initialPositionReached = false;
    private int currentPathIndex = 0;
    private float moveDirection = 1f;
    private float attackCooldown = 0f;
    private List<GameObject> activeProjectiles = new List<GameObject>();
    private float limbDestroyedTime = 0f;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.SetHealth(bossMaxHealth);
        }
        else
        {
            Debug.LogError("HealthSystem not set for boss enemy");
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning(gameObject.name + " has no SpriteRenderer");
        }

        target = origin;
        startPosition = transform.position;

        foreach (var limb in limbs)
        {
            HealthSystem limbHealthSystem = limb.limbObject.GetComponent<HealthSystem>();
            if (limbHealthSystem == null)
            {
                limbHealthSystem = limb.limbObject.AddComponent<HealthSystem>();
            }
            limbHealthSystem.SetHealth(limb.limbHealth);

            if (limb.limbObject.GetComponent<Collider2D>() == null)
            {
                limb.limbObject.AddComponent<BoxCollider2D>();
            }

            BossLimbComponent limbComponent = limb.limbObject.AddComponent<BossLimbComponent>();
            limbComponent.parentBoss = this;
            limbComponent.limbIndex = limbs.IndexOf(limb);
        }
    }

    void Update()
    {
        if (!initialPositionReached)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, origin) < 0.1f)
            {
                initialPositionReached = true;
            }
            return;
        }

        HandleMovement();

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            Attack();
        }

        foreach (var limb in limbs)
        {
            if (limb.isDestroyed || !limb.canFire) continue;

            if (Time.time >= limb.nextFireTime)
            {
                LimbAttack(limb);
                limb.nextFireTime = Time.time + limb.fireRate;
            }
        }

    }

    void HandleMovement()
    {
        switch (movementPattern)
        {
            case MovementPattern.Stationary:
                break;

            case MovementPattern.HorizontalMovement:
                Vector3 newPos = transform.position;
                newPos.x += moveSpeed * moveDirection * Time.deltaTime;

                if (newPos.x > origin.x + movementRange.x || newPos.x < origin.x - movementRange.x)
                {
                    moveDirection *= -1;
                }

                transform.position = newPos;
                break;

            case MovementPattern.VerticalMovement:
                Vector3 vertPos = transform.position;
                vertPos.y += moveSpeed * moveDirection * Time.deltaTime;

                if (vertPos.y > origin.y + movementRange.y || vertPos.y < origin.y - movementRange.y)
                {
                    moveDirection *= -1;
                }

                transform.position = vertPos;
                break;

            case MovementPattern.CustomPath:
                if (customPathPoints != null && customPathPoints.Length > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        customPathPoints[currentPathIndex].position,
                        moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, customPathPoints[currentPathIndex].position) < 0.1f)
                    {
                        currentPathIndex = (currentPathIndex + 1) % customPathPoints.Length;
                    }
                }
                break;
        }
    }

    void Attack()
    {
        Vector3 playerDirection = Game.Instance.playerObj.transform.position - transform.position;
        Shoot(playerDirection.normalized);
    }

    void LimbAttack(BossLimb limb)
    {
        if (limb.bulletSpawnPoint == null) return;

        Vector3 playerDirection = Game.Instance.playerObj.transform.position - limb.bulletSpawnPoint.transform.position;

        GameObject bullet = Instantiate(enemyBulletPrefab, limb.bulletSpawnPoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = playerDirection.normalized * bulletSpeed;

        EnemyBullet bulletComponent = bullet.GetComponent<EnemyBullet>();
        bulletComponent.damage = 1;

        activeProjectiles.Add(bullet);
    }

    void Shoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;

        EnemyBullet bulletComponent = bullet.GetComponent<EnemyBullet>();

        activeProjectiles.Add(bullet);
    }

    public void TakeDamage(int damage)
    {
        SoundFXManager.Instance.PlaySoundClip(damage_sound, transform, 0.4f, 1f);
        healthSystem.LoseHealth(damage);

        StartCoroutine(DamageFlash());

        if (healthSystem.GetHealth() <= 0)
        {
            Die();
        }
    }

    public void DamageLimb(int limbIndex, int damage)
    {
        if (limbIndex < 0 || limbIndex >= limbs.Count) return;

        BossLimb limb = limbs[limbIndex];
        if (limb.isDestroyed) return;

        HealthSystem limbHealthSystem = limb.limbObject.GetComponent<HealthSystem>();
        if (limbHealthSystem != null)
        {
            SoundFXManager.Instance.PlaySoundClip(damage_sound, limb.limbObject.transform, 0.4f, 1f);
            limbHealthSystem.LoseHealth(damage);

            StartCoroutine(LimbDamageFlash(limb.limbObject));

            if (limbHealthSystem.GetHealth() <= 0)
            {
                DestroyLimb(limbIndex);
            }
        }
    }

    void DestroyLimb(int limbIndex)
    {
        if (limbIndex < 0 || limbIndex >= limbs.Count) return;

        BossLimb limb = limbs[limbIndex];
        limb.isDestroyed = true;

        SoundFXManager.Instance.PlaySoundClip(limb_destroyed_sound, limb.limbObject.transform, 0.6f, 1f);

        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.ScoreIncrease(limb.limbScoreValue);
            ScoreSystem.Instance.MultIncrease();
        }

        SpriteRenderer limbSprite = limb.limbObject.GetComponent<SpriteRenderer>();
        if (limbSprite != null)
        {
            limbSprite.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }

        limbDestroyedTime = 3f;

        bool allLimbsDestroyed = true;
        foreach (var l in limbs)
        {
            if (!l.isDestroyed)
            {
                allLimbsDestroyed = false;
                break;
            }
        }

        if (allLimbsDestroyed && requireAllLimbsDestroyed)
        {
            Die();
        }
    }

    void Die()
    {
        SoundFXManager.Instance.PlaySoundClip(boss_death_sound, transform, 0.8f, 1f);

        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.ScoreIncrease(bossScoreValue);
            ScoreSystem.Instance.MultIncrease();
        }

        Game.Instance.GetComponent<Game>().kills += 1;

        foreach (var projectile in activeProjectiles)
        {
            if (projectile != null)
            {
                Destroy(projectile);
            }
        }

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        moveSpeed = 0;
        attackCooldown = 999f;

        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        // Add death particles here if created

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }

    IEnumerator LimbDamageFlash(GameObject limb)
    {
        SpriteRenderer limbSprite = limb.GetComponent<SpriteRenderer>();
        if (limbSprite != null)
        {
            limbSprite.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            limbSprite.color = Color.white;
        }
    }

}