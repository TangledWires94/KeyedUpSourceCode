using System;
using UnityEngine;

public class Grid<TGridObject>
{
    TGridObject[,] gridObjects;
    public float cellSize, rowOffset;
    public Vector2 origin;

    public Grid(int x, int y, float cellSize, Vector2 origin, GameObject spawnObject, Func<Grid<TGridObject>, int, int, KeyCode, GameObject, TGridObject> CreateGridObject, float rowOffset = 0f)
    {
        gridObjects = new TGridObject[x, y];
        this.cellSize = cellSize;
        this.rowOffset = rowOffset;
        this.origin = origin;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                gridObjects[i, j] = CreateGridObject(this, i, j, KeyCode.None, spawnObject);
            }
        }
    }

    public TGridObject GetCellValue(int x, int y)
    {
        return gridObjects[x, y];
    }

    public Vector2 GridPositionToWorldSpace(Vector2 gridPosition)
    {
        float worldX = (cellSize * gridPosition.x) + origin.x + (cellSize / 2) - (rowOffset * gridPosition.y);
        float worldY = (cellSize * gridPosition.y) + origin.y + (cellSize / 2);
        return new Vector2(worldX, worldY);
    }

    public Vector2 WorldPositionToGridPosition(Vector2 worldPosition)
    {
        int gridY = Mathf.RoundToInt((worldPosition.y - origin.y - (cellSize / 2)) / cellSize);
        int gridX = Mathf.RoundToInt((worldPosition.x - origin.x - (cellSize / 2) + (rowOffset * gridY)) / cellSize);
        return new Vector2(gridX, gridY);
    }

    public TGridObject[,] GetGridObjects()
    {
        return gridObjects;
    }

    public bool PositionWithinGrid(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridObjects.GetLength(0) && gridPosition.y >= 0 && gridPosition.y < gridObjects.GetLength(1);
    }
}
