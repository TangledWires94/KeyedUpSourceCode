using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    Transform robotTransform;
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        robotTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        robotTransform.Translate(Vector2.right * movementSpeed * Time.timeScale);
    }

    public void SetVariables(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }
}
