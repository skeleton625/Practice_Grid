using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int Width = 0;
    [SerializeField] private int Height = 0;
    [SerializeField] private float CellSize = 0f;
    [SerializeField] private Transform StartTransform = null;
    [SerializeField] private BuildingData buildData = null;

    private Camera mainCamera = null;
    private GridXZ<GridObject> grid = null;

    private void Awake()
    {
        mainCamera = Camera.main;
        grid = new GridXZ<GridObject>(StartTransform, Width, Height, CellSize, StartTransform.position, 
                                      (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(cameraRay.origin, cameraRay.direction, out RaycastHit hit, 1000f))
            {
                grid.GetIntPosition(hit.point - StartTransform.position, out int x, out int z);

                var gridPositionList = buildData.GetGridPositinoList(new Vector2Int(x, z), BuildingData.Dir.Forward);
                var gridObjectList = new List<GridObject>();

                foreach(var position in gridPositionList)
                {
                    var gridObject = grid.GetGridObject(position.x, position.y);
                    if (!gridObject.CanBuild())
                    {
                        Debug.Log(string.Format("Cannot build here ! : {0}", hit.point));
                        return;
                    }
                    gridObjectList.Add(gridObject);
                }

                var buildTransform = Instantiate(buildData.prefab, grid.GetWorldPosition(x, z), Quaternion.identity);
                foreach(var gridObject in gridObjectList)
                    gridObject.SetTransform(buildTransform);
            }
        }
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearTransform()
        {
            transform = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, z, transform);
        }
    }
}
