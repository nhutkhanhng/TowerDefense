using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NodeA : IHeapItem<NodeA>
{

    #region Fields for A*
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public NodeA parent;

    int heapIndex;
    #endregion

    #region
    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    #endregion

    public NodeA()
    {
        walkable = true;
        worldPosition = new Vector3();
        gridX = gridY = 0;

    }
    public NodeA(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public NodeA(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gCost)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        this.gCost = _gCost;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(NodeA nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
