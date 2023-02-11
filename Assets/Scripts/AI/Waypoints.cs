using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Waypoints : MonoBehaviour
{
    public Transform root;
    public Transform[] wayPoints;
    public Transform player;
    public Dictionary<int, int> invisiblePoints;

    private void Awake()
    {
        root = transform;
    }

    private void Start()
    {

        invisiblePoints = new Dictionary<int, int>();
        wayPoints = new Transform[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            wayPoints[i] = transform.GetChild(i).transform;
        }

        InvokeRepeating("CalculateInvisiblePonts", 0, 1f);
    }

    private void Update()
    {
        for (int i = 0; i < wayPoints.Length; i++)
        {
            Debug.DrawRay(wayPoints[i].position + new Vector3(0, 1f, 0), player.transform.position - (wayPoints[i].position + new Vector3(0, 1f, 0)), Color.green);

            if (invisiblePoints.ContainsValue(i))
            {
                wayPoints[i].gameObject.name = string.Format("(invisible) Point{0}", i);
            }
            else
            {
                wayPoints[i].gameObject.name = string.Format("Point{0}", i);
            }
        }
    }
    private void CalculateInvisiblePonts()
    {
        int index = 0;
        invisiblePoints.Clear();

        for (int i = 0; i < wayPoints.Length; i++)
        {
            //is visible in screen?
            if (!IsInScreen(wayPoints[i]))
            {
                invisiblePoints.Add(index++, i);
                continue;
            }

            Ray ray = new Ray(wayPoints[i].position + new Vector3(0, 1f, 0), player.transform.position - (wayPoints[i].position + new Vector3(0, 1f, 0)));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(wayPoints[i].position + new Vector3(0, 1f, 0), player.transform.position)))
            {
                if (hit.transform.tag != "Player")
                {
                    invisiblePoints.Add(index++, i);
                    continue;
                }
            }
        }
    }
    private bool IsInScreen(Transform targetTransform)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(targetTransform.position);
        Vector3 dir = (targetTransform.position - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >=0 && viewPos.y <=1 )
            return true;
        else
            return false;
    }
}
