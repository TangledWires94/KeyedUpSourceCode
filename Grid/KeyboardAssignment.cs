using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardAssignment : MonoBehaviour
{
    Grid<KeyObject> KeyboardGrid;
    public List<KeyCode> UniversalKeys = new List<KeyCode>();
    public List<KeyCode> Player1Keys = new List<KeyCode>();
    public List<KeyCode> Player2Keys = new List<KeyCode>();
    KeyObject.KeyAssignment assignee;
    Dropdown assigneeDropdown;

    public float cellSize;
    public Vector2 origin;
    public GameObject highlightObject;
    public Color universalColor, player1Color, player2Color, unassignedColor;
    public Vector2[] unusedCells;

    public GameObject universalColourMarker, player1ColourMarker, player2ColourMarker, unassignedColourMarker;

    KeyCode[,] keyboardKeys = new KeyCode[,] { { KeyCode.Z, KeyCode.A, KeyCode.Q }, { KeyCode.X, KeyCode.S, KeyCode.W }, { KeyCode.C, KeyCode.D, KeyCode.E }, { KeyCode.V, KeyCode.F, KeyCode.R }, { KeyCode.B, KeyCode.G, KeyCode.T }, { KeyCode.N, KeyCode.H, KeyCode.Y }, { KeyCode.M, KeyCode.J, KeyCode.U }, { KeyCode.None, KeyCode.K, KeyCode.I }, { KeyCode.None, KeyCode.L, KeyCode.O }, { KeyCode.None, KeyCode.None, KeyCode.P } };

    struct KeyAssignmentInfo
    {
        public List<KeyCode> keys;
        public Color keyColor;

        public KeyAssignmentInfo(List<KeyCode> keys, Color keyColor)
        {
            this.keys = keys;
            this.keyColor = keyColor;
        }
    }

    Dictionary<KeyObject.KeyAssignment, KeyAssignmentInfo> assignmentInfoDictionary = new Dictionary<KeyObject.KeyAssignment, KeyAssignmentInfo>();

    void Start()
    {
        //Set up keyboard grid
        KeyboardGrid = new Grid<KeyObject>(10, 3, cellSize, origin, highlightObject, (Grid<KeyObject> grid, int x, int y, KeyCode key, GameObject highlightObject) => new KeyObject(grid, x, y, key, highlightObject), cellSize/2);
        KeyObject[,] keys = KeyboardGrid.GetGridObjects();
        foreach(KeyObject keyObject in keys)
        {
            keyObject.SetActive(true);
            UpdateKey(keyObject, KeyObject.KeyAssignment.Universal, universalColor);
        }
        foreach(Vector2 position in unusedCells)
        {
            UpdateKey(keys[(int)position.x, (int)position.y], KeyObject.KeyAssignment.Unassigned, unassignedColor);
            keys[(int)position.x, (int)position.y].SetActive(false);
        }

        //Assign keycodes to keys and add to universal keys list
        for (int j = 0; j < keys.GetLength(1); j++)
        {
            for (int i = 0; i < keys.GetLength(0); i++)
            {
                keys[i, j].SetKey(keyboardKeys[i, j]);
                if(keys[i, j].GetKey() != KeyCode.None)
                {
                    UniversalKeys.Add(keys[i, j].GetKey());
                }
            }
        }

        //Set up assignment info for when keys are clicked, universal is the default starting assignee
        assignee = KeyObject.KeyAssignment.Universal;
        assignmentInfoDictionary.Add(KeyObject.KeyAssignment.Universal, new KeyAssignmentInfo(UniversalKeys, universalColor));
        assignmentInfoDictionary.Add(KeyObject.KeyAssignment.Player1, new KeyAssignmentInfo(Player1Keys, player1Color));
        assignmentInfoDictionary.Add(KeyObject.KeyAssignment.Player2, new KeyAssignmentInfo(Player2Keys, player2Color));

        //Get reference to dropdown
        assigneeDropdown = FindObjectOfType<Dropdown>();
        List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
        dropdownOptions.Add(new Dropdown.OptionData("Universal"));
        dropdownOptions.Add(new Dropdown.OptionData("Player 1"));
        dropdownOptions.Add(new Dropdown.OptionData("Player 2"));
        assigneeDropdown.ClearOptions();
        assigneeDropdown.AddOptions(dropdownOptions);
        assigneeDropdown.onValueChanged.AddListener(delegate
        {
            ChangeAssignee();
        });

        //Set marker colours
        universalColourMarker.transform.Find("Colour Object").GetComponent<RawImage>().color = universalColor;
        player1ColourMarker.transform.Find("Colour Object").GetComponent<RawImage>().color = player1Color;
        player2ColourMarker.transform.Find("Colour Object").GetComponent<RawImage>().color = player2Color;
        unassignedColourMarker.transform.Find("Colour Object").GetComponent<RawImage>().color = Color.black;

        //Hide unused character colours
        ChangeNumberOfPlayers(Manager<InputManager>.Instance.numberOfPlayers);
    }

    void Update()
    {
        //if left mouse button clicked add or remove 
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 gridPosition = KeyboardGrid.WorldPositionToGridPosition(mousePosition);
            if (KeyboardGrid.PositionWithinGrid(gridPosition))
            {
                KeyObject selectedKey = KeyboardGrid.GetGridObjects()[(int)gridPosition.x, (int)gridPosition.y];
                if (selectedKey.GetActive())
                {
                    KeyAssignmentInfo assignmentInfo;
                    assignmentInfoDictionary.TryGetValue(assignee, out assignmentInfo);

                    if (selectedKey.keyAssignment == assignee)
                    {
                        assignmentInfo.keys.Remove(selectedKey.GetKey());
                        UpdateKey(selectedKey, KeyObject.KeyAssignment.Unassigned, unassignedColor);
                    } 
                    else
                    {
                        if (selectedKey.keyAssignment != KeyObject.KeyAssignment.Unassigned)
                        {
                            KeyAssignmentInfo oldInfo;
                            assignmentInfoDictionary.TryGetValue(selectedKey.keyAssignment, out oldInfo);
                            oldInfo.keys.Remove(selectedKey.GetKey());
                        }
                        assignmentInfo.keys.Add(selectedKey.GetKey());
                        UpdateKey(selectedKey, assignee, assignmentInfo.keyColor);
                    }
                }
            }
        }
    }

    public void UpdateKey(KeyObject keyObject, KeyObject.KeyAssignment keyAssignment, Color color)
    {
        keyObject.SetSpriteColor(color);
        keyObject.keyAssignment = keyAssignment;
    }

    public void SetAllUniversal()
    {
        UniversalKeys.Clear();
        Player1Keys.Clear();
        Player2Keys.Clear();

        foreach(KeyObject keyObject in KeyboardGrid.GetGridObjects())
        {
            if (keyObject.GetActive())
            {
                UpdateKey(keyObject, KeyObject.KeyAssignment.Universal, universalColor);
                UniversalKeys.Add(keyObject.GetKey());
            }
        }
    }

    public void UnassignAllKeys()
    {
        UniversalKeys.Clear();
        Player1Keys.Clear();
        Player2Keys.Clear();

        foreach (KeyObject keyObject in KeyboardGrid.GetGridObjects())
        {
            UpdateKey(keyObject, KeyObject.KeyAssignment.Unassigned, unassignedColor);
        }
    }

    public void ChangeAssignee()
    {
        int optionSelected = assigneeDropdown.value;
        assignee = (KeyObject.KeyAssignment)optionSelected;   
    }

    public void ChangeNumberOfPlayers(int newNumberOfPlayers)
    {
        if(Manager<InputManager>.Instance.numberOfPlayers > 1)
        {
            player1ColourMarker.SetActive(true);
            player2ColourMarker.SetActive(true);
            assigneeDropdown.gameObject.SetActive(true);
        } else
        {
            player1ColourMarker.SetActive(false);
            player2ColourMarker.SetActive(false);
            assigneeDropdown.gameObject.SetActive(false);
            KeyObject[,] grid = KeyboardGrid.GetGridObjects();
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j].keyAssignment == KeyObject.KeyAssignment.Player1)
                    {
                        Player1Keys.Remove(grid[i, j].GetKey());
                        UniversalKeys.Add(grid[i, j].GetKey());
                        UpdateKey(grid[i, j], KeyObject.KeyAssignment.Universal, universalColor);
                    } else if (grid[i, j].keyAssignment == KeyObject.KeyAssignment.Player2)
                    {
                        Player2Keys.Remove(grid[i, j].GetKey());
                        UniversalKeys.Add(grid[i, j].GetKey());
                        UpdateKey(grid[i, j], KeyObject.KeyAssignment.Universal, universalColor);
                    }
                }
            }
        }        
    }

    public List<KeyCode> GetUniversalKeys()
    {
        return UniversalKeys;
    }

    public List<KeyCode> GetPlayer1Keys()
    {
        return Player1Keys;
    }

    public List<KeyCode> GetPlayer2Keys()
    {
        return Player2Keys;
    }
}
