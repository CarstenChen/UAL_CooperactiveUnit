using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPriority : MonoBehaviour
{
    Light spotLight;

    private void Awake()
    {
        spotLight = GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        if (IsInScreen(this.transform))
        {
            spotLight.renderMode = LightRenderMode.ForcePixel;
        }
        else
        {
            spotLight.renderMode = LightRenderMode.Auto;
        }
    }


    private bool IsInScreen(Transform targetTransform)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(targetTransform.position);
        Vector3 dir = (targetTransform.position - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }
}
