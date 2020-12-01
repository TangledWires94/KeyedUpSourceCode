using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyObject;
    public enum Direction { left, right, up, down};
    public Direction direction;
    public float movementSpeed;
    public float minX, maxX, minY, maxY, delayBetweenSpawning, spawningOffset, spawnRangeLeft, spawnRangeRight, spawnRangeUp, spawnRangeDown;
    Transform playerTransform;

    public void SetVariables(float delayBetweenSpawning, float spawningOffset, GameObject enemyObject, float movementSpeed)
    {
        this.delayBetweenSpawning = delayBetweenSpawning;
        this.spawningOffset = spawningOffset;
        this.enemyObject = enemyObject;
        this.movementSpeed = movementSpeed;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
        if (direction == Direction.up || direction == Direction.down)
        {
            spawnX = Random.Range(playerTransform.position.x + spawnRangeLeft, playerTransform.position.x + spawnRangeRight);
            spawnY = minY;
        }
        else
        {
            spawnX = minX;
            spawnY = Random.Range(playerTransform.position.y + spawnRangeDown, playerTransform.position.y + spawnRangeUp); ;
        }
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, -0.2f);
        EnemyController enemy = Instantiate(enemyObject, spawnPosition, transform.rotation).GetComponent<EnemyController>();
        Vector3 directionVector;
        switch (direction)
        {
            case Direction.left:
                directionVector = Vector3.left;
                break;
            case Direction.right:
                directionVector = Vector3.right;
                break;
            case Direction.up:
                directionVector = Vector3.up;
                break;
            case Direction.down:
                directionVector = Vector3.down;
                break;
            default:
                directionVector = Vector3.zero;
                break;
        }
        enemy.SetVariables(directionVector, movementSpeed);
        bool spawning = true;
        while (spawning)
        {
            if (Time.time > lastSpawnTime + delayBetweenSpawning)
            {
                lastSpawnTime = Time.time;
                if(direction == Direction.up || direction == Direction.down)
                {
                    spawnX = Random.Range(playerTransform.position.x + spawnRangeLeft, playerTransform.position.x + spawnRangeRight);
                    spawnY = minY;
                } else
                {
                    spawnX = minX;
                    spawnY = Random.Range(playerTransform.position.y + spawnRangeDown, playerTransform.position.y + spawnRangeUp); ;
                }
                spawnPosition = new Vector3(spawnX, spawnY, -0.2f);
                enemy = Instantiate(enemyObject, spawnPosition, transform.rotation).GetComponent<EnemyController>();
                enemy.SetVariables(directionVector, movementSpeed);
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
