using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static Grid s_Instance = null;

    // This defines a stati c instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static Grid instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first GridManager object in the scene.
                s_Instance = FindObjectOfType(typeof(Grid)) as Grid;
                if (s_Instance == null)
                    Debug.Log("Could not locate an GridManager object. \n You have to have exactly one GridManager in the scene.");
            }
            return s_Instance;
        }
    }

    public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	NodeA[,] grid;
    
	float nodeDiameter;
	int gridSizeX, gridSizeY;

    void Awake() {
        //if (instance != null)
        //{
        //    Debug.LogError("More than one Grid!");
        //    return;
        //}
        //else
        //    s_Instance = this;


        nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

		CreateGrid();

        Debug.Log("Init GridMap");
    }

    
	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid() {
		grid = new NodeA[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));

				grid[x,y] = new NodeA(walkable,worldPoint, x,y);
			}
		}

        for (int i = 0; i <= gridSizeX / 2; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
                grid[i, j].gCost = grid[gridSizeX - i - 1,j].gCost = i;
        }

        for (int step = 0; step < gridSizeX / 2; step++)
        {
            for (int j = step; j < gridSizeX - step; j++)
            {
                grid[j,step].gCost = grid[j,gridSizeY - step - 1].gCost = step;
            }
        }
    }

	public List<NodeA> GetNeighbours(NodeA node, bool Hypotenuse) {
		List<NodeA> neighbours = new List<NodeA>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {

				if (x == 0 && y == 0)
					continue;

                if (Hypotenuse && (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public NodeA NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

		return grid[x,y];
	}

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (NodeA n in grid)
            {
                if (!n.walkable)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;

                    Vector3 draw = Vector3.one * nodeDiameter;
                    draw.y = .01f;

                    Gizmos.DrawCube(n.worldPosition, draw);
                }
            }
        }
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        // CreateGrid();

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        // Draw the horizontal grid lines
        for (int i = 0; i < gridSizeX + 1; i++)
        {
            Vector3 startPos = worldBottomLeft + i * this.nodeDiameter * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + gridWorldSize.x * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startPos, endPos, Color.blue);
        }
            
        // Draw the vertial grid lines
        for (int i = 0; i < gridSizeY + 1; i++)
        {
            Vector3 startPos = worldBottomLeft + i * nodeDiameter * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + gridWorldSize.y * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, Color.blue);
        }
    }

    void OnApplicationQuit()
    {
        s_Instance = null;
    }
}