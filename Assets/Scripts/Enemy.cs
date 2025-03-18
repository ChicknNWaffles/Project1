using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
    private Vector3 target;
    public float turnLikelihood;
    private float curTurnLikelihood;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start(){
        var curTurnLikelihood = turnLikelihood;
        var direction = Vector3.left;
        InvokeRepeating("pathfind", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update(){
        // Moves enemy towards target chosen by pathfinder
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    // handles enemy pathfinding differently depending on enemy type
    void pathfind(){
        if(type == Type.MovingFighter){

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
        } 
    }

    // Detects when the enemy collides with another object
    // or collider
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.transform.parent.gameObject.name.Equals("OffscreenHitbox")){
            curTurnLikelihood = 1.0f;
        }else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-High")){
            curTurnLikelihood = turnLikelihood + 7 * ((1.0f - turnLikelihood)/12);
        }else if (collision.transform.parent.gameObject.name.Equals("TurnSoon-Low")){
            curTurnLikelihood = turnLikelihood + 3 * ((1.0f - turnLikelihood)/12);
        }else if (collision.transform.parent.gameObject.name.Equals("NormalTurn")){
            curTurnLikelihood = turnLikelihood;
        }
    }
}
