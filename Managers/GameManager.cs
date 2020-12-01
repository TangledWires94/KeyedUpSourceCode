using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{
    [Header("Prefabs")]
    public GameObject InputManagerObject;
    public GameObject UIManagerObject;
    public GameObject[] PlayerPrefabs;
    public GameObject startRoundAnimator, winningAnimator, timeUpAnimator, playerDiedAnimator, gameOverAnimator;

    GameObject inputManager;
    GameObject uiManager;
    Button gameStartButton;
    Button setupButton;
    public GameObject keyboardObject;
    PlayerControl[] players = new PlayerControl[0];
    bool[] playersReady;
    bool subbedToAction = false;
    GameObject currentAnimation;
    GameObject mainUI, keyboardSetup;
    Button allUniversal, allUnassigned;

    //public enum Verbs { Catch, Dodge, Jump, Collect, Shoot, Carry, Reach, Switch, Press, Default};
    public enum Verbs { Catch, Dodge, Collect, Shoot, Jump, Default };
    [Header("Level Type")]
    public Verbs currentRoundtype = Verbs.Default;
    public List<Verbs> unusedRounds = new List<Verbs>();
    public Verbs lastRound = Verbs.Default; 

    [Header("Levels Completed")]
    public int levelsCompleted = 0;

    #region BaseStats
    [Header("Levels Until Input Changes")]
    public int levelsUntilChange = 5;
    public int baseLevelsUntilChange = 5;
    public int minLevelsUntilChange = 1;
    public int numberOfChanges = 0;
    [Header("Level Timer")]
    public int baseWaitTime = 15;
    public int waitTimeOffset = 0;
    public int baseWaitTimeOffset = 0;
    public int maxWaitTimeOffset = 10;
    public int waitTimeOffsetChange = 2;
    [Header("Number Of Lives")]
    public int livesRemaining = 3;
    public int baseLivesRemaining = 3;
    #endregion

    #region Catch
    [Header("Catch Objects")]
    public List<GameObject> catchSpawnObjects = new List<GameObject>();
    [Header("Spawn Rate")]
    public float delayBetweenSpawn = 2;
    public float baseDelayBetweenSpawn = 2;
    public float minDelayBetweenSpawn = 0.2f;
    public float delayBetweenSpawnChange = 0.2f;
    [Header("Number of Objects to Catch")]
    public int numberToCatch = 5;
    public int baseNumberToCatch = 5;
    public int maxNumberToCatch = 10;
    public int numberToCatchChange = 1;
    public int collectiblesCaught = 0;
    #endregion

    #region Dodge
    [Header("Dodge Objects")]
    public List<GameObject> dodgeSpawnObjects = new List<GameObject>();
    [Header("Enemy Spawn Rate")]
    public float delayBetweenEnemySpawn = 5;
    public float baseDelayBetweenEnemySpawn = 5;
    public float minDelayBetweenEnemySpawn = 0.5f;
    public float delayBetweenSpawnEnemyChange = 0.5f;
    [Header("Enemy Movement Speed")]
    public float enemyMovementSpeed = 5;
    public float baseEnemyMovementSpeed = 5;
    public float maxEnemyMovementSpeed = 10;
    public float enemyMovementSpeedChange = 1;
    #endregion

    #region Collect
    [Header("Collect Objects")]
    public List<GameObject> collectSpawnObjects = new List<GameObject>();
    [Header("Collect Platforms")]
    public List<GameObject> collectPlatformObjects = new List<GameObject>();
    [Header("Number of Objects to Catch")]
    public int numberToCollect = 6;
    public int baseNumberToCollect = 6;
    public int maxNumberToCollect = 12;
    public int numberToCollectChange = 2;
    //public int collectiblesCaught = 0;
    #endregion

    #region Shoot
    [Header("Shoot Objects")]
    public List<GameObject> shootSpawnObjects = new List<GameObject>();
    [Header("Number of Targets to Shoot")]
    public int numberToShoot = 5;
    public int baseNumberToShoot = 5;
    public int maxNumberToShoot = 10;
    public int numberToShootChange = 1;
    public int targetsShot = 0;
    #endregion

    #region Jump
    [Header("Number Of Jump Spawners")]
    public float numberOfJumpSpawners = 1;
    public float baseNumberOfJumpSpawners = 1;
    public float maxNumberOfJumpSpawners = 2;
    public float numberOfJumpSpawnersChange = 1;
    public float changesUntilSpawnIncrease = 1;
    [Header("Robot Spawn Rate")]
    public float delayBetweenRobotSpawn = 5;
    public float baseDelayBetweenRobotSpawn = 5;
    public float minDelayBetweenRobotSpawn = 0.5f;
    public float delayBetweenSpawnRobotChange = 0.5f;
    [Header("Robot Movement Speed")]
    public float robotMovementSpeed = 5;
    public float baseRobotMovementSpeed = 5;
    public float maxRobotMovementSpeed = 10;
    public float robotMovementSpeedChange = 1;
    #endregion

    public void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded(SceneManager.GetActiveScene(), new LoadSceneMode());
        Cursor.visible = true;
    }

    //Select the type of round then load the template scene for that type
    public void LoadNewRound()
    {
        if (subbedToAction)
        {
            Manager<InputManager>.Instance.OnAction1 -= ContinuePressed;
            Manager<InputManager>.Instance.OnAction2 -= ContinuePressed;
        }
        if(unusedRounds.Count <= 0)
        {
            for (int i = 0; i < Enum.GetNames(typeof(Verbs)).Length - 1; i++)
            {
                unusedRounds.Add((Verbs)i);
            }
        }
        int max = unusedRounds.Count;
        bool sameRound = true;
        int newIndex = 0;
        while (sameRound)
        {
            newIndex = UnityEngine.Random.Range(0, max);
            sameRound = unusedRounds[newIndex] == lastRound;
        }
        currentRoundtype = unusedRounds[newIndex];
        unusedRounds.RemoveAt(newIndex);
        lastRound = currentRoundtype;
        SceneManager.LoadScene((int)currentRoundtype + 3);
    }

    public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Time.timeScale = 1;

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0: //Main Menu
                mainUI = GameObject.Find("Main");
                setupButton = GameObject.Find("Setup").GetComponent<Button>();
                setupButton.onClick.AddListener(GoToSetup);
                ResetStats();
                Cursor.visible = true;
                break;

            case 1: //Game Setup
                keyboardSetup = GameObject.Find("Keyboard Setup UI");
                gameStartButton = GameObject.Find("Start Game").GetComponent<Button>();
                gameStartButton.onClick.AddListener(GoToTestingArea);
                keyboardObject = FindObjectOfType<KeyboardAssignment>().gameObject;
                allUniversal = GameObject.Find("Set All Universal").GetComponent<Button>();
                allUniversal.onClick.AddListener(keyboardObject.GetComponent<KeyboardAssignment>().SetAllUniversal);
                allUnassigned = GameObject.Find("Unassign All").GetComponent<Button>();
                allUnassigned.onClick.AddListener(keyboardObject.GetComponent<KeyboardAssignment>().UnassignAllKeys);
                Dropdown numberOfPlayers = GameObject.Find("Number Of Players Dropdown").GetComponent<Dropdown>();
                List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
                dropdownOptions.Add(new Dropdown.OptionData("1"));
                dropdownOptions.Add(new Dropdown.OptionData("2"));
                numberOfPlayers.ClearOptions();
                numberOfPlayers.AddOptions(dropdownOptions);
                numberOfPlayers.onValueChanged.AddListener(delegate
                {
                    Manager<InputManager>.Instance.ChangeNumberOfPlayers(numberOfPlayers.value + 1);
                });
                numberOfPlayers.onValueChanged.AddListener(delegate
                {
                    keyboardObject.GetComponent<KeyboardAssignment>().ChangeNumberOfPlayers(numberOfPlayers.value + 1);
                });
                //keyboardSetup.SetActive(false);
                //keyboardObject.SetActive(false);
                inputManager = Instantiate(InputManagerObject);
                break;

            case 2: //Testing Area
                Manager<SoundManager>.Instance.ChangeBackgroundMusic(SoundManager.BackgroundMusic.TestArea);
                ResetStats();
                SpawnPlayers();
                foreach(PlayerControl player in players)
                {
                    player.OnGoalReached += PlayerReady;
                }
                playersReady = new bool[Manager<InputManager>.Instance.numberOfPlayers];
                Cursor.visible = false;
                break;

            case 5: //Collect
                //Change Music
                if (Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.TestArea || Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.Typing)
                {
                    Manager<SoundManager>.Instance.ChangeBackgroundMusic(SoundManager.BackgroundMusic.Speed1);
                }

                //Spawn platforms object
                int platformIndex = UnityEngine.Random.Range(0, collectPlatformObjects.Count);
                GameObject collectPlatforms = Instantiate(collectPlatformObjects[platformIndex]);

                //Get list of collectable transforms
                List<Transform> collectableSpawnPoints = new List<Transform>();
                foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("ObjectSpawn"))
                {
                    collectableSpawnPoints.Add(spawnPoint.transform);
                }

                //Pick collectable
                int collectableIndex = UnityEngine.Random.Range(0, collectSpawnObjects.Count);
                GameObject collectableToSpawn = collectSpawnObjects[collectableIndex];

                //Spawn collectables
                for (int i = 0; i < numberToCollect; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, collectableSpawnPoints.Count);
                    Instantiate(collectableToSpawn, collectableSpawnPoints[randomIndex]);
                    collectableSpawnPoints.RemoveAt(randomIndex);
                }

                //Run normal level loaded code
                SpawnPlayers();
                Instantiate(startRoundAnimator);
                break;

            case 6:
                //Change Music
                if (Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.TestArea || Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.Typing)
                {
                    Manager<SoundManager>.Instance.ChangeBackgroundMusic(SoundManager.BackgroundMusic.Speed1);
                }
                //Get list of target transforms
                List<Transform> targetSpawnPoints = new List<Transform>();
                foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("ObjectSpawn"))
                {
                    targetSpawnPoints.Add(spawnPoint.transform);
                }

                //Pick collectable
                collectableIndex = UnityEngine.Random.Range(0, shootSpawnObjects.Count);
                GameObject targetToSpawn = shootSpawnObjects[collectableIndex];

                //Spawn targets
                for (int i = 0; i < numberToShoot; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, targetSpawnPoints.Count);
                    Instantiate(targetToSpawn, targetSpawnPoints[randomIndex]);
                    targetSpawnPoints.RemoveAt(randomIndex);
                }

                //Run normal level loaded code
                SpawnPlayers();
                Instantiate(startRoundAnimator);
                break;

            default:
                if (Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.TestArea || Manager<SoundManager>.Instance.currentMusic == SoundManager.BackgroundMusic.Typing)
                {
                    Manager<SoundManager>.Instance.ChangeBackgroundMusic(SoundManager.BackgroundMusic.Speed1);
                }
                SpawnPlayers();
                Instantiate(startRoundAnimator);
                break;
        }
    }

    public void RunGame()
    {
        switch (currentRoundtype)
        {
            case Verbs.Catch:
                ObjectSpawner catchSpawner = GameObject.Find("Object Spawner").GetComponent<ObjectSpawner>();
                int index = UnityEngine.Random.Range(0, catchSpawnObjects.Count);
                GameObject spawnObject = catchSpawnObjects[index];
                catchSpawner.SetSpawnerValues(delayBetweenSpawn, catchSpawnObjects[index]);
                catchSpawner.StartSpawning();
                collectiblesCaught = 0;
                foreach(PlayerControl player in players)
                {
                    player.OnCaughtObject += PlayerCaughtObject;
                    player.active = true;
                }
                StartCoroutine(TimerCountDown(baseWaitTime - waitTimeOffset));
                break;
            case Verbs.Dodge:
                float offset = 0;
                index = UnityEngine.Random.Range(0, dodgeSpawnObjects.Count);
                GameObject enemyObject = dodgeSpawnObjects[index];
                foreach (EnemySpawner dodgeSpawner in FindObjectsOfType<EnemySpawner>())
                {
                    offset += delayBetweenEnemySpawn * 0.25f;
                    dodgeSpawner.SetVariables(delayBetweenEnemySpawn, offset, enemyObject, enemyMovementSpeed);
                    dodgeSpawner.StartSpawning();
                }
                foreach (PlayerControl player in players)
                {
                    player.OnPlayerKilled += PlayerDied;
                    player.active = true;
                }
                StartCoroutine(TimerCountDown(baseWaitTime + waitTimeOffset));
                break;
            case Verbs.Collect:
                //Level layout needs to spawn before player does so so takes place in level loaded function
                collectiblesCaught = 0;
                foreach (PlayerControl player in players) 
                {
                    player.OnCaughtObject += PlayerCaughtObject;
                    player.active = true;
                }
                StartCoroutine(TimerCountDown(baseWaitTime - waitTimeOffset));
                break;
            case Verbs.Shoot:
                //Level layout needs to spawn before player does so so takes place in level loaded function
                targetsShot = 0;
                foreach (PlayerControl player in players)
                {
                    player.active = true;
                }
                StartCoroutine(TimerCountDown(baseWaitTime - waitTimeOffset));
                break;
            case Verbs.Jump:
                offset = 0;
                for(int i = 0; i < FindObjectsOfType<RobotSpawner>().Length; i++)
                {
                    if(i < numberOfJumpSpawners)
                    {
                        RobotSpawner robotSpawner = FindObjectsOfType<RobotSpawner>()[i];
                        offset += delayBetweenRobotSpawn * 0.5f;
                        robotSpawner.SetVariables(delayBetweenRobotSpawn, offset, robotMovementSpeed);
                        robotSpawner.StartSpawning();
                    }
                }
                foreach (PlayerControl player in players)
                {
                    player.OnPlayerKilled += PlayerDied;
                    player.active = true;
                }
                StartCoroutine(TimerCountDown(baseWaitTime + waitTimeOffset));
                break;
            case Verbs.Default:
                break;
            default:
                break;
        }
    }

    #region Level End Functions

    public void TimeUp()
    {
        StopAllCoroutines();
        Time.timeScale = 0;
        livesRemaining--;
        if(livesRemaining > 0)
        {
            if(currentAnimation != null)
            {
                Destroy(currentAnimation);
            }
            currentAnimation = Instantiate(timeUpAnimator);
        }
        else
        {
            if (currentAnimation != null)
            {
                Destroy(currentAnimation);
            }
            currentAnimation = Instantiate(gameOverAnimator);
        }
    }

    public void PlayerDied()
    {
        StopAllCoroutines();
        Time.timeScale = 0;
        livesRemaining--;
        if (livesRemaining > 0)
        {
            if (currentAnimation != null)
            {
                Destroy(currentAnimation);
            }
            Instantiate(playerDiedAnimator);
        }
        else
        {
            if (currentAnimation != null)
            {
                Destroy(currentAnimation);
            }
            Instantiate(gameOverAnimator);
        }
    }

    public void PlayerWin()
    {
        StopAllCoroutines();
        Time.timeScale = 0;
        levelsCompleted++;
        levelsUntilChange--;
        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
        }
        currentAnimation = Instantiate(winningAnimator);
    }

    #endregion

    #region GamePlay Functions

    void PlayerReady(PlayerControl readyPlayer)
    {
        Debug.LogFormat("Player {0} Ready Out Of {1}", readyPlayer.playerNumber, playersReady.Length);

        //Set the boolean for this player number to true
        playersReady[readyPlayer.playerNumber - 1] = true;
        
        //Check if all booleans are true, if they are load new level
        if(playersReady.All(x => x))
        {
            LoadNewRound();
        }
    }

    IEnumerator TimerCountDown(int time)
    {
        int timeRemaining = time;
        Manager<UIManager>.Instance.UpdateTimeRemaining(timeRemaining);
        while (timeRemaining > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            timeRemaining--;
            Manager<UIManager>.Instance.UpdateTimeRemaining(timeRemaining);
        }
        if(currentRoundtype == Verbs.Dodge || currentRoundtype == Verbs.Jump)
        {
            PlayerWin();
        }
        else
        {
            TimeUp();
        }
    }

    public void PlayerCaughtObject()
    {
        collectiblesCaught++;
        if(SceneManager.GetActiveScene().name == "Catch")
        {
            if (collectiblesCaught >= numberToCatch)
            {
                PlayerWin();
            }
        } else if (SceneManager.GetActiveScene().name == "Collect")
        {
            if (collectiblesCaught >= numberToCollect)
            {
                PlayerWin();
            }
        }
    }

    public void PlayerShotObject()
    {
        targetsShot++;
        if (SceneManager.GetActiveScene().name == "Shoot")
        {
            if (targetsShot >= numberToShoot)
            {
                PlayerWin();
            }
        }
    }

    public void SubscribeToContinue()
    {
        Manager<InputManager>.Instance.OnAction1 += ContinuePressed;
        Manager<InputManager>.Instance.OnAction2 += ContinuePressed;
        subbedToAction = true;
    }

    void IncreaseChaos()
    {
        //Randomise one of the players keys again
        RandomiseInput(1); //Add code to decide which player(s) input gets randomised if using multiplayer

        //Increment game variables
        numberOfChanges++;
        if (numberOfChanges <= 3)
        {
            int musicIndex = 2 + numberOfChanges;
            Manager<SoundManager>.Instance.ChangeBackgroundMusic((SoundManager.BackgroundMusic)musicIndex);
        }

        levelsUntilChange = baseLevelsUntilChange - numberOfChanges;
        if (levelsUntilChange <= 0)
        {
            levelsUntilChange = minLevelsUntilChange;
        }

        //Increment level specific variables
        if (waitTimeOffset < maxWaitTimeOffset)
        {
            waitTimeOffset += waitTimeOffsetChange;
            if (waitTimeOffset > maxWaitTimeOffset)
            {
                waitTimeOffset = maxWaitTimeOffset;
            }
        }

        if (delayBetweenSpawn > minDelayBetweenSpawn)
        {
            delayBetweenSpawn -= delayBetweenSpawnChange;
            if (delayBetweenSpawn < minDelayBetweenSpawn)
            {
                delayBetweenSpawn = minDelayBetweenSpawn;
            }
        }

        if (numberToCatch < maxNumberToCatch)
        {
            numberToCatch += numberToCatchChange;
            if (numberToCatch > maxNumberToCatch)
            {
                numberToCatch = maxNumberToCatch;
            }
        }

        if (delayBetweenEnemySpawn > minDelayBetweenEnemySpawn)
        {
            delayBetweenEnemySpawn -= delayBetweenSpawnEnemyChange;
            if (delayBetweenEnemySpawn < minDelayBetweenEnemySpawn)
            {
                delayBetweenEnemySpawn = minDelayBetweenEnemySpawn;
            }
        }

        if (enemyMovementSpeed < maxEnemyMovementSpeed)
        {
            enemyMovementSpeed += enemyMovementSpeedChange;
            if(enemyMovementSpeed > maxEnemyMovementSpeed)
            {
                enemyMovementSpeed = maxEnemyMovementSpeed;
            }
        }

        if (numberToCollect < maxNumberToCollect)
        {
            numberToCollect += numberToCollectChange;
            if (numberToCollect > maxNumberToCollect)
            {
                numberToCollect = maxNumberToCollect;
            }
        }

        if (numberToShoot < maxNumberToShoot)
        {
            numberToShoot += numberToShootChange;
            if (numberToShoot > maxNumberToShoot)
            {
                numberToShoot = maxNumberToShoot;
            }
        }

        if(numberOfChanges >= changesUntilSpawnIncrease && numberOfJumpSpawners < maxNumberOfJumpSpawners)
        {
            numberOfJumpSpawners += numberOfJumpSpawnersChange;
            if (numberOfJumpSpawners > maxNumberOfJumpSpawners)
            {
                numberOfJumpSpawners = maxNumberOfJumpSpawners;
            }
        }

        if (delayBetweenRobotSpawn > minDelayBetweenRobotSpawn)
        {
            delayBetweenRobotSpawn -= delayBetweenSpawnRobotChange;
            if (delayBetweenRobotSpawn > minDelayBetweenRobotSpawn)
            {
                delayBetweenRobotSpawn = minDelayBetweenRobotSpawn;
            }
        }

        if (robotMovementSpeed < maxRobotMovementSpeed)
        {
            robotMovementSpeed += robotMovementSpeedChange;
            if (robotMovementSpeed < maxRobotMovementSpeed)
            {
                robotMovementSpeed = maxRobotMovementSpeed;
            }
        }
    }

    void ResetStats()
    {
        levelsCompleted = 0;
        numberOfChanges = 0;
        levelsUntilChange = baseLevelsUntilChange;
        waitTimeOffset = baseWaitTimeOffset;
        livesRemaining = baseLivesRemaining;
        delayBetweenSpawn = baseDelayBetweenSpawn;
        numberToCatch = baseNumberToCatch;
        delayBetweenEnemySpawn = baseDelayBetweenEnemySpawn;
        enemyMovementSpeed = baseEnemyMovementSpeed;
        numberToCollect = baseNumberToCollect;
        currentRoundtype = Verbs.Default;
    }

    public void RandomiseInput(int playerNumber)
    {
        InputManager.PlayerKeys playerKeySelected = (InputManager.PlayerKeys)UnityEngine.Random.Range(0, 4);
        KeyCode newKeyCode = Manager<InputManager>.Instance.RandomiseOnePlayerKey(playerKeySelected, playerNumber);
        Manager<UIManager>.Instance.ShowRandomisedKey(playerKeySelected, newKeyCode);
    }

    public void SpawnPlayers()
    {
        players = new PlayerControl[Manager<InputManager>.Instance.numberOfPlayers];
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(PlayerPrefabs[i], playerSpawns[i].transform.position, playerSpawns[i].transform.rotation).GetComponent<PlayerControl>();
            players[i].playerNumber = i + 1;
            players[i].active = true;
        }
    }

    #endregion

    #region Game Setup Functions

    void InitialiseInputLists()
    {
        Manager<InputManager>.Instance.universalKeysList = keyboardObject.GetComponent<KeyboardAssignment>().GetUniversalKeys();
        Manager<InputManager>.Instance.player1KeysList = keyboardObject.GetComponent<KeyboardAssignment>().GetPlayer1Keys();
        Manager<InputManager>.Instance.player2KeysList = keyboardObject.GetComponent<KeyboardAssignment>().GetPlayer2Keys();
        Manager<InputManager>.Instance.unusedKeysList = Manager<InputManager>.Instance.InitialiseUnusedKeys();
    }

    #endregion

    #region Transition Functions

    void GoToSetup()
    {
        setupButton.onClick.RemoveAllListeners();
        SceneManager.LoadScene(1);
    }

    public void GoToTestingArea()
    {
        gameStartButton.onClick.RemoveAllListeners();
        uiManager = Instantiate(UIManagerObject);
        InitialiseInputLists();
        Manager<InputManager>.Instance.AssignNewPlayerKeys();
        Manager<UIManager>.Instance.Init();
        SceneManager.LoadScene(2);
    }

    void GoToMainMenu()
    {
        Destroy(inputManager);
        Destroy(uiManager);
        SceneManager.LoadScene(0);
    }

    void ContinuePressed()
    {
        if (livesRemaining > 0)
        {
            if (levelsUntilChange <= 0)
            {
                Destroy(currentAnimation);
                IncreaseChaos();
            }
            else
            {
                LoadNewRound();
            }
        }
        else
        {
            GoToMainMenu();
        }
    }

    #endregion

}
