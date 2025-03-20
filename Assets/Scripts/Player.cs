using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public float moveSpeed;
    private float canGoUp = 1.0f;
    private float canGoDown = 1.0f;
    private HealthSystem healthSystem;

    // Dodge implementation
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 3f;
    private bool isDodging = false;
    private bool canDodge = true;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    // Start is called before the first frame update
    void Start() {
        healthSystem = GetComponent<HealthSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        var input = Game.Input.Standard;
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * input.MoveUp.ReadValue<float>() * canGoUp);
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime * input.MoveDown.ReadValue<float>() * canGoDown);
        
        // handles dodge ofc
        HandleDodge();

    }

    void HandleDodge() {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge) {
            StartCoroutine(Dodge());
        }
    }
    
    IEnumerator Dodge() {
        isDodging = true;
        canDodge = false;

        // Disable collisions and turn invincible
        playerCollider.enabled = false;
        healthSystem.SetInvincible(true);

        // Flicker the ship throughout the invincibility duration
        for (int i = 0; i < 5; i++) {
            // half transparent
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); 
            yield return new WaitForSeconds(dodgeDuration / 10);
            // full color
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
            yield return new WaitForSeconds(dodgeDuration / 10);
        }

        // Re enable collision
        isDodging = false;
        playerCollider.enabled = true;
        healthSystem.SetInvincible(false);

        // Cooldown implementation
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
    // Damage and healing implementation, will be referenced whenever hit or healed
    public void TakeDamage(int damage) {
        healthSystem.LoseHealth(damage);
    }

    public void HealPlayer(int amount) {
        healthSystem.Heal(amount);
    }

    // detects if the player is going offscreen
    // and stops them
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox")){
            if(collision.name.Equals("Top")){
                print("gong offscreen");
                canGoUp = 0.0f;
            } else {
                print("gong offscreen");
                canGoDown = 0.0f;
            }
        }
    }

    // detects when player is no longer
    // about to go offscreen and re-enables
    // movement in that direction
    void OnTriggerExit2D(Collider2D collision) { 
        if(collision.transform.parent.gameObject.name.Equals("OffscreenHitbox")){
            canGoUp = 1.0f;
            canGoDown = 1.0f;
        }
    }
}
