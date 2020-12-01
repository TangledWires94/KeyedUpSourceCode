using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public float delayBetweenSpawning = 0;
    public GameObject objectToSpawn;
    public float minX, maxX, spawnY;

    public void SetSpawnerValues(float delayBetweenSpawning, GameObject objectToSpawn, float minX = -6.6f, float maxX = 6.05f, float spawnY = 6)
    {
        this.delayBetweenSpawning = delayBetweenSpawning;
        this.objectToSpawn = objectToSpawn;
        this.minX = minX;
        this.maxX = maxX;
        this.spawnY = spawnY;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnObjects());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnObjects()
    {
        float lastSpawnTime = Time.time;
        float spawnX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, -0.2f);
        Instantiate(objectToSpawn, spawnPosition, transform.rotation);
        bool spawning = true;
        while (spawning)
        {
            if (Time.time > lastSpawnTime + delayBetweenSpawning)
            {
                lastSpawnTime = Time.time;
                spawnX = Random.Range(minX, maxX);
                spawnPosition = new Vector3(spawnX, spawnY, -0.2f);
                Instantiate(objectToSpawn, spawnPosition, transform.rotation);
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
