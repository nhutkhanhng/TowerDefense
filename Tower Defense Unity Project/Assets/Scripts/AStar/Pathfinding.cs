using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {
	
	PathRequestManager requestManager;
    public static bool isFound = true;
    // Grid grid;
	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
	}

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, bool Hypotenuse) {
		StartCoroutine(FindPath(startPos,targetPos, Hypotenuse));
	}
	
	public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, bool _Hypotenuse) {

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
        
        NodeA startNode = Grid.instance.NodeFromWorldPoint(startPos);
        NodeA targetNode = Grid.instance.NodeFromWorldPoint(targetPos);
		
		
		if (startNode.walkable && targetNode.walkable) {
			Heap<NodeA> openSet = new Heap<NodeA>(Grid.instance.MaxSize);
			HashSet<NodeA> closedSet = new HashSet<NodeA>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
                NodeA currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (NodeA neighbour in Grid.instance.GetNeighbours(currentNode, _Hypotenuse)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

					if (newMovementCostToNeighbour < neighbour.gCost 
                        || !openSet.Contains(neighbour)) {
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
		yield return null;

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        isFound = pathSuccess;
            // Chổ này là hết đường đi rồi nè


        requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	
	Vector3[] RetracePath(NodeA startNode, NodeA endNode) {
		List<NodeA> path = new List<NodeA>();
        NodeA currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	
	Vector3[] SimplifyPath(List<NodeA> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
        int i = 1;
		for (; i < path.Count; i ++) {

			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,
                                    path[i-1].gridY - path[i].gridY);

			if (directionNew != directionOld) 
				waypoints.Add(path[i -1].worldPosition);

			directionOld = directionNew;
		}

        // Thằng chó nào Code đoạn này chơi bố. Bỏ mẹ điểm đầu mà còn Smooth
        // Như vầy Smooth mới chính cmn Xác
        waypoints.Add(path[i - 1].worldPosition);

		return waypoints.ToArray();
	}
	
	public static int GetDistance(NodeA nodeA, NodeA nodeB) {
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
