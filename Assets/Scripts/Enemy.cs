using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //Audio Imports
    [SerializeField] private AudioClip damage_sound;

    // Public fields
    public enum Type {
        MovingFighter,
        FormationFighterMoving,
        FormationFighterStationary,
        BackFighter,
        BattleCruiser,
        Station,
        Asteroid
    }
    public GameObject enemyBulletPrefab;
    public Type type;
    public float moveSpeed = 3;
    public float turnLikelihood;
    public float bulletSpeed = 10f;
    public Vector3 origin;


    // Private fields
    private Vector3 target;
    private bool readyToStart = false;
    private float curTurnLikelihood;
    private Vector3 direction;
    private float pathTimer;
    private float shootTimer;
    private int inBurst;

    // for moving enemies
    private Vector3[] path;
    private int at;

    // Refernce health system and set default enemy hp, CAN BE CHANGED FOR EACH ENEMY!
    private HealthSystem healthSystem;
    public int enemyMaxHealth = 3;
    private SpriteRenderer spriteRenderer;

    // Enemy score value that can be changed per enemy
    public int enemyScoreValue = 1;


    // Start is called before the first frame update
    void Start() {
        curTurnLikelihood = turnLikelihood;
        direction = Vector3.left;
        pathTimer = 0.5f;
        shootTimer = Random.Range(1.0f, 2.5f);
        inBurst = 0;
        target = transform.position;
        at = 0;

        // health handling
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) {
        Debug.LogWarning(gameObject.name + " has no SpriteRenderer");
        }

        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null) {
            healthSystem.SetHealth(enemyMaxHealth);
        }
        else {
            Debug.LogError("HealthSytem not set for this enemy");
        }

        // setting the enemy scores based on type
        switch (type) {
            case Type.MovingFighter:
                enemyMaxHealth = 3;
                enemyScoreValue = 5;
                break;
            case Type.FormationFighterMoving:
                enemyMaxHealth = 3;
                enemyScoreValue = 10;
                break;
            case Type.FormationFighterStationary:
                enemyMaxHealth = 3;
                enemyScoreValue = 8;
                break;
            case Type.BackFighter:
                enemyMaxHealth = 3;
                enemyScoreValue = 12;
                break;
            case Type.BattleCruiser:
                enemyMaxHealth = 8;
                enemyScoreValue = 20;
                break;
            case Type.Station:
                enemyMaxHealth = 12;
                enemyScoreValue = 25;
                break;
            case Type.Asteroid:
                enemyMaxHealth = 5;
                enemyScoreValue = 3;
                break;
            default:
                enemyMaxHealth = 3;
                enemyScoreValue = 1;
                break;
        }

        healthSystem.SetHealth(enemyMaxHealth);
    }

    // Update is called once per frame
    void Update() {
        if(transform.position == origin){
            if (path == null && type == Type.MovingFighter) {
                path = new Vector3[6];
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

            readyToStart = true;
        }else if (!readyToStart){
            target = origin;
        }


        pathTimer -= Time.deltaTime;
        if (pathTimer < 0 && readyToStart) {
            pathfind();
            pathTimer = 0.5f;
        }
        // Moves enemy towards target chosen by pathfinder
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // shooting
        if (readyToStart) {
            if (type != Type.Asteroid) {
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0) {
                    if (type == Type.Station) {
                        var playerPos = Game.Instance.playerObj.transform.position;
                        //var playerPos = transform.position;
                        var curPos = transform.position;
                        var direction = playerPos - curPos;
                        Shoot(direction);
                        shootTimer = Random.Range(1.0f, 2.5f);
                    } else if (type == Type.BattleCruiser) {
                        Shoot();
                        if (inBurst == 0) {
                            shootTimer = Random.Range(2.5f, 4.0f);
                            inBurst = 5;
                        } else {
                            shootTimer = 0.2f;
                            inBurst -= 1;
                        }
                    } else {
                        Shoot();
                        shootTimer = Random.Range(1.0f, 2.5f);
                    }
                }
            }
        }

        // Despawn if offscreen
        if(transform.position.x < -9.5) {
            print("Despawned enemy");
            Destroy(gameObject);
        }

    }

    public void originSetter(Vector3 newOrigin){
        origin = newOrigin;
    }

    // handles enemy pathfinding differently depending on enemy type
    void pathfind() {
        if (type == Type.MovingFighter) {
            if (target == transform.position) {
                target = path[at];
                if (at == 5) {
                    at = 0;
                } else {
                    at++;
                }
            }
        } else if (type == Type.FormationFighterMoving || type == Type.Station || type == Type.Asteroid) {
            target = transform.position + (Vector3.left * moveSpeed);
        } else if (type == Type.BackFighter) {
            // if this is the first time setting the target, pick
            // a random direction. Else, decide whether or not
            // to turn around.
            if (direction == Vector3.left) {
                if (Random.value >= 0.5f) {
                    direction = Vector3.up;
                } else {
                    direction = Vector3.down;
                }
            } else {
                if (Random.value <= curTurnLikelihood) {
                    if (direction == Vector3.up) {
                        direction = Vector3.down;
                    } else {
                        direction = Vector3.up;
                    }
                }
            }
            target = transform.position + (direction * moveSpeed);
        } else {
            target = transform.position;
        }
    }

    // Detects when the enemy collides with another object
    // or collider
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name.Equals("Top") || collision.name.Equals("Bottom")) {
            if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox")) {
                curTurnLikelihood = 1.0f;
            } else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-High")) {
                curTurnLikelihood = turnLikelihood + 7 * ((1.0f - turnLikelihood) / 12);
            } else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-Low")) {
                curTurnLikelihood = turnLikelihood + 3 * ((1.0f - turnLikelihood) / 12);
            } else if (collision.transform.parent.gameObject.name.Equals("NormalTurn")) {
                curTurnLikelihood = turnLikelihood;
            }
        }
    }
    
    // Links with the health system to take damage
    public void TakeDamage(int damage) {
        SoundFXManager.Instance.PlaySoundClip(damage_sound, transform, 1f);
        healthSystem.LoseHealth(damage);
        if (healthSystem.GetHealth() <= 0) {
            Die();
        }
    }
    
    // kill and increase the score by the enemies score value
    private void Die() {
        if (ScoreSystem.Instance != null) {
            ScoreSystem.Instance.ScoreIncrease(enemyScoreValue);
            ScoreSystem.Instance.MultIncrease();
        }
        Debug.Log("Enemy destroyed");
        Destroy(gameObject);
    }

    private void Shoot() {
        Shoot(Vector3.left);
    }

    private void Shoot(Vector3 direction) {
        var firepoint = transform;
        GameObject bullet = Instantiate(enemyBulletPrefab, firepoint.position, firepoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.velocity = direction.normalized * bulletSpeed;

        EnemyBullet bulletComponent = bullet.GetComponent<EnemyBullet>();
        // assign damage base don enemy type
        if (type == Type.BattleCruiser) {
            bulletComponent.damage = 3;
        }
        else {
            bulletComponent.damage = 1;
        }
        

    }
}
