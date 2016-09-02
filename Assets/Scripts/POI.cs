using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class POI : MonoBehaviour {
    public bool occupied = false;
    public bool walkable = true;

    public List<POI> ConnectedNodes = new List<POI>();

    public int resource = 1;

    public int PathCost = 1;

    public bool RandomEvent = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddConnection(POI NodeTo)
    {
        ConnectedNodes.Add(NodeTo);
    }

    public void ResetConnections()
    {
        ConnectedNodes.Clear();
    }

    public void SetOccupiedTrue()
    {
        occupied = true;
    }

    public void SetOccupiedFalse()
    {
        occupied = false;
    }

    public void SetWalkableTrue()
    {
        walkable = true;
    }

    public void SetWalkableFalse()
    {
        walkable = false;
    }

    public void SetResource(int NewResource)
    {
        resource = NewResource;
    }

    public void SetPathCost(int NewPathCost)
    {
        PathCost = NewPathCost;
    }
}
