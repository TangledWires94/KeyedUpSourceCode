using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class InputManager : Manager<InputManager>
{
    [SerializeField]
    public List<KeyCode> universalKeysList = new List<KeyCode>();
    [SerializeField]
    public List<KeyCode> player1KeysList = new List<KeyCode>();
    [SerializeField]
    public List<KeyCode> player2KeysList = new List<KeyCode>();
    [SerializeField]
    public List<KeyCode> unusedKeysList = new List<KeyCode>();

    public KeyCode[] leftMove;
    public KeyCode[] rightMove;
    public KeyCode[] jump;
    public KeyCode[] action;

    public enum PlayerKeys { Left, Right, Jump, Action };
    public int numberOfPlayers = 1;
    public delegate void InputManagerEvent();
    //Player 1
    public event InputManagerEvent OnMoveLeft1;
    public event InputManagerEvent OnStopLeft1;
    public event InputManagerEvent OnMoveRight1;
    public event InputManagerEvent OnStopRight1;
    public event InputManagerEvent OnJump1;
    public event InputManagerEvent OnAction1;

    //Player 2
    public event InputManagerEvent OnMoveLeft2;
    public event InputManagerEvent OnStopLeft2;
    public event InputManagerEvent OnMoveRight2;
    public event InputManagerEvent OnStopRight2;
    public event InputManagerEvent OnJump2;
    public event InputManagerEvent OnAction2;

    void Start()
    {
        leftMove = new KeyCode[numberOfPlayers];
        rightMove = new KeyCode[numberOfPlayers];
        jump = new KeyCode[numberOfPlayers];
        action = new KeyCode[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            leftMove[i] = KeyCode.None;
            rightMove[i] = KeyCode.None;
            jump[i] = KeyCode.None;
            action[i] = KeyCode.None;
        }
    }

    void Update()
    {
        #region Player 1 Inputs
        if (Input.GetKey(leftMove[0]))
        {
            if(OnMoveLeft1 != null)
            {
                OnMoveLeft1.Invoke();
            }
        } else if (Input.GetKeyUp(leftMove[0]))
        {
            if (OnStopLeft1 != null)
            {
                OnStopLeft1.Invoke();
            }
        }
        if (Input.GetKey(rightMove[0]))
        {
            if (OnMoveRight1 != null)
            {
                OnMoveRight1.Invoke();
            }
        }
        else if (Input.GetKeyUp(rightMove[0]))
        {
            if (OnStopRight1 != null)
            {
                OnStopRight1.Invoke();
            }
        }
        if (Input.GetKeyDown(jump[0]))
        {
            if (OnJump1 != null)
            {
                OnJump1.Invoke();
            }
        }
        if (Input.GetKeyDown(action[0]))
        {
            if (OnAction1 != null)
            {
                OnAction1.Invoke();
            }
        }
        #endregion

        #region Player2 Inputs
        if(numberOfPlayers > 1)
        {
            if (Input.GetKey(leftMove[1]))
            {
                if (OnMoveLeft2 != null)
                {
                    OnMoveLeft2.Invoke();
                }
            }
            else if (Input.GetKeyUp(leftMove[1]))
            {
                if (OnStopLeft2 != null)
                {
                    OnStopLeft2.Invoke();
                }
            }
            if (Input.GetKey(rightMove[1]))
            {
                if (OnMoveRight2 != null)
                {
                    OnMoveRight2.Invoke();
                }
            }
            else if (Input.GetKeyUp(rightMove[1]))
            {
                if (OnStopRight2 != null)
                {
                    OnStopRight2.Invoke();
                }
            }
            if (Input.GetKeyDown(jump[1]))
            {
                if (OnJump2 != null)
                {
                    OnJump2.Invoke();
                }
            }
            if (Input.GetKeyDown(action[1]))
            {
                if (OnAction2 != null)
                {
                    OnAction2.Invoke();
                }
            }
        }
        #endregion
    }

    public void ChangeNumberOfPlayers(int newNumberOfPlayers)
    {
        numberOfPlayers = newNumberOfPlayers;
        //Create new temp arrays to hold current keys so that values aren't lost in size change
        KeyCode[] tempLeftArray = new KeyCode[leftMove.Length];
        KeyCode[] tempRightArray = new KeyCode[rightMove.Length];
        KeyCode[] tempJumpArray = new KeyCode[jump.Length];
        KeyCode[] tempActionArray = new KeyCode[action.Length];
        for (int i = 0; i < leftMove.Length; i++)
        {
            tempLeftArray[i] = leftMove[i];
            tempRightArray[i] = rightMove[i];
            tempJumpArray[i] = jump[i];
            tempActionArray[i] = action[i];
        }

        //Create new arrays for each input, add old values up to the previous dimension size, if new size is greater than old size add empty keys for extra players
        leftMove = new KeyCode[numberOfPlayers];
        rightMove = new KeyCode[numberOfPlayers];
        jump = new KeyCode[numberOfPlayers];
        action = new KeyCode[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i < tempLeftArray.Length)
            {
                leftMove[i] = tempLeftArray[i];
                rightMove[i] = tempRightArray[i];
                jump[i] = tempJumpArray[i];
                action[i] = tempActionArray[i];
            }
            else
            {
                leftMove[i] = KeyCode.None;
                rightMove[i] = KeyCode.None;
                jump[i] = KeyCode.None;
                action[i] = KeyCode.None;
            }
        }
    }

    public KeyCode GetPlayerInputKeyCode(PlayerKeys playerKey, int playerNumber)
    {
        KeyCode key;
        switch (playerKey)
        {
            case PlayerKeys.Left:
                key = leftMove[playerNumber - 1];
                break;
            case PlayerKeys.Right:
                key = leftMove[playerNumber - 1];
                break;
            case PlayerKeys.Jump:
                key = jump[playerNumber - 1];
                break;
            case PlayerKeys.Action:
                key = action[playerNumber - 1];
                break;
            default:
                key = KeyCode.None;
                break;
        }
        return key;
    }
    
    //Assign new keys to all controls, used at the start of a new game
    public void AssignNewPlayerKeys()
    {
        ClearAllPlayerKeys();
        for(int i = 0; i < numberOfPlayers; i++)
        {
            leftMove[i] = GetRandomUnusedKey(i);
            rightMove[i] = GetRandomUnusedKey(i);
            jump[i] = GetRandomUnusedKey(i);
            action[i] = GetRandomUnusedKey(i);
        }
    }

    public KeyCode RandomiseOnePlayerKey(PlayerKeys playerKey, int playerNumber)
    {
        ClearOnePlayerKey(playerKey, playerNumber - 1);
        KeyCode newKeyCode = GetRandomUnusedKey(playerNumber);
        switch (playerKey)
        {
            case PlayerKeys.Left:
                leftMove[playerNumber - 1] = newKeyCode;
                break;
            case PlayerKeys.Right:
                rightMove[playerNumber - 1] = newKeyCode;
                break;
            case PlayerKeys.Jump:
                jump[playerNumber - 1] = newKeyCode;
                break;
            case PlayerKeys.Action:
                action[playerNumber - 1] = newKeyCode;
                break;
            default:
                break;
        }
        UpdateAllUIKeys();
        return newKeyCode;
    }

    public void UpdateAllUIKeys()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Manager<UIManager>.Instance.UpdateKeyUI(PlayerKeys.Left, leftMove[i], i);
            Manager<UIManager>.Instance.UpdateKeyUI(PlayerKeys.Right, rightMove[i], i);
            Manager<UIManager>.Instance.UpdateKeyUI(PlayerKeys.Jump, jump[i], i);
            Manager<UIManager>.Instance.UpdateKeyUI(PlayerKeys.Action, action[i], i);
        }
    }

    //Add all wanted keys to the list of unused keys
    /*
    public List<KeyCode> InitialiseUnusedKeys(int[] keyCodeIndices)
    {
        List<KeyCode> newList = new List<KeyCode>();
        foreach(int index in keyCodeIndices)
        {
            newList.Add((KeyCode)index);
        }
        return newList;
    }
    */

    public List<KeyCode> InitialiseUnusedKeys()
    {
        List<KeyCode> newUnusedKeys = new List<KeyCode>();
        foreach (KeyCode key in universalKeysList)
        {
            if (!newUnusedKeys.Contains(key))
            {
                newUnusedKeys.Add(key);
            }
        }
        foreach (KeyCode key in player1KeysList)
        {
            if (!newUnusedKeys.Contains(key))
            {
                newUnusedKeys.Add(key);
            }
        }
        foreach (KeyCode key in player2KeysList)
        {
            if (!newUnusedKeys.Contains(key))
            {
                newUnusedKeys.Add(key);
            }
        }
        return newUnusedKeys;
    }

    //Clear all currently set keys an add them back to the unused list
    public void ClearAllPlayerKeys()
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            ClearOnePlayerKey(PlayerKeys.Action, i);
            ClearOnePlayerKey(PlayerKeys.Jump, i);
            ClearOnePlayerKey(PlayerKeys.Left, i);
            ClearOnePlayerKey(PlayerKeys.Right, i);
        }
    }

    public void ClearOnePlayerKey(PlayerKeys playerKey, int playerNumber)
    {
        switch (playerKey)
        {
            case PlayerKeys.Left:
                if (!unusedKeysList.Contains(leftMove[playerNumber]) && leftMove[playerNumber] != KeyCode.None)
                {
                    unusedKeysList.Add(leftMove[playerNumber]);
                }
                leftMove[playerNumber] = KeyCode.None;
                break;

            case PlayerKeys.Right:
                if (!unusedKeysList.Contains(rightMove[playerNumber]) && rightMove[playerNumber] != KeyCode.None)
                {
                    unusedKeysList.Add(rightMove[playerNumber]);
                }
                rightMove[playerNumber] = KeyCode.None;
                break;

            case PlayerKeys.Jump:
                if (!unusedKeysList.Contains(jump[playerNumber]) && jump[playerNumber] != KeyCode.None)
                {
                    unusedKeysList.Add(jump[playerNumber]);
                }
                jump[playerNumber] = KeyCode.None;
                break;

            case PlayerKeys.Action:
                if (!unusedKeysList.Contains(action[playerNumber]) && action[playerNumber] != KeyCode.None)
                {
                    unusedKeysList.Add(action[playerNumber]);
                }
                action[playerNumber] = KeyCode.None;
                break;

            default:
                break;
        }
    }

    //Selects a random key from the list of unused keys, removes it from the list and returns it
    KeyCode GetRandomUnusedKey(int playerNumber)
    {
        KeyCode newKey = KeyCode.None; //placeholder key
        List<KeyCode> playerKeysList = new List<KeyCode>();
        foreach(KeyCode key in universalKeysList)
        {
            playerKeysList.Add(key);
        }
        if(playerNumber == 0)
        {
            foreach(KeyCode key in player1KeysList)
            {
                playerKeysList.Add(key);
            }
        } else
        {
            foreach (KeyCode key in player2KeysList)
            {
                playerKeysList.Add(key);
            }
        }

        for(int i = playerKeysList.Count - 1; i >= 0; i--)
        {
            if (!unusedKeysList.Contains(playerKeysList[i]))
            {
                playerKeysList.Remove(playerKeysList[i]);
            }
        }
        if (playerKeysList.Count > 0)
        {
            int minIndex = 0;
            int randomIndex = Random.Range(minIndex, playerKeysList.Count);
            newKey = playerKeysList[randomIndex];
            unusedKeysList.RemoveAt(randomIndex);
        } else
        {
            newKey = KeyCode.None;
        }
        return newKey;
    }

    void SwapRandomKeys(int playerNumber, KeyCode[,] keyChoices, out int keyChanged1, out KeyCode newKey1, out int keyChanged2, out KeyCode newKey2)
    {
        int index1 = Random.Range(0, 4);
        keyChanged1 = index1;
        KeyCode key1 = keyChoices[playerNumber, index1];
        newKey1 = key1;

        int index2 = index1;
        while (index2 == index1)
        {
            index2 = Random.Range(0, 4);
        }
        keyChanged2 = index2;
        newKey2 = keyChoices[playerNumber, index2];


    }
}