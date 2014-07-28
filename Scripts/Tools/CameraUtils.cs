using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CameraUtils
{
    public static void GetCameraLimitPointsAtZ(float camZ, out Vector3 camMinPoint, out Vector3 camMaxPoint)
    {
        camMinPoint = Vector3.zero;
        camMaxPoint = Vector3.zero;

        Ray camMinRay = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
        Ray camMaxRay = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0));

        Vector3 zPoint = new Vector3(0, 0, camZ);
        Plane zPlane = new Plane(-Vector3.forward, zPoint);

        float rayDistance = 0;
        if (zPlane.Raycast(camMinRay, out rayDistance))
        {
            camMinPoint = camMinRay.GetPoint(rayDistance);
        }
        if (zPlane.Raycast(camMaxRay, out rayDistance))
        {
            camMaxPoint = camMaxRay.GetPoint(rayDistance);
        }

        //Debug.Log("CameraFilterPicture - Screen width = " + Camera.main.pixelWidth + " - height = " + Camera.main.pixelHeight + " - cam Z = " + camZ + " -> camMinPoint = " + camMinPoint + " - camMaxPoint = " + camMaxPoint);
    }

    public static void ComputeCameraSpaceSize(float camZ, out float camSpaceWidth, out float camSpaceHeight)
    {
        Vector3 camMinPoint;
        Vector3 camMaxPoint;
        GetCameraLimitPointsAtZ(camZ, out camMinPoint, out camMaxPoint);
        camSpaceWidth = camMaxPoint.x - camMinPoint.x;
        camSpaceHeight = camMaxPoint.y - camMinPoint.y;
        //Debug.Log("CameraFilterPicture => camSpaceWidth = " + camSpaceWidth + " - camSpaceHeight = " + camSpaceHeight);
    }

    public static void ResizeSprite(SpriteRenderer spriteRenderer, ref Vector3 scale, float newWidth, float newHeight, bool uniformScaling)
    {
        float originalSpriteWidthInUnits = spriteRenderer.sprite.bounds.size.x;
        float originalSpriteHeightInUnits = spriteRenderer.sprite.bounds.size.y;
        float currentSpriteWidthInUnits = originalSpriteWidthInUnits * scale.x;
        float currentSpriteHeightInUnits = originalSpriteHeightInUnits * scale.y;
        float scaleWidthFactor = newWidth / currentSpriteWidthInUnits;
        float scaleHeightFactor = newHeight / currentSpriteHeightInUnits;
        //Debug.Log("CameraFilterPicture - Sprite position = " + spriteRenderer.transform.position + " - original width = " + originalSpriteWidthInUnits + ", height = " + originalSpriteHeightInUnits + " - current width = " + currentSpriteWidthInUnits + ", height = " + currentSpriteHeightInUnits);
        //Debug.Log("CameraFilterPicture => scale factor for width = " + scaleWidthFactor + ", height = " + scaleHeightFactor);

        if (uniformScaling)
        {
            float maxScaleFactor = Mathf.Max(scaleWidthFactor, scaleHeightFactor);
            scaleWidthFactor = maxScaleFactor;
            scaleWidthFactor = maxScaleFactor;
        }

        scale.x *= scaleWidthFactor;
        //scale.y *= scaleHeightFactor; TODO: not correct for height, to adjust
    }
}
