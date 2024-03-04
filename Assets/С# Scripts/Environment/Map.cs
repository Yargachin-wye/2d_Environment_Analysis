using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int graphSize => transform.childCount;

    private MapGraph graph;
    private Vector3 labelPosition;
    public static Map instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("instance != null");
            return;
        }
        instance = this;
    }
    private void Start()
    {
        CreateGraph();
        FindePath(5, 0);
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Handles.color = Color.red;
        int id = 0;

        foreach (Transform child in transform)
        {
            List<Point> points = child.GetComponent<Point>().connectionPoints;
            for(int i = 0; i < points.Count; i++)
            {
                if (points[i] != null)
                {
                    Handles.DrawLine(child.position, points[i].transform.position);
                }
            }
            Vector3 worldPosition = child.transform.TransformPoint(labelPosition);

            Handles.Label(worldPosition + Vector3.up, id.ToString());
            child.gameObject.name = id.ToString();
            id++;
        }
    }
#endif  
    public void CreateGraph()
    {
        graph = new MapGraph(graphSize);
        int id = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<Point>().Id = id;
            id++;
        }
        foreach (Transform child in transform)
        {
            List<Point> points = child.GetComponent<Point>().connectionPoints;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == null)
                {
                    child.GetComponent<Point>().connectionPoints.Remove(points[i]);
                    continue;
                }
                graph.AddEdge(
                    child.GetComponent<Point>().Id,
                    points[i].Id,
                    Vector2.Distance(child.position, points[i].transform.position
                    ));
                graph.AddEdge(
                    points[i].Id,
                    child.GetComponent<Point>().Id,
                    Vector2.Distance(child.position, points[i].transform.position
                    ));
            }
        }
    }

    public void FindePath(int id, int idTarget)
    {
        List<int> path = graph.AStar(id, idTarget);
        if (path == null )
        {
            Debug.LogWarning("path == null!");
            return;
        }
        string str = "";
        foreach (int x in path)
        {
            str += x.ToString() + " ";
        }
        Debug.Log(str);
    }

    public Vector2 GetPointPos(int id)
    {
        return transform.GetChild(id).position;
    }
}
