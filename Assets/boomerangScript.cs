using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class boomerangScript : MonoBehaviour
{
    public Transform startPoint, endPoint, center;
    public int count;
    public float radius;
    public GameObject guide;
    private List<GameObject> guideInstances = new List<GameObject>();
    private float startTime;
    public GameObject boomerang;
    private bool isMoving;
    public float speed;

    void Start()
    {
        startTime = Time.time;
        boomerang.transform.position = startPoint.position;

        InstantiateGuidePrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startTime = Time.time;
            isMoving = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isMoving = false;
            boomerang.transform.position = startPoint.position;
        }

        if (isMoving)
        {
            moveSphere(startPoint.position, endPoint.position, center.position);
        }
        
        UpdateGuidePositions(startPoint.position, endPoint.position, center.position);
    }

    public void OnDrawGizmos()
    {
        foreach (var point in EvaluateSlerpPoints(startPoint.position, endPoint.position, center.position, count))
        {
            Gizmos.DrawWireSphere(point, radius);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center.position, radius * 2);
    }

    void InstantiateGuidePrefabs()
    {
        foreach (var point in EvaluateSlerpPoints(startPoint.position, endPoint.position, center.position, count))
        {
            GameObject guideInstance = Instantiate(guide, point, Quaternion.identity);
            guideInstances.Add(guideInstance);
        }
    }

    void UpdateGuidePositions(Vector3 start, Vector3 end, Vector3 center)
    {
        var points = EvaluateSlerpPoints(start, end, center, count);
        int index = 0;
        foreach (var point in points)
        {
            if (index < guideInstances.Count)
            {
                guideInstances[index].transform.position = point;
            }
            index++;
        }
    }

    IEnumerable<Vector3> EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center, int count)
    {
        var startRelativeCenter = start - center;
        var endRelativeCenter = end - center;

        var f = 1f / count;

        for (float i = 0; i < 1 + f; i += f)
        {
            yield return Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + center;
        }
    }

    void moveSphere(Vector3 start, Vector3 end, Vector3 center)
    {
        var startRelativeCenter = start - center;
        var endRelativeCenter = end - center;

        float fracComplete = (Time. time - startTime) * speed;

        boomerang.transform.position = Vector3.Slerp(startRelativeCenter, endRelativeCenter, fracComplete);
        boomerang.transform.position += center;

        boomerang.transform.Rotate(Vector3.back, 360f * Time.deltaTime);

        if (fracComplete >= 1f)
        {
            isMoving = false;
        }
    }
}
