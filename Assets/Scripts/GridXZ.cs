using System;
using UnityEngine;

public class GridXZ<GridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridObjectChanged = null;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int width = 0;
    private int height = 0;
    private int cellSize = 0;
    private Vector3 center = Vector3.zero;
    private Transform textParent = null;
    private GridObject[,] gridArray = null;

    public GridXZ(Transform parent, int width, int height, int cellSize, Vector3 center, Func<GridXZ<GridObject>, int, int, GridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.center = center;
        this.cellSize = cellSize;
        this.textParent = parent;

        gridArray = new GridObject[width, height];

        Vector3 worldPosition;
        Vector3 textPlusPosition = new Vector3(cellSize / 2, 0, cellSize / 2);
        var debugTextArray = new TextMesh[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
                worldPosition = GetWorldPosition(x, z);
                debugTextArray[x, z] = CreateText(worldPosition + textPlusPosition, gridArray[x, z].ToString(), Color.white, TextAnchor.MiddleCenter, 20, TextAlignment.Center);
                Debug.DrawLine(worldPosition, GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(worldPosition, GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
        worldPosition = GetWorldPosition(width, height);
        Debug.DrawLine(GetWorldPosition(0, height), worldPosition, Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), worldPosition, Color.white, 100f);

        OnGridObjectChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
        { debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z].ToString(); };
    }

    private TextMesh CreateText(Vector3 position, string text, Color color, TextAnchor anchor, int fontSize, TextAlignment alignment)
    {
        var textObject = new GameObject("World_Text", typeof(TextMesh));
        textObject.transform.position = position;
        textObject.transform.SetParent(textParent);
        var textMesh = textObject.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = color;
        textMesh.anchor = anchor;
        textMesh.fontSize = fontSize;
        textMesh.alignment = alignment;
        return textMesh;
    }

    public int GetCellSize()
    {
        return cellSize;
    }

    public GridObject GetGridObject(Vector2Int position)
    {
        if (position.x >= 0 && position.y >= 0 && 
            position.x < width && position.y < height)
        {
            return gridArray[position.x, position.y];
        }
        else
        {
            return default(GridObject);
        }
    }

    public Vector2Int GetIntPosition(Vector3 worldPosition)
    {
        var position = Vector2Int.zero;
        position.x = Mathf.FloorToInt((worldPosition.x - center.x) / cellSize);
        position.y = Mathf.FloorToInt((worldPosition.z - center.z) / cellSize);
        return position;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + center;
    }

    public Vector3 GetWorldPosition(Vector2Int position, Vector2Int rotation)
    {
        var intPosition = (position + rotation) * cellSize;
        return new Vector3(intPosition.x, 0, intPosition.y) + center;
    }

    /* Activate OnGridObjectChange EventHandler -> text change */
    public void TriggerGridObjectChanged(int x, int z)
    {
        /*
         *  if (OnGridObjectChanged != null)
         *      OnGridObjectChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
         */
        OnGridObjectChanged.Invoke(this, new OnGridValueChangedEventArgs { x = x, z = z });
    }
}
