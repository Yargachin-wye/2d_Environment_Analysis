using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGraph
{
    private List<GraphNode> nodes = new List<GraphNode>();

    public MapGraph(int size)
    {
        for (int i = 0; i < size; i++)
        {
            nodes.Add(new GraphNode(i));
        }
    }

    public void DebugGraph()
    {
        foreach(GraphNode node in nodes)
        {
            string str = node.Id.ToString() + ": ";
            foreach (Edge edge in node.Neighbors)
            {
                str += edge.TargetNode.Id + " ";
            }
            Debug.Log(str);
        }
    }

    public void AddEdge(int source, int destination, float dist)
    {
        nodes[source].Neighbors.Add(new Edge(nodes[destination], dist));
    }

    public List<int> AStar(int start, int goal)
    {
        // Initialize dictionaries to store costs and parent nodes
        Dictionary<GraphNode, float> gScore = new Dictionary<GraphNode, float>();
        Dictionary<GraphNode, float> fScore = new Dictionary<GraphNode, float>();
        Dictionary<GraphNode, GraphNode> cameFrom = new Dictionary<GraphNode, GraphNode>();

        // Initialize priority queue for open set
        PriorityQueue<GraphNode> openSet = new PriorityQueue<GraphNode>();

        // Initialize costs for start node
        gScore[nodes[start]] = 0;
        fScore[nodes[start]] = HeuristicCost(nodes[start], nodes[goal]);

        // Add start node to open set
        openSet.Enqueue(nodes[start], fScore[nodes[start]]);

        while (openSet.Count > 0)
        {
            // Get node with lowest fScore from open set
            GraphNode current = openSet.Dequeue();

            // If current node is the goal, reconstruct and return path
            if (current == nodes[goal])
            {
                List<int> path = ReconstructPath(cameFrom, current);
                return path;
            }

            // Check neighbors of current node
            foreach (Edge neighborEdge in current.Neighbors)
            {
                GraphNode neighbor = neighborEdge.TargetNode;
                float tentativeGScore = gScore[current] + neighborEdge.Distance;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCost(neighbor, nodes[goal]);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        // If open set is empty and goal was not reached, return null
        return null;
    }

    private float HeuristicCost(GraphNode nodeA, GraphNode nodeB)
    {
        // This heuristic function returns the Euclidean distance between two nodes
        return MathF.Sqrt(MathF.Pow(nodeB.Id - nodeA.Id, 2));
    }

    private List<int> ReconstructPath(Dictionary<GraphNode, GraphNode> cameFrom, GraphNode current)
    {
        List<int> path = new List<int>();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current.Id);
            current = cameFrom[current];
        }
        path.Insert(0, current.Id); // Add start node
        return path;
    }
}

public class GraphNode
{
    public int Id { get; }
    public List<Edge> Neighbors { get; }

    public GraphNode(int id)
    {
        Id = id;
        Neighbors = new List<Edge>();
    }
}

public class Edge
{
    public GraphNode TargetNode { get; }
    public float Distance { get; }

    public Edge(GraphNode targetNode, float distance)
    {
        TargetNode = targetNode;
        Distance = distance;
    }
}

public class PriorityQueue<T>
{
    private SortedDictionary<float, Queue<T>> dict = new SortedDictionary<float, Queue<T>>();

    public int Count { get; private set; }

    public void Enqueue(T item, float priority)
    {
        if (!dict.ContainsKey(priority))
        {
            dict[priority] = new Queue<T>();
        }

        dict[priority].Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        var item = dict.First();
        var queue = item.Value;
        var result = queue.Dequeue();
        if (queue.Count == 0)
        {
            dict.Remove(item.Key);
        }
        Count--;
        return result;
    }

    public bool Contains(T item)
    {
        foreach (var queue in dict.Values)
        {
            if (queue.Contains(item))
                return true;
        }
        return false;
    }
}
