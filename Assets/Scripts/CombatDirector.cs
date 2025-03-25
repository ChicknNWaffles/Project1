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
    public GameObject FFStationaryPrefab;
    public GameObject FFHorizontalPrefab;
    public GameObject FLBackPrefab;
    public GameObject FLMovingPrefab;
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
    public int waveNumber = 0; // count the number of waves that have been spawned
    public float spawnTimer = 3; // seconds it waits until the next "wave" is spawned
    public bool HALT = false; // HALT the combat director
    public bool linkWithScore = false; // if this is true, then the score will affect the heat meter

    // private variables
    private float x;    private float y;
    private Vector3 spawnPoint; // a temporary variable that selects the centerpoint for a wave to spawn in
    private Vector3 spawnOffset; // a temporary variable that stores where individual enemies are spawned in relation to the spawnpoint
    private List<string> enabled = new List<string>(); // a list that will store the enabled enemy types


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!HALT) // the game should only spawn in enemies when the Combat Director is enabled. HALT stops the enemy spawns.
        {
            // check what enemies have spawns enabled
            checkEnabledEnemies();

            // make sure the Heat isn't above 3 or below 0
            if (heatLevel > 3) { heatLevel = 3; }
            if (heatLevel < 0) { heatLevel = 0; }

            spawnTimer -= Time.deltaTime;
            // wait for a timer to reach zero. When that timer is zero, spawn a valid enemy (or wave of enemies)
            // at a random coordinate
            if (spawnTimer < 0)
            {

                // choose which valid enemy to spawn

                int len = enabled.Count;
                // spawn the enemy with the correct spawn function
                switch (enabled[Random.Range(0, len)])
                {
                    case "cruiser":
                        SpawnCruiserWave();
                        break;
                    case "ffStationary":
                        SpawnFFStationaryWave();
                        break;
                    case "ffHorizontal":
                        SpawnFFHorizontalWave();
                        break;
                    case "flBack":
                        SpawnFLBackWave();
                        break;
                    case "flMoving":
                        SpawnFLMovingWave();
                        break;
                    case "station":
                        SpawnStationWave();
                        break;
                    case "asteroid":
                        SpawnAsteroidWave();
                        break;
                    default:
                        // don't spawn anything.
                        waveNumber -= 1;
                        break;
                }


                // set a new stall timer depending on the "Heat" of the game
                spawnTimer = 2 + (int)(heatLevel * 1.5);
                // increment the wave number
                waveNumber += 1;
            }
        }
        else { ; }
    }

    void SpawnAnEnemy(GameObject prefab, float x, float y)
    {
        spawnOffset.x = x; spawnOffset.y = y;
        GameObject temp = Instantiate(prefab);
        temp.transform.position = spawnOffset;
        spawnOffset.x = spawnOffset.x - 5f;
        temp.GetComponent<Enemy>().origin = spawnOffset;
    }

    /* ========================================================================================================================
     * Enemy switch cases
     * ======================================================================================================================== */

    // the cruiser spawns a large formation of stationary fighters, so it has the strangest spawn conditions.
    // At maximum Heat, spawns 3 cruisers that surround 6 stationary fighters
    void SpawnCruiserWave()
    {
        // pick a random x coordinate between 12 and 13
        spawnPoint.x = Random.Range(12f, 13f);
        // pick a random y coordinate between -2 and 2
        spawnPoint.y = Random.Range(-1f, 1f);

        // when a cruiser is spawned, it may spawn some stationary fighters next to it depending on the Heat level.
        if (heatLevel >= 3)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y + 1.5f);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y - 1.5f);

            SpawnAnEnemy(cruiserPrefab, spawnPoint.x + 0, spawnPoint.y + 3.5f);
            SpawnAnEnemy(cruiserPrefab, spawnPoint.x + 0, spawnPoint.y - 3.5f);
        }
        if (heatLevel >= 2)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y + 2);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y -2);
        }
        if (heatLevel >= 1)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y + 1);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y -1);
        }
        if (heatLevel >= 0)
        {
            // if heat is 0, spawn just the one figther
            SpawnAnEnemy(cruiserPrefab, spawnPoint.x, spawnPoint.y);
        }

    }

    // most of the normal fighters (formation fighters (stationary and horizontal), lone fighters (back and moving) ) will spawn
    // in generally the same way: a cluster formation of ships.
    // At maximum Heat, spawns a wall of stationary ships (with two in advance)
    void SpawnFFStationaryWave()
    {
        // pick a random x coordinate between 12 and 13
        spawnPoint.x = Random.Range(12f, 13f);
        // pick a random y coordinate between -4 and 4
        spawnPoint.y = Random.Range(-4f, 4f);

        spawnOffset.x = spawnPoint.x; spawnOffset.y = spawnPoint.y;
        // these fighters should sit still on the opposite side of the player.
        // As the Heat increases, so too does the pack size.
        if (heatLevel >= 3)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y + 2);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y -2);
        }
        if (heatLevel >= 2)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y + 1);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 1, spawnPoint.y -1);
        }
        if (heatLevel >= 1)
        {
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y + 1);
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x + 0, spawnPoint.y -1);
        }
        if (heatLevel >= 0)
        {
            // if Heat is 0, spawn just the one figther
            SpawnAnEnemy(FFStationaryPrefab, spawnPoint.x, spawnPoint.y);
        }

    }

    // At maximum Heat, spawn a V-shaped flock of ships.
    void SpawnFFHorizontalWave()
    {
        // pick a random x coordinate between 12 and 13
        spawnPoint.x = Random.Range(12f, 13f);
        // pick a random y coordinate between -4 and 4
        spawnPoint.y = Random.Range(-4f, 4f);

        if (heatLevel >= 3)
        {
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 3, spawnPoint.y + 3);
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 3, spawnPoint.y -3);
        }
        if (heatLevel >= 2)
        {
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 2, spawnPoint.y + 2);
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 2, spawnPoint.y -2);
        }
        if (heatLevel >= 1)
        {
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 1, spawnPoint.y + 1);
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x + 1, spawnPoint.y -1);
        }
        if (heatLevel >= 0)
        {
            // if Heat is 0, spawn just the one figther
            SpawnAnEnemy(FFHorizontalPrefab, spawnPoint.x, spawnPoint.y);
        }

    }

    // At maximum Heat, spawn a zig-zag of ships
    void SpawnFLBackWave()
    {
        // pick a random x coordinate between 12 and 13
        spawnPoint.x = Random.Range(12f, 13f);
        // pick a random y coordinate between -4 and 4
        spawnPoint.y = Random.Range(-4f, 4f);

        if (heatLevel >= 3)
        {
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 1, spawnPoint.y + 3);
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 1, spawnPoint.y -3);
        }
        if (heatLevel >= 2)
        {
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 0, spawnPoint.y + 2);
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 0, spawnPoint.y -2);
        }
        if (heatLevel >= 1)
        {
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 1, spawnPoint.y + 1);
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x + 1, spawnPoint.y -1);
        }
        if (heatLevel >= 0)
        {
            // if Heat is 0, spawn just the one figther
            SpawnAnEnemy(FLBackPrefab, spawnPoint.x, spawnPoint.y);
        }

    }

    // On maximum Heat, spawn a flat wall of ships
    void SpawnFLMovingWave()
    {
        // pick a random x coordinate between 12 and 13
        spawnPoint.x = Random.Range(12f, 13f);
        // pick a random y coordinate between -4 and 4
        spawnPoint.y = Random.Range(-4f, 4f);

        if (heatLevel >= 3){
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y + 3);
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y -3);
        }
        if(heatLevel >=2){
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y + 2);
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y -2);
        }
        if (heatLevel >= 1){
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y + 1);
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x + 0, spawnPoint.y -1);
        }
        if (heatLevel >= 0){
            // if Heat is 0, spawn just the one figther
            SpawnAnEnemy(FLMovingPrefab, spawnPoint.x, spawnPoint.y);
        }

    }
    
    // asteroids and stations should have similar spawns. Stations have fewer spawns per Heat, though, since they shoot.
    void SpawnStationWave()
    {

        // stations should spawn in random clusters that increase in size along with the Heat.
        // 4 stations are spawned at Heat 3, and one less is spawned as the Heat goes down.
        for (int i = heatLevel; i >= 0; i--)
        {
            // pick a random x coordinate between 12 and 15
            float x = Random.Range(12f, 15f);
            // pick a random y coordinate between -4.2 and 4.2
            float y = Random.Range(-4.2f, 4.2f);

            // spawn a station at that coordinate
            SpawnAnEnemy(stationPrefab, x, y);
        }

    }
    void SpawnAsteroidWave()
    {

        // asteroids should spawn in random clusters that increase in size along with the Heat.
        // 7 asteroids are spawned at Heat 3, and two less are spawned as the Heat goes down.
        for (int i = heatLevel * 3; i >= 0; i--)
        {
            // pick a random x coordinate between 12 and 15
            float x = Random.Range(12f, 16f);
            // pick a random y coordinate between -4.2 and 4.2
            float y = Random.Range(-4.2f, 4.2f);

            // spawn an asteroid at that coordinate
            SpawnAnEnemy(asteroidPrefab, x, y);
        }

    }

    void checkEnabledEnemies()
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

    void updateScore()
    {

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