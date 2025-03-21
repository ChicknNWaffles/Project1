using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDirector : MonoBehaviour
{
    // the enemy type should be allowed to spawn when each type is set to "true"
    public bool cruiser;
    public bool fighterFormationStationary;
    public bool fighterFormationHorizontal;
    public bool fighterLoneBack;
    public bool fighterLoneMoving;
    public bool station;
    public bool asteroid;

    // prefabs
    public GameObject cruiserPrefab;
    public GameObject fighterFormationStationaryPrefab;
    public GameObject fighterFormationHorizontalPrefab;
    public GameObject fighterLoneBackPrefab;
    public GameObject fighterLoneMovingPrefab;
    public GameObject stationPrefab;
    public GameObject asteroidPrefab;

    /* depending on level difficulty, some enemies will spawn in more difficult patterns, or larger waves
     * 
     * 0 = default, no special spawns.
     * 1 = easy (only a couple of special spawn characteristics)
     * 2 = moderate (some special spawn characteristics)
     * 3 = hard (most enemies have special spawn characteristics)
     */
    public int heatLevel = 0;

    // private variables
    private float spawnTimer = 3; // seconds it waits until the next "wave" is spawned
    private float x;    private float y;
    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the heat isn't above 3 or below 0
        if(heatLevel > 3) { heatLevel = 3; }
        if (heatLevel < 0) { heatLevel = 0; }

        spawnTimer -= Time.deltaTime;
        // wait for a timer to reach zero. When that timer is zero, spawn a valid enemy (or wave of enemies)
        // at a random coordinate
        if (spawnTimer < 0)
        {
            // pick a random x coordinate between 12 and 15
            spawnPoint.x = Random.Range(12f, 15f);
            // pick a random y coordinate between -4 and 4
            spawnPoint.y = Random.Range(-4f, 4f);

            // choose which valid enemy to spawn
            GameObject c1 = Instantiate(asteroidPrefab);
            c1.transform.position = spawnPoint;

            // set a new stall timer depending on the "weight" of the enemies spawned, and the "heat" of the game
            spawnTimer = 3 + heatLevel;
        }
        

    }

    void SpawnCruiser(float x = 12, float y = 0)
    {
        
        // when a cruiser is spawned, it may spawn some stationary fighters next to it depending on the heat level.
        switch (heatLevel)
        { 
            // this switch case does not use fallthrough, so each heat level can spawn a different wave of enemies
            case 3:
                // on hard heat, spawn two cruisers and six fighters
                break;
            case 2:
                // on medium heat, spawn one cruiser and four fighters
                break;
            case 1:
                // on easy heat, spawn one cruiser and two figthers
                break;
            default:
                // if heat is 0, spawn just the one cruiser
                //GameObject cTemp = Instantiate(cruiserPrefab, transform.position, transform.rotate);
                break;
        }

    }


    void SpawnAsteroid()
    {

        // asteroids should spawn in random clusters that increase in size along with the heat.
        // 7 asteroids are spawned at heat 3,
        for(int i = heatLevel*2; i>=0; i--)
        {
            // pick a random x coordinate between 12 and 15

            // pick a random y coordinate between -4 and 4

            // spawn an asteroid at that coordinate
            //GameObject a1 = Instantiate(asteroidPrefab, transform.position, transform.rotate);
        }

    }

    void SpawnFormationHorizontal(float x = 12, float y = 0)
    {

        // these fighters should move in groups, horizontally across the screen. As the heat increases, so too does the pack size.
        switch (heatLevel)
        {
            // this switch case does use fallthrough, so each heat level builds on the last one.
            case 3:
                // on hard heat, spawn two more figthers offset even more
                break;
            case 2:
                // on medium heat, spawn two more figthers offset more
                break;
            case 1:
                // on easy heat, spawn two more figthers offset a bit
                break;
            default:
                // if heat is 0, spawn just the one figthers
                //GameObject f1 = Instantiate(fighterFormationHorizontalPrefab, transform.position, transform.rotate);
                break;
        }

    }
}

/*
 * ########## Enemy types and intended spawn behavior: ##########
 * 
 * Battle Cruiser and Formation Fighter (stationary)
 * - come in from the right side of the screen, and stay still in that location
 * 
 * Lone Fighter (back)
 * - come in from the right side of the screen, moves up and down on the opposite side of the player
 * 
 * Lone Fighter (moving)
 * - come in from the right side of the screen, follows a flight pattern of five random points
 * 
 * Station, Asteroid, Formation Fighter (moving)
 * come in from the right side of the screen, move in a straight line across the screen
 * 
 * 
 * Enemies should be spawned at a default of x=12, between y=4 and y=-4
 * 
 */