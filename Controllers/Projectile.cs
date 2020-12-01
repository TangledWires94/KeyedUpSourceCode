using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Transform projectileTransform;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        projectileTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        projectileTransform.Translate(Vector2.right * speed);
    }


}
