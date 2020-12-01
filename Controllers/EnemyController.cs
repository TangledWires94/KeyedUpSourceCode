using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Vector3 direction;
    public float movementSpeed;
    Transform enemyTransform;

    public void SetVariables(Vector3 direction, float movementSpeed)
    {
        this.direction = direction;
        this.movementSpeed = movementSpeed;
    }

    void Start()
    {
        enemyTransform = transform;
        StartCoroutine(MoveEnemy());
    }

    IEnumerator MoveEnemy()
    {
        while (true)
        {
            enemyTransform.Translate(direction * movementSpeed * Time.deltaTime);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
