using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject
{
    Grid<KeyObject> grid;
    int x, y;
    KeyCode key;
    bool active;
    SpriteRenderer spriteRenderer;
    
    public enum KeyAssignment { Universal, Player1, Player2, Unassigned};
    public KeyAssignment keyAssignment;

    public KeyObject(Grid<KeyObject> grid, int x, int y, KeyCode key, GameObject spawnObject)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.key = key;
        Vector2 worldPosition = grid.GridPositionToWorldSpace(new Vector2(x,y));
        Transform spawnParent = GameObject.FindObjectOfType<KeyboardAssignment>().transform;
        this.spriteRenderer = GameObject.Instantiate(spawnObject, new Vector3(worldPosition.x, worldPosition.y, -0.1f), Quaternion.identity, spawnParent).GetComponent<SpriteRenderer>();
    }

    public KeyCode GetKey()
    {
        return key;
    }

    public void SetKey(KeyCode key)
    {
        this.key = key;
    }

    public Vector2 GetGridPosition()
    {
        Vector2 positionVector = new Vector2(x, y);
        return positionVector;
    }

    public Grid<KeyObject> GetGridReference()
    {
        return grid;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetSpriteColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
