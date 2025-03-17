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

    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        // Movement
        if(type == Type.MovingFighter){

        }else if(type == Type.FormationFighterMoving || type == Type.Station || type == Type.Asteroid){
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else if (type == Type.BackFighter){

        }
    }
}
