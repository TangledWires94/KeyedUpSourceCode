using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public List<GameObject> robotObjects;
    public float movementSpeed;
    public float delayBetweenSpawning, spawningOffset;
    public Transform spawnTransform;
    public bool active = false;

    public void SetVariables(float delayBetweenSpawning, float spawningOffset, float movementSpeed)
    {
        this.delayBetweenSpawning = delayBetweenSpawning;
        this.spawningOffset = spawningOffset;
        this.movementSpeed = movementSpeed;
        spawnTransform = this.transform;
    }

    /*
    void Start()
    {
        spawnTransform = this.transform;
        StartSpawning();
    }
    */

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(spawningOffset);
        float lastSpawnTime = Time.time;
        float spawnX, spawnY;

        spawnX = spawnTransform.position.x;
        spawnY = spawnTransform.position.y;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, -0.2f);

        GameObject robotObject;
        int robotIndex = Random.Range(0, robotObjects.Count);
        robotObject = robotObjects[robotIndex];

        RobotController robot = Instantiate(robotObject, spawnTransform.position, spawnTransform.rotation).GetComponent<RobotController>();
        robot.SetVariables(movementSpeed);

        bool spawning = true;
        while (spawning)
        {
            if (Time.time > lastSpawnTime + delayBetweenSpawning)
            {
                lastSpawnTime = Time.time;
                spawnPosition = new Vector3(spawnX, spawnY, -0.2f);
                robotIndex = Random.Range(0, robotObjects.Count);
                robotObject = robotObjects[robotIndex];
                robot = Instantiate(robotObject, spawnTransform.position, spawnTransform.rotation).GetComponent<RobotController>();
                robot.SetVariables(movementSpeed);
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
