using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    public GameObject UIPrefab;
    public Text[] moveLeft, moveRight, jump, action;
    Text timer, instruction, livesRemaining, scoreText, continueText;

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), new LoadSceneMode());
        moveLeft = new Text[3];
        moveRight = new Text[3];
        jump = new Text[3];
        action = new Text[3];
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Change UI to show new key
    public void UpdateKeyUI(InputManager.PlayerKeys key, KeyCode keyCode, int playerNumber) //Modify when multiplayer UI set up
    {
        switch (key)
        {
            case InputManager.PlayerKeys.Left:
                moveLeft[playerNumber].text = "Left = " + keyCode.ToString();
                break;
            case InputManager.PlayerKeys.Right:
                moveRight[playerNumber].text = "Right = " + keyCode.ToString();
                break;
            case InputManager.PlayerKeys.Jump:
                jump[playerNumber].text = "Jump = " + keyCode.ToString();
                break;
            case InputManager.PlayerKeys.Action:
                action[playerNumber].text = "Action = " + keyCode.ToString();
                break;
            default:
                break;
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (GameObject.Find("Game UI") == null)
        {
            Instantiate(UIPrefab);
        }

        moveLeft = new Text[2];
        moveRight = new Text[2];
        jump = new Text[2];
        action = new Text[2];

        GameObject Player1Container = GameObject.Find("Player 1 Inputs");

        GameObject test = GameObject.Find("Left 1");

        moveLeft[0] = GameObject.Find("Left 1").GetComponent<Text>();
        UpdateKeyUI(InputManager.PlayerKeys.Left, Manager<InputManager>.Instance.leftMove[0], 0);

        moveRight[0] = Player1Container.transform.Find("Right 1").GetComponent<Text>();
        UpdateKeyUI(InputManager.PlayerKeys.Right, Manager<InputManager>.Instance.rightMove[0], 0);

        jump[0] = Player1Container.transform.Find("Jump 1").GetComponent<Text>();
        UpdateKeyUI(InputManager.PlayerKeys.Jump, Manager<InputManager>.Instance.jump[0], 0);

        action[0] = Player1Container.transform.Find("Action 1").GetComponent<Text>();
        UpdateKeyUI(InputManager.PlayerKeys.Action, Manager<InputManager>.Instance.action[0], 0);

        GameObject Player2Container = GameObject.Find("Player 2 Inputs");

        if (Manager<InputManager>.Instance.numberOfPlayers > 1)
        {
            moveLeft[1] = Player2Container.transform.Find("Left 2").GetComponent<Text>();
            UpdateKeyUI(InputManager.PlayerKeys.Left, Manager<InputManager>.Instance.leftMove[1], 1);

            moveRight[1] = Player2Container.transform.Find("Right 2").GetComponent<Text>();
            UpdateKeyUI(InputManager.PlayerKeys.Right, Manager<InputManager>.Instance.rightMove[1], 1);

            jump[1] = Player2Container.transform.Find("Jump 2").GetComponent<Text>();
            UpdateKeyUI(InputManager.PlayerKeys.Jump, Manager<InputManager>.Instance.jump[1], 1);

            action[1] = Player2Container.transform.Find("Action 2").GetComponent<Text>();
            UpdateKeyUI(InputManager.PlayerKeys.Action, Manager<InputManager>.Instance.action[1], 1);
        } else
        {
            Player2Container.SetActive(false);
        }

        timer = GameObject.Find("Timer").GetComponent<Text>();

        instruction = GameObject.Find("Instruction").GetComponent<Text>();
        switch (Manager<GameManager>.Instance.currentRoundtype)
        {
            case GameManager.Verbs.Catch:
                SetInstruction("Catch " + Manager<GameManager>.Instance.numberToCatch);
                break;
            case GameManager.Verbs.Dodge:
                SetInstruction("Dodge for " + (Manager<GameManager>.Instance.baseWaitTime + Manager<GameManager>.Instance.waitTimeOffset).ToString());
                break;
            case GameManager.Verbs.Collect:
                SetInstruction("Collect " + Manager<GameManager>.Instance.numberToCollect);
                break;
            case GameManager.Verbs.Shoot:
                SetInstruction("Shoot " + Manager<GameManager>.Instance.numberToShoot);
                break;
            case GameManager.Verbs.Jump:
                SetInstruction("Jump for " + (Manager<GameManager>.Instance.baseWaitTime + Manager<GameManager>.Instance.waitTimeOffset).ToString());
                break;
            case GameManager.Verbs.Default:
                SetInstruction("Move to flag and press Action to start");
                break;
            default:
                break;
        }

        continueText = GameObject.Find("Continue Text").GetComponent<Text>();
        continueText.text = "";

        livesRemaining = GameObject.Find("Lives Text").GetComponent<Text>();
        livesRemaining.text = "";

        scoreText = GameObject.Find("Score Text").GetComponent<Text>();
        scoreText.text = "";
    }

    public void SetInstruction(string instructionText)
    {
        instruction.text = instructionText;
    }

    public void UpdateTimeRemaining(int timeRemaining)
    {
        timer.text = timeRemaining.ToString();
    }

    public void ShowContinue()
    {
        continueText.text = "Press Action to continue";
    }

    public void SetLivesNumber(int lives)
    {
        livesRemaining.text = lives.ToString() + " x";
    }

    public void SetScoreNumber(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowRandomisedKey(InputManager.PlayerKeys playerKey, KeyCode newKeyCode)
    {
        scoreText.text = "";
        livesRemaining.text = "";

        string inputName;
        switch (playerKey)
        {
            case InputManager.PlayerKeys.Left:
                inputName = "Left";
                break;
            case InputManager.PlayerKeys.Right:
                inputName = "Right";
                break;
            case InputManager.PlayerKeys.Jump:
                inputName = "Jump";
                break;
            case InputManager.PlayerKeys.Action:
                inputName = "Action";
                break;
            default:
                inputName = "";
                break;
        }

        continueText.text = "Input Changed!\n" + inputName + " : " + newKeyCode.ToString() + "\nPress Action button to continue";
    }
}
