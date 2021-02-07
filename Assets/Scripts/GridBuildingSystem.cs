using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int Width = 0;
    [SerializeField] private int Height = 0;
    [SerializeField] private float CellSize = 0f;
    [SerializeField] private Transform centerTransform = null;

    private GridXZ<GridObject> grid = null;

    private void Awake()
    {
        grid = new GridXZ<GridObject>(centerTransform, Width, Height, CellSize, centerTransform.position, 
                                      (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", x, z);
        }
    }
}
