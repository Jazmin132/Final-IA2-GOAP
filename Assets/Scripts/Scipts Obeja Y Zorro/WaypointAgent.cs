using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAgent : MonoBehaviour
{
    public GameObject[] allWaypoints;

    public float speed = 5;

    private int _currentWaypoint = 0;

    void Update()
    {
        GameObject waypoint = allWaypoints[_currentWaypoint];
        Vector3 dir = waypoint.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (dir.magnitude <= 0.3f)
        {
            _currentWaypoint++;
            if (_currentWaypoint > allWaypoints.Length - 1)
            {
                _currentWaypoint = 0;
            }
        }

    }
}
