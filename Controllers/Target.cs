using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hit = false;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Projectile" && !hit)
        {
            Destroy(other.gameObject);
            anim.SetTrigger("Hit");
            hit = true;
            Manager<GameManager>.Instance.PlayerShotObject();
        }
    }

}
