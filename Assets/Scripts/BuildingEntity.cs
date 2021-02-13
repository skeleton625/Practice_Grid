using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEntity : MonoBehaviour
{
    public static BuildingEntity Create(Vector3 worldPosition, Vector2Int origin , Dir dir, BuildingData buildData)
    {
        var buildTransform = Instantiate(buildData.prefab, worldPosition, Quaternion.Euler(0, BuildingData.GetRotationAngle(dir), 0));
        var buildingEntity = buildTransform.GetComponent<BuildingEntity>();

        buildingEntity.buildData = buildData;
        buildingEntity.origin = origin;
        buildingEntity.dir = dir;

        return buildingEntity;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return buildData.GetGridPositinoList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private BuildingData buildData = null;
    private Vector2Int origin = default;
    private Dir dir = default;
}
