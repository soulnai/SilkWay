using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using EnumSpace;

public class POI : MonoBehaviour {
    public GameStateManager GM;

    public bool occupied = false;
    public bool walkable = true;

    public List<POI> ConnectedNodes = new List<POI>();

    public int resource = 1;

    public int PathCost = 1;

    public int ID;

    public bool RandomEvent = true;

    public static event Action<POI> OnPOIClicked;

    public bool Victory = false;

    public bool revealed = false;

    public List<RandomEvent> Events = new List<RandomEvent>();

    // Use this for initialization
    void Start () {
        OnPOIClicked += MoveUnitToPOI;
        GM = GameObject.Find("GM").GetComponent<GameStateManager>();
    }

    private void MoveUnitToPOI(POI obj)
    {
        Debug.Log(GM.gamestate);
        if (GM.gamestate == GameStates.GlobalMap)
        {
            GM.MoveUnitToPOI(obj);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void OnMouseDown()
    {
        Debug.Log(GM.gamestate);
        OnPOIClicked(this);
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
