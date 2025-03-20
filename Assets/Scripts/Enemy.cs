using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Public fields
    public int health = 3;

    public enum Type{
        MovingFighter,
        FormationFighterMoving,
        FormationFighterStationary,
        BackFighter,
        BattleCruiser,
        Station,
        Asteroid
    }
    public Type type;
    public float moveSpeed;
    public float turnLikelihood;

    // Private fields
    private Vector3 target;
    private float curTurnLikelihood;
    private Vector3 direction;
    private float pathTimer;

    // for moving enemies
    private Vector3[] path;
    private int at;

    // Refernce health system and set default enemy hp, CAN BE CHANGED FOR EACH ENEMY!
    private HealthSystem healthSystem;
    public int enemyMaxHealth = 50;

    // Start is called before the first frame update
    void Start(){
        curTurnLikelihood = turnLikelihood;
        direction = Vector3.left;
        pathTimer = 0.5f;
        target = transform.position;
        at = 0;
        path = new Vector3[6];
        
        // health handling
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null) {
            healthSystem.SetHealth(enemyMaxHealth);
        }
        else  {
            Debug.LogError("HealthSytem not set for this enemy");
        }
        


        if (type == Type.MovingFighter){

            // generate 5 coordinates
            for (var i = 0; i < 5; i++){
                // generate x
                var x = Random.Range(-5.5f, 8.0f);
                // generate y
                var y = Random.Range(-4.5f, 4.0f);

                // add coords to path
                var newCoord = new Vector3(x, y, 0.0f);
                path[i] = newCoord;
            }

            // put the start position as the last coord
            // to complete the loop
            path[5] = transform.position;
            
        }
    }

    // Update is called once per frame
    void Update(){
        pathTimer -= Time.deltaTime;
        if (pathTimer < 0) {
            pathfind();
            pathTimer = 0.5f;
        }
        // Moves enemy towards target chosen by pathfinder
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    // handles enemy pathfinding differently depending on enemy type
    void pathfind(){
        if(type == Type.MovingFighter){
            if (target == transform.position) {
                target = path[at];
                if (at == 5) {
                    at = 0;
                } else {
                    at++;
                }
            }
        }else if(type == Type.FormationFighterMoving || type == Type.Station || type == Type.Asteroid){
            target = transform.position + (Vector3.left * moveSpeed);
        }else if (type == Type.BackFighter){
            // if this is the first time setting the target, pick
            // a random direction. Else, decide whether or not
            // to turn around.
            if (direction == Vector3.left) {
                if (Random.value >= 0.5f) {
                    direction = Vector3.up;
                } else {
                    direction = Vector3.down;
                }
            }else { 
                if(Random.value <= curTurnLikelihood) {
                    if(direction == Vector3.up) {
                        direction = Vector3.down;
                    } else {
                        direction = Vector3.up;
                    }
                }
            }
            target = transform.position + (direction * moveSpeed);
        } else{
            target = transform.position;
        }
    }

    // Detects when the enemy collides with another object
    // or collider
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox")){
            curTurnLikelihood = 1.0f;
        } else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-High")){
            curTurnLikelihood = turnLikelihood + 7 * ((1.0f - turnLikelihood)/12);
        } else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-Low")){
            curTurnLikelihood = turnLikelihood + 3 * ((1.0f - turnLikelihood)/12);
        } else if (collision.transform.parent.gameObject.name.Equals("NormalTurn")){
            curTurnLikelihood = turnLikelihood;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy destroyed");
        Destroy(gameObject);
    }
}
