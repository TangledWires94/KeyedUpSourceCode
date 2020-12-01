using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    bool jumping = false;
    bool onGoal = false;
    public bool active = false;
    public float moveSpeed, jumpForce;
    Transform playerTransform;
    Rigidbody2D rb;
    Animator anim;
    public int playerNumber = 1;

    public GameObject gunSprite;
    Transform projectileSpawn;
    bool useGun = false;
    public GameObject projectile;

    public delegate void CharacterEvent();
    public event CharacterEvent OnCaughtObject;
    public event CharacterEvent OnPlayerKilled;
    public delegate void PlayerControlEvent(PlayerControl player);
    public event PlayerControlEvent OnGoalReached;

    // Start is called before the first frame update
    void Start()
    {
        switch (playerNumber)
        {
            case 1:
                Manager<InputManager>.Instance.OnMoveLeft1 += MoveLeft;
                Manager<InputManager>.Instance.OnStopLeft1 += StopLeft;
                Manager<InputManager>.Instance.OnMoveRight1 += MoveRight;
                Manager<InputManager>.Instance.OnStopRight1 += StopRight;
                Manager<InputManager>.Instance.OnJump1 += Jump;
                Manager<InputManager>.Instance.OnAction1 += Action;
                break;

            case 2:
                Manager<InputManager>.Instance.OnMoveLeft2 += MoveLeft;
                Manager<InputManager>.Instance.OnStopLeft2 += StopLeft;
                Manager<InputManager>.Instance.OnMoveRight2 += MoveRight;
                Manager<InputManager>.Instance.OnStopRight2 += StopRight;
                Manager<InputManager>.Instance.OnJump2 += Jump;
                Manager<InputManager>.Instance.OnAction2 += Action;
                break;

            default:
                break;
        }

        playerTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gunSprite = transform.Find("Laser Gun").gameObject;
        if(SceneManager.GetActiveScene().name == "Shoot")
        {
            useGun = true;
            projectileSpawn = gunSprite.GetComponentInChildren<Transform>();
            gunSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            useGun = false;
            gunSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    void OnDisable()
    {
        switch (playerNumber)
        {
            case 1:
                Manager<InputManager>.Instance.OnMoveLeft1 -= MoveLeft;
                Manager<InputManager>.Instance.OnStopLeft1 -= StopLeft;
                Manager<InputManager>.Instance.OnMoveRight1 -= MoveRight;
                Manager<InputManager>.Instance.OnStopRight1 -= StopRight;
                Manager<InputManager>.Instance.OnJump1 -= Jump;
                Manager<InputManager>.Instance.OnAction1 -= Action;
                break;

            case 2:
                Manager<InputManager>.Instance.OnMoveLeft2 -= MoveLeft;
                Manager<InputManager>.Instance.OnStopLeft2 -= StopLeft;
                Manager<InputManager>.Instance.OnMoveRight2 -= MoveRight;
                Manager<InputManager>.Instance.OnStopRight2 -= StopRight;
                Manager<InputManager>.Instance.OnJump2 -= Jump;
                Manager<InputManager>.Instance.OnAction2 -= Action;
                break;

            default:
                break;
        }
    }

    void MoveLeft()
    {
        if (active)
        {
            playerTransform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
            anim.SetBool("MovingLeft", true);
        }
    }

    void StopLeft()
    {
        anim.SetBool("MovingLeft", false);
    }

    void MoveRight()
    {
        if (active)
        {
            playerTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
            anim.SetBool("MovingRight", true);
        }
    }

    void StopRight()
    {
        anim.SetBool("MovingRight", false);
    }

    void Jump()
    {
        if (!jumping && active)
        {
            Manager<SoundManager>.Instance.PlaySoundEffect(SoundManager.SoundEffect.Jump);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            jumping = true;
            anim.SetBool("Jumping", true);
        }
    }

    void Action()
    {
        if (onGoal && active)
        {
            OnGoalReached.Invoke(this);
        }

        if (useGun)
        {
            Manager<SoundManager>.Instance.PlaySoundEffect(SoundManager.SoundEffect.Shoot);
            Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            jumping = false;
            anim.SetBool("Jumping", false);
        } 
        else if(other.tag == "Goal")
        {
            onGoal = true;
        } 
        else if(other.tag == "Collectable")
        {
            Manager<SoundManager>.Instance.PlaySoundEffect(SoundManager.SoundEffect.Collect);
            Destroy(other.gameObject);
            OnCaughtObject.Invoke();
        } 
        else if(other.tag == "Enemy")
        {
            if (OnPlayerKilled != null)
            {
                Manager<SoundManager>.Instance.PlaySoundEffect(SoundManager.SoundEffect.PlayerDie);
                OnPlayerKilled.Invoke();
                Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Goal")
        {
            onGoal = false;
        }
    }

    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    public int GetPlayerNumber()
    {
        return playerNumber;
    }
}
