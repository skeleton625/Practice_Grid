using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilClass
{
    private static Camera mainCamera = null;

    public static void InitializeUtilClass()
    {
        mainCamera = Camera.main;
    }

    public static Vector3 RaycastCamera()
    {
        var cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out RaycastHit hit, 1000f))
            return hit.point;
        return Vector3.zero;
    }
}
