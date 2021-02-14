using System;
using UnityEngine;

public class BuildingModelController : MonoBehaviour
{
    [SerializeField] private float LerpScale = 0;

    private Transform visual = null;
    private GridBuildingSystem buildingSystem = null;

    private void Start()
    {
        buildingSystem = GridBuildingSystem.Instance;
        buildingSystem.OnSelectedChanged += Event_OnSlectedChanged;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = buildingSystem.GetMouseGridSnappedPosition() + Vector3.up;
        Quaternion targetRotation = buildingSystem.GetBulidingEntityRotation();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * LerpScale);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * LerpScale);
    }

    private void RefreshVisual()
    {
        if(visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        var preBuildingData = buildingSystem.GetSelectedBuildingData();
        if(preBuildingData != null)
        {
            visual = Instantiate(preBuildingData.visual, transform);
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 9);
        }
    }

    private void SetLayerRecursive(GameObject targetParent, int layer)
    {
        targetParent.layer = layer;
        foreach (Transform targetChild in targetParent.transform)
            SetLayerRecursive(targetChild.gameObject, layer);
    }

    private void Event_OnSlectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }
}
