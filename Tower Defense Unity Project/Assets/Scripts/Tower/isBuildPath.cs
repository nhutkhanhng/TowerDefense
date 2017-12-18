using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isBuildPath : MonoBehaviour {

    [HideInInspector]
    public Transform target;

    public static bool isBuild = true;

    public static isBuildPath instance;

    void Awake()
    {
        instance = this;
        target = GameObject.FindGameObjectWithTag("End").transform;

        isBuild = PathFindding(transform.position, target.position, true);
    }

    public bool checkPath()
    {
        isBuild = PathFindding(transform.position, target.position, true);

        Debug.Log(isBuild);

        return isBuild;
    }
    public static bool PathFindding(Vector3 startPos, Vector3 targetPos, bool _Hypotenuse)
    {

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        NodeA startNode = Grid.instance.NodeFromWorldPoint(startPos);
        NodeA targetNode = Grid.instance.NodeFromWorldPoint(targetPos);


        if (startNode.walkable && targetNode.walkable)
        {
            Heap<NodeA> openSet = new Heap<NodeA>(Grid.instance.MaxSize);
            HashSet<NodeA> closedSet = new HashSet<NodeA>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                NodeA currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (NodeA neighbour in Grid.instance.GetNeighbours(currentNode, _Hypotenuse))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.gCost
                        || !openSet.Contains(neighbour))
                    {
                        //Assign neighbour node properties
                        neighbour.gCost = newMovementCostToNeighbour;
                        //Cost from current node to this neighbour node
                        neighbour.hCost = GetDistance(neighbour, targetNode);

                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        return pathSuccess;
    }

    public static int GetDistance(NodeA nodeA, NodeA nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
        //Vector3 vecCost = nodeA.worldPosition - nodeB.worldPosition;
        //return (int)vecCost.magnitude;
        // return Mathf.Abs(nodeB.gCost - nodeA.gCost);
    }
}
