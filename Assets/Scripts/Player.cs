using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Audio Imports
    [SerializeField] private AudioClip shoot_sound;
    [SerializeField] private AudioClip charge_shoot_sound;
    [SerializeField] private AudioClip damage_sound;
    [SerializeField] private AudioClip death_sound;
    [SerializeField] private AudioClip game_over;
    [SerializeField] private AudioClip dodgeStartSound;

    //Movement
    public float moveSpeed;
    private float canGoUp = 1.0f;
    private float canGoDown = 1.0f;
    
    // Weapon properties
    public GameObject bulletPrefab;
    public GameObject chargeBulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.2f;
    private float fireRateCooldown = 1.0f;
    private float nextFireTime = 0f;
    
    // Charge shot properties
    private bool isCharging = false;
    private float chargeStartTime;
    public float maxChargeTime = 2.0f;
    public float minDamageMultiplier = 1.0f;
    public float maxDamageMultiplier = 3.0f;
    
    // Weapon selection
    private enum WeaponType { MainGun, ChargeGun }
    private WeaponType currentWeapon = WeaponType.MainGun;

    // DODGE UI
    [SerializeField] private UnityEngine.UI.Image dodgeCooldownImage; 
    private float dodgeCooldownTimer = 0f;

    // Dodge implementation
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 5f;
    private bool isDodging = false;
    private bool canDodge = true;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    // Health implementation
    private HealthSystem healthSystem;

    // Keeps track of kills for win condition
    public int kills = 0;

    // Start is called before the first frame update
   
    void Start()
    {
        // Initialize firePoint if not assigned in inspector
        if (firePoint == null)
        {
            firePoint = transform;
        }
        healthSystem = GetComponent<HealthSystem>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        if (healthSystem == null) Debug.LogError("HealthSystem is missing on Player!");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer is missing on Player!");
        if (playerCollider == null) Debug.LogError("Collider2D is missing on Player!");

    }
    
    // Update is called once per frame
    void Update()
    {
        var input = Game.Input.Standard;
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * input.MoveUp.ReadValue<float>() * canGoUp);
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime * input.MoveDown.ReadValue<float>() * canGoDown);

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currentWeapon = WeaponType.MainGun;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            currentWeapon = WeaponType.ChargeGun;
        }

        // cooldown timer implementation
        if (!canDodge) {
            dodgeCooldownTimer -= Time.deltaTime;
            dodgeCooldownImage.fillAmount = 1 - (dodgeCooldownTimer / dodgeCooldown);
        if (dodgeCooldownTimer <= 0) {
            canDodge = true;
            dodgeCooldownTimer = 0;
            dodgeCooldownImage.fillAmount = 1f; // cooldown is off
        }
    }


        
        // handles dodge ofc
        HandleDodge();
        HandleShooting();
    }
    
    void HandleShooting()
    {
        switch (currentWeapon)
        {
            case WeaponType.MainGun:
                HandleMainGun();
                break;
            case WeaponType.ChargeGun:
                HandleChargeGun();
                break;
        }
    }
    
    void HandleMainGun()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            ShootMainGun();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void HandleChargeGun()
    {
        // Start charging
        if (Input.GetKeyDown(KeyCode.Space) && !isCharging && Time.time >= nextFireTime)
        {
            StartCharging();
        }
        
        // Release charge shot
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            ReleaseChargeShot();
            
            // Add cooldown after firing
            nextFireTime = Time.time + fireRateCooldown;
        }
    }
    
    void ShootMainGun()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            SoundFXManager.Instance.PlaySoundClip(shoot_sound, transform, 0.4f, 1f);
            rb.velocity = firePoint.right * bulletSpeed;
        }
        
        // Set bullet damage if it has a Bullet component
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.damage = 1;
        }
    }
    
    void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        Debug.Log("Started charging");
    }
    
    void ReleaseChargeShot()
    {
        float chargeTime = Mathf.Clamp(Time.time - chargeStartTime, 0, maxChargeTime);
        float chargePercent = chargeTime / maxChargeTime;
        float damageMultiplier = Mathf.Lerp(minDamageMultiplier, maxDamageMultiplier, chargePercent);
        
        GameObject bullet = Instantiate(chargeBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            SoundFXManager.Instance.PlaySoundClip(charge_shoot_sound, transform, 0.6f, 1f);
            rb.velocity = firePoint.right * bulletSpeed;
        }
        
        // Set bullet damage if it has a Bullet component
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.damage = Mathf.RoundToInt(damageMultiplier);
        }
        
        // Apply visual effects based on charge level
        ApplyChargeEffects(bullet, chargePercent);
        
        isCharging = false;
        Debug.Log("Released charge shot with damage multiplier: " + damageMultiplier);
    }
    
    void ApplyChargeEffects(GameObject bullet, float chargePercent)
    {
        // Scale the bullet based on charge level
        float scaleMultiplier = Mathf.Lerp(1.0f, 2.0f, chargePercent);
        bullet.transform.localScale *= scaleMultiplier;
    }
    
    // dodge handling for left shift
    void HandleDodge() {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge) {
            Debug.Log("Dodge triggered");
            StartCoroutine(Dodge());
        }
    }
    
    IEnumerator Dodge() {
        if (!canDodge) yield break;

        isDodging = true;
        canDodge = false;
        dodgeCooldownTimer = dodgeCooldown;
        
        // play dodge sound
        SoundFXManager.Instance.PlaySoundClip(dodgeStartSound, transform, 0.8f, 1f);

        // Disable collisions and turn invincible
        playerCollider.enabled = false;
        healthSystem.SetInvincible(true);

        // Flicker the ship throughout the invincibility duration
        float flickerTime = 0f;
        while (flickerTime < dodgeDuration) {
            // half transparent red
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); 
            yield return new WaitForSeconds(dodgeDuration / 10);
            // half transparent purple
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
            yield return new WaitForSeconds(dodgeDuration / 10);

            flickerTime += dodgeDuration / 10;
        }

        // Re enable collision
        isDodging = false;
        playerCollider.enabled = true;
        healthSystem.SetInvincible(false);
        spriteRenderer.color = Color.white;

        // Cooldown implementation
        yield return new WaitForSeconds(dodgeCooldown);

        // enable dodging when cooldown over
        canDodge = true;
        dodgeCooldownTimer = 0f;
        dodgeCooldownImage.fillAmount = 1f;
    }

    // Damage and healing implementation, will be referenced whenever hit or healed
    public void TakeDamage(int damage) {
        SoundFXManager.Instance.PlaySoundClip(damage_sound, transform, 0.6f, 1f);
        healthSystem.LoseHealth(damage);
        ScoreSystem.Instance.MultReset();
        if (healthSystem.GetHealth() <= 0) {
            Die();
        }
    }

    // KILLS PLAYER
    private void Die() {
        Debug.Log("Player destroyed");
        SoundFXManager.Instance.PlaySoundClip(death_sound, transform, 1f, 1f);
        SoundFXManager.Instance.PlaySoundClip(game_over, transform, 1f, 1f);
        Destroy(gameObject);
        Game.Instance.GameOver();
    }

    public void HealPlayer(int amount) {
        healthSystem.Heal(amount);
    }

    // detects if the player is going offscreen
    // and stops them
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name.Equals("Top") || collision.name.Equals("Bottom")){
            if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox"))
            {
                if (collision.name.Equals("Top"))
                {
                    print("going offscreen");
                    canGoUp = 0.0f;
                }
                else
                {
                    print("going offscreen");
                    canGoDown = 0.0f;
                }
            }
        }
    }
    
    // detects when player is no longer
    // about to go offscreen and re-enables
    // movement in that direction
    void OnTriggerExit2D(Collider2D collision){
        if (collision.name.Equals("Top") || collision.name.Equals("Bottom")){
            if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox"))
            {
                canGoUp = 1.0f;
                canGoDown = 1.0f;
            }
        }
    }
}