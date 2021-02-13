using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dir { Forward, Back, Left, Right };

[CreateAssetMenu(fileName = "New Building", menuName = "Datas/Building")]
public class BuildingData : ScriptableObject
{
    public static Dir GetNextDir(Dir dir)
    {
        switch(dir)
        {
            case Dir.Forward:   return Dir.Left;
            case Dir.Left:      return Dir.Back;
            case Dir.Back:      return Dir.Right;
            case Dir.Right:     return Dir.Forward;
            default: return default;
        }
    }

    public static int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            case Dir.Forward: return 0;
            case Dir.Left: return 90;
            case Dir.Back: return 180;
            case Dir.Right: return 270;
            default: return 0;
        }
    }

    public static Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            case Dir.Forward: return Vector2Int.zero;
            case Dir.Left: return new Vector2Int(0, 1);
            case Dir.Back: return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    public string BuildingName = null;
    public Transform prefab = null;
    public int width = 0;
    public int height = 0;

    public List<Vector2Int> GetGridPositinoList(Vector2Int offset, Dir dir)
    {
        int width = 0, height = 0, xScale = 0, zScale = 0;
        var gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            case Dir.Forward:
                width = this.width; height = this.height;
                xScale = 1; zScale = 1;
                break;
            case Dir.Left:
                width = this.height; height = this.width;
                xScale = 1; zScale = -1;
                break;
            case Dir.Back:
                width = this.width; height = this.height;
                xScale = -1; zScale = -1;
                break;
            case Dir.Right:
                width = this.height; height = this.width;
                xScale = -1; zScale = 1;
                break;
        }

        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                gridPositionList.Add(offset + new Vector2Int(x * xScale, z * zScale));

        return gridPositionList;
    }
}
