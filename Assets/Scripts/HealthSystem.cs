using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // Declares and sets the max hp to 100 and initializes the current health and health bar variables
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the current hp to max upon startup
        currentHealth = maxHealth;
        UpdateHealthbar();
    }

    // Function for when damage is taken, subtracts damage from current health
    public void LoseHealth(int damage) {
        currentHealth -= damage;
        
        // Detects if entity is dead, resets hp to 0 in case of overflowing damage and triggers death function
        if (currentHealth <= 0) {
            currentHealth = 0;
            Death();
        }
    }

    // Function for when damage is healed, used by powerups
    public void Heal(int healAmount) {

        // If healed over max, resets down to max health
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthbar();
    }

    // Fills the healthbar with whatever amount the hp is set to, changes color to red as it gets lower
    void UpdateHealthbar() {
        healthBar.UpdateHealth((float)currentHealth / maxHealth);  
    }

    void Death() {
        // placeholder for now, kinda just destroysd and murders the entity
        Destroy(gameObject);
    }

}