using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Datas/Building")]
public class BuildingData : ScriptableObject
{
    public enum Dir { Forward, Back, Left, Right };
    public static Dir GetNextDir(Dir dir)
    {
        switch(dir)
        {
            case Dir.Forward: return Dir.Right;
            case Dir.Right: return Dir.Back;
            case Dir.Back: return Dir.Left;
            case Dir.Left: return Dir.Forward;
            default: return default;
        }
    }

    public string BuildingName = null;
    public Transform prefab = null;
    public int width = 0;
    public int height = 0;

    public int GetRotationAngle(Dir dir)
    {
        return 0;
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        return Vector2Int.zero;
    }

    public List<Vector2Int> GetGridPositinoList(Vector2Int offset, Dir dir)
    {
        int width, height;
        switch(dir)
        {
            case Dir.Forward:
            case Dir.Back:
                width = this.width;
                height = this.height;
                break;
            default:
                width = this.height;
                height = this.width;
                break;
        }

        var gridPositionList = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
                gridPositionList.Add(offset + new Vector2Int(x, z));
        }
        return gridPositionList;
    }
}
