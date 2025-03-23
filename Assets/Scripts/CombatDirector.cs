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
    private List<string> enabled = new List<string>(); // a list that will store the enabled enemy types

    // Start is called before the first frame update
    void Start()
    {
        // for each enabled enemy type, add the type to the list, so it can be randomly selected from.
        if (cruiser) { enabled.Add("cruiser"); }
        if (fighterFormationStationary) { enabled.Add("ffStationary"); }
        if (fighterFormationHorizontal) { enabled.Add("ffHorizontal"); }
        if (fighterLoneBack) { enabled.Add("flBack"); }
        if (fighterLoneMoving) { enabled.Add("flMoving"); }
        if (station) { enabled.Add("station"); }
        if (asteroid) { enabled.Add("asteroid"); }
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the heat isn't above 3 or below 0
        if (heatLevel > 3) { heatLevel = 3; }
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

            int len = enabled.Count;
            // spawn the enemy with the correct spawn function
            switch (enabled[Random.Range(0,len)])
            {
                case "cruiser":
                    SpawnCruiser();
                    break;
                case "ffStationary":
                    // code block
                    break;
                case "ffHorizontal":
                    SpawnFormationHorizontal();
                    break;
                case "flBack":
                    // code block
                    break;
                case "flMoving":
                    // code block
                    break;
                case "station":
                    // code block
                    break;
                case "asteroid":
                    SpawnAsteroid();
                    break;
                default:
                    // don't spawn anything.
                    break;
            }


            // set a new stall timer depending on the "weight" of the enemies spawned, and the "heat" of the game
            spawnTimer = 3 + heatLevel;
        }
        

    }

    void SpawnCruiser()
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
                GameObject temp = Instantiate(cruiserPrefab);
                temp.transform.position = spawnPoint;
                break;
        }

    }

    // asteroids and stations should have similar spawns
    void SpawnAsteroid()
    {

        // asteroids should spawn in random clusters that increase in size along with the heat.
        // 7 asteroids are spawned at heat 3, and two less are spawned as the heat goes down.
        for(int i = heatLevel*2; i>=0; i--)
        {
            // pick a random x coordinate between 12 and 15
            spawnPoint.x = Random.Range(12f, 15f);
            // pick a random y coordinate between -4.2 and 4.2
            spawnPoint.y = Random.Range(-4.2f, 4.2f);

            // spawn an asteroid at that coordinate
            GameObject temp = Instantiate(asteroidPrefab);
            temp.transform.position = spawnPoint;
        }

    }

    // most of the normal fighters (formation fighters (stationary and horizontal), lone fighters (back and moving) ) will spawn
    // in generally the same way: a cluster formation of ships.
    void SpawnFormationHorizontal()
    {
        // these fighters should move in groups, horizontally across the screen. As the heat increases, so too does the pack size.
        switch (heatLevel)
        {
            // this switch case does use fallthrough, so each heat level builds on the last one.
            case 3:
                // on hard heat, spawn two more figthers offset even more (back and to the sides, making a zig-zag)
                break;
            case 2:
                // on medium heat, spawn two more figthers offset more (on level with the single middle fighter, making a W)
                break;
            case 1:
                // on easy heat, spawn two more figthers offset a bit (back and to the sides, making a V)
                break;
            default:
                // if heat is 0, spawn just the one figther
                GameObject temp = Instantiate(fighterFormationHorizontalPrefab);
                temp.transform.position = spawnPoint;
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