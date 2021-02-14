using System;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int Width = 0;
    [SerializeField] private int Height = 0;
    [SerializeField] private int CellSize = 0;
    [SerializeField] private Transform StartTransform = null;
    [SerializeField] private List<BuildingData> BuildDataList = null;

    private Dir dir = default;
    private GridXZ<GridObject> grid = null;
    private BuildingData selectedBuildingData = null;

    public event EventHandler OnSelectedChanged = null;
    public static GridBuildingSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        UtilClass.InitializeUtilClass();
        grid = new GridXZ<GridObject>(StartTransform, Width, Height, CellSize, StartTransform.position, 
                                      (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && selectedBuildingData != null)
        {
            var gridPosition = GetMouseGridPosition();
            var gridObjectList = new List<GridObject>();
            var gridPositionList = selectedBuildingData.GetGridPositinoList(gridPosition, dir);

            foreach (var position in gridPositionList)
            {
                var gridObject = grid.GetGridObject(position);
                if (gridObject == null || !gridObject.CanBuild())
                {
                    Debug.Log(string.Format("Cannot build here ! : {0}", gridPosition));
                    return;
                }
                gridObjectList.Add(gridObject);
            }

            var worldPosition = grid.GetWorldPosition(gridPosition, BuildingData.GetRotationOffset(dir));
            var buildingEntity = BuildingEntity.Create(worldPosition, gridPosition, dir, selectedBuildingData);

            foreach (var gridObject in gridObjectList)
                gridObject.SetBuildingEntity(buildingEntity);
        }

        if(Input.GetMouseButtonDown(1) && selectedBuildingData != null)
        {
            DeselectBuildingData();
        }

        if(Input.GetMouseButtonDown(2))
        {
            var gridObject = grid.GetGridObject(GetMouseGridPosition());
            var buildingEntity = gridObject.GetBuildingEntity();
            if (buildingEntity != null)
            {
                var gridPositionList = buildingEntity.GetGridPositionList();
                buildingEntity.DestroySelf();

                foreach (var position in gridPositionList)
                {
                    gridObject = grid.GetGridObject(position);
                    if (gridObject != null)
                        gridObject.ClearBuildingEntity();
                }

            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            dir = BuildingData.GetNextDir(dir);
            Debug.Log(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectBuildingData(); }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedBuildingData = BuildDataList[0]; RefreshSelectedBulidingData(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedBuildingData = BuildDataList[1]; RefreshSelectedBulidingData(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedBuildingData = BuildDataList[2]; RefreshSelectedBulidingData(); }
    }

    public BuildingData GetSelectedBuildingData()
    {
        return selectedBuildingData;
    }

    public Vector3 GetMouseGridSnappedPosition()
    {
        var mousePosition = UtilClass.RaycastCamera();
        if (selectedBuildingData != null)
        {
            var rotationOffset = BuildingData.GetRotationOffset(dir);
            return grid.GetWorldPosition(grid.GetIntPosition(mousePosition), rotationOffset);
        }
        return mousePosition;
    }

    public Quaternion GetBulidingEntityRotation()
    {
        if (selectedBuildingData != null)
            return Quaternion.Euler(0, BuildingData.GetRotationAngle(dir), 0);
        else
            return Quaternion.identity;
    }

    private Vector2Int GetMouseGridPosition()
    {
        return grid.GetIntPosition(UtilClass.RaycastCamera());
    }

    private void RefreshSelectedBulidingData()
    {
        OnSelectedChanged.Invoke(this, EventArgs.Empty);
    }

    private void DeselectBuildingData()
    {
        selectedBuildingData = null; RefreshSelectedBulidingData();
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private BuildingEntity buildingEntity = null;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetBuildingEntity(BuildingEntity buildingEntity)
        {
            this.buildingEntity = buildingEntity;
            grid.TriggerGridObjectChanged(x, z);
        }

        public BuildingEntity GetBuildingEntity()
        {
            if (buildingEntity != null)
                return buildingEntity;
            return null;
        }

        public void ClearBuildingEntity()
        {
            buildingEntity = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return buildingEntity == null;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, z, buildingEntity != null);
        }
    }
}
