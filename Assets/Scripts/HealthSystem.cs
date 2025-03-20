using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // Declares and sets the max hp to 100 and initializes the current health and health bar variables
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar; // optional, calls of update healthbar will only be called if a healthbar is assigned (player)
    bool isInvincible = false;

    // for changing color
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the current hp to max upon startup
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHealthbar();
    }

    // For setting custom hp values for enemies
    public void SetHealth(int maxHP) {
        maxHealth = maxHP;
        currentHealth = maxHealth;
    }

    // Function for when damage is taken, subtracts damage from current health
    public void LoseHealth(int damage) {
        // doesnt lose health if invincible
        if (isInvincible) {
            return;
        }
        currentHealth -= damage;
        UpdateHealthbar();

        StartCoroutine(FlashRed());
        // Detects if entity is dead, resets hp to 0 in case of overflowing damage and triggers death function
        if (currentHealth <= 0) {
            currentHealth = 0;
            Death();
        }


    }
    
    // co routine to make entity flash red when taking damage
    IEnumerator FlashRed() {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white; 
    }

    // Function for when damage is healed, used by powerups
    public void Heal(int healAmount) {

        // If healed over max, resets down to max health
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthbar();
    }

    // Fills the healthbar with whatever amount the hp is set to, changes color to red as it gets lower
    void UpdateHealthbar() {
        if (healthBar == null) {
            return;
        }
        healthBar.UpdateHealth((float)currentHealth / maxHealth);  
    }

    // For the dodge function
    public void SetInvincible(bool value) {
        isInvincible = value;
    }

    void Death() {
        // placeholder for now, kinda just destroysd and murders the entity
        Debug.Log("Enemy destroyed");
        Destroy(gameObject);
    }

}