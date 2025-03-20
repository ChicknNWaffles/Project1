using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public float moveSpeed;
    private float canGoUp = 1.0f;
    private float canGoDown = 1.0f;
    private HealthSystem healthSystem;
    // Start is called before the first frame update
    void Start() {
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update() {
        var input = Game.Input.Standard;
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * input.MoveUp.ReadValue<float>() * canGoUp);
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime * input.MoveDown.ReadValue<float>() * canGoDown);

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
