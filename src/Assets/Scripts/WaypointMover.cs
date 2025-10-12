using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex;
    public Transform target;
    public bool isWaiting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting)
        {
            return;
        }
        MoveToWaypoint();
    }

    public void MoveToWaypoint()
    {
        target = waypoints[currentWaypointIndex];

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitWaypoint());

        }
    }
    IEnumerator WaitWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }

}
