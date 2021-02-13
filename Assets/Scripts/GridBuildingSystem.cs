using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int Width = 0;
    [SerializeField] private int Height = 0;
    [SerializeField] private float CellSize = 0f;
    [SerializeField] private Transform StartTransform = null;
    [SerializeField] private List<BuildingData> buildDataList = null;

    private Dir dir = default;
    private Camera mainCamera = null;
    private GridXZ<GridObject> grid = null;
    private BuildingData preBuildingData = null;

    private void Awake()
    {
        mainCamera = Camera.main;
        grid = new GridXZ<GridObject>(StartTransform, Width, Height, CellSize, StartTransform.position, 
                                      (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

        preBuildingData = buildDataList[0];
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(cameraRay.origin, cameraRay.direction, out RaycastHit hit, 1000f))
            {
                grid.GetIntPosition(hit.point - StartTransform.position, out int x, out int z);
                var gridPosition = new Vector2Int(x, z);
                var gridObjectList = new List<GridObject>();
                var gridPositionList = preBuildingData.GetGridPositinoList(gridPosition, dir);

                foreach (var position in gridPositionList)
                {
                    var gridObject = grid.GetGridObject(position.x, position.y);
                    if (gridObject == null || !gridObject.CanBuild())
                    {
                        Debug.Log(string.Format("Cannot build here ! : {0}", hit.point));
                        return;
                    }
                    gridObjectList.Add(gridObject);
                }

                var rotationOffset = BuildingData.GetRotationOffset(dir);
                var objectPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                var buildingEntity = BuildingEntity.Create(objectPosition, gridPosition, dir, preBuildingData);

                foreach(var gridObject in gridObjectList)
                    gridObject.SetBuildingEntity(buildingEntity);
            }
        }

        if(Input.GetMouseButtonDown(2))
        {
            var cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(cameraRay.origin, cameraRay.direction, out RaycastHit hit, 1000f))
            {
                grid.GetIntPosition(hit.point - StartTransform.position, out int x, out int z);
                var gridObject = grid.GetGridObject(x, z);
                var buildingEntity = gridObject.GetBuildingEntity();
                if (buildingEntity != null)
                {
                    var gridPositionList = buildingEntity.GetGridPositionList();
                    buildingEntity.DestroySelf();

                    foreach(var position in gridPositionList)
                    {
                        gridObject = grid.GetGridObject(position.x, position.y);
                        if (gridObject != null)
                            gridObject.ClearBuildingEntity();
                    }

                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            dir = BuildingData.GetNextDir(dir);
            Debug.Log(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) preBuildingData = buildDataList[0];
        if (Input.GetKeyDown(KeyCode.Alpha2)) preBuildingData = buildDataList[1];
        if (Input.GetKeyDown(KeyCode.Alpha3)) preBuildingData = buildDataList[2];
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
