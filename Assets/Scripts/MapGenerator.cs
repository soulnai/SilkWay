﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Vectrosity;
using System.Linq;

public class MapGenerator : MonoBehaviour {
    public GameStateManager GM;
    public Transform mapTransform;
    public List<GameObject> AllPoi = new List<GameObject>();
    //public List<PointNode> AllPoiNodes = new List<PointNode>();
    public List<List<GameObject>> All = new List<List<GameObject>>();

    public Material lineMat;

    public int POIcount = 3;
    public int rowCount = 3;

    public GameObject endPOI;
    // Use this for initialization
    void Start () {
        GM = GameObject.Find("GM").GetComponent<GameStateManager>();
        MapGenerate();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void MapGenerate()
    {
        Vector3 basePoint = new Vector3(0f, 0, 0);
        lineMat.color = Color.green;
        mapTransform = GameObject.Find("MapController").transform;
        GameObject startPOI = Instantiate(Resources.Load("prefab/Cube"), basePoint, Quaternion.identity) as GameObject;
        startPOI.GetComponent<POI>().ID = 0;
        AllPoi.Add(startPOI);

        int id = 1;
        float x = 0f;

        for (int i = 0; i < rowCount; i++)
        {
            List<GameObject> smallList = new List<GameObject>();
            float z = -2.5f;
            for (int j = 0; j < POIcount; j++)
            {
                float randX = Random.Range(1f, 2f);

                GameObject poi = PlacePoi(x, 0, randX, startPOI);

                poi.GetComponent<POI>().ID = id;
                GenerateEvents(poi.GetComponent<POI>());
                id++;
                AllPoi.Add(poi);
                smallList.Add(poi);
                if (i != 0)
                    poi.gameObject.SetActive(false);
                z += 2.5f;
            }
            All.Add(smallList);
            x += 1.5f;
        }

        float largestX = AllPoi.Last().gameObject.transform.position.x;
        foreach (GameObject poi in AllPoi)
        {
            if (poi.gameObject.transform.position.x > largestX)
            {
                largestX = poi.gameObject.transform.position.x;
            }
        }

        Debug.Log(largestX);
        Debug.Log(x);
        Debug.Log(AllPoi.Last().gameObject.transform.position.x);
        float newx = 1f + largestX;
        endPOI = Instantiate(Resources.Load("prefab/Cube"), new Vector3(newx, 0, 0), Quaternion.identity) as GameObject;
        endPOI.GetComponent<Renderer>().material.color = Color.green;
        endPOI.GetComponent<POI>().ID = id;
        endPOI.transform.position = new Vector3(newx, 0, 0);
        AllPoi.Add(endPOI);
        endPOI.SetActive(false);
        AllPoi.Last().GetComponent<POI>().Victory = true;

        foreach (GameObject p in All[0])
        {
            ConnectPOIs(p.GetComponent<POI>(), startPOI.GetComponent<POI>());
        }

        foreach (GameObject p in All[All.Count-1])
        {
            ConnectPOIs(p.GetComponent<POI>(), endPOI.GetComponent<POI>());
        }

        for (int i = 0; i < rowCount-1; i++)
        {
            for (int j = 0; j < POIcount; j++)
            {
                foreach (GameObject p in All[i + 1])
                {
                    ConnectPOIs(p.GetComponent<POI>(), All[i][j].GetComponent<POI>());
                }
            }

        }

        List<GameObject> RandomPois = new List<GameObject>();
        RandomPois.Add(AllPoi[Random.Range(0, AllPoi.Count)]);
        RandomPois.Add(AllPoi[Random.Range(0, AllPoi.Count)]);
        RandomPois.Add(AllPoi[Random.Range(0, AllPoi.Count)]);

        foreach (GameObject point in AllPoi)
        {
            for (int counts = 0; counts < 2; counts++)
            {
                int remove = Random.Range(0, 2);
                if (remove == 1 && point.GetComponent<POI>().ConnectedNodes.Count >= 3 && point != startPOI && point != endPOI && !startPOI.GetComponent<POI>().ConnectedNodes.Contains(point.GetComponent<POI>()))
                {
                    POI randomnode = point.GetComponent<POI>().ConnectedNodes[Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count)];
                    point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    randomnode.GetComponent<POI>().ConnectedNodes.Remove(point.GetComponent<POI>());
                }
            }
        }

        /*foreach (GameObject point in AllPoi)
        {
            foreach (POI connectedPoi in point.GetComponent<POI>().ConnectedNodes)
                DrawConnections(point.GetComponent<POI>(), connectedPoi);
        }*/


        foreach (POI connectedPoi in startPOI.GetComponent<POI>().ConnectedNodes)
            DrawConnections(startPOI.GetComponent<POI>(), connectedPoi);
        

        GameObject player = Instantiate(Resources.Load("prefab/Capsule"), basePoint + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        GM.SetPlayer(player);
        player.GetComponent<Player>().CurrentPOI = AllPoi[0];



        /*
        ConnectPOIs(AllPoi[0].GetComponent<POI>(), AllPoi[1].GetComponent<POI>());
        ConnectPOIs(AllPoi[1].GetComponent<POI>(), AllPoi[2].GetComponent<POI>());
        ConnectPOIs(AllPoi[2].GetComponent<POI>(), AllPoi[0].GetComponent<POI>());
        */
        /*
        PointGraph pointGraph = AstarPath.active.astarData.AddGraph(typeof(PointGraph)) as PointGraph;
        pointGraph.root = mapTransform;
        pointGraph.raycast = false;
        pointGraph.recursive = false;
        pointGraph.autoLinkNodes = false;
        pointGraph.limits = new Vector3(1f, 0.6f, 1f);

        foreach (GameObject Point in AllPoi)
        {
            PointNode n = pointGraph.AddNode((Int3)Point.transform.position);
            n.Tag = (uint)1000;

            //n.gameObject = Point.gameObject;
            n.Penalty = (uint)1000;
            n.Walkable = true;
            AllPoiNodes.Add(n);
        }

        foreach (PointNode Node in AllPoiNodes)
        {
            foreach (PointNode TNode in AllPoiNodes)
            {
                Node.AddConnection(TNode, (uint)1000);
            }
        }*/
    }

    private static GameObject PlacePoi(float x, float z, float randX, GameObject start)
    {
        Vector3 location = new Vector3(x, z, randX) + new Vector3(x+Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        Collider[] hitColliders = Physics.OverlapSphere(location, 0.5f);
        Debug.Log(hitColliders.Length);
        return Instantiate(Resources.Load("prefab/Cube"), location, Quaternion.identity) as GameObject;
        /*
        if (hitColliders.Length <= 1 && Physics.OverlapSphere(start.transform.position, 0.5f).Length == 0)
            return Instantiate(Resources.Load("prefab/Cube"), location, Quaternion.identity) as GameObject;
        else
            return PlacePoi(location.x, location.z, randX, start);*/
    }

    public void ConnectPOIs(POI start, POI end)
    {
        if (start.walkable == true && end.walkable == true)
        {
            start.AddConnection(end);
            end.AddConnection(start);
        }
    }

    public void DrawConnections(POI start, POI end)
    {
        VectorLine.canvas.sortingOrder = -1;
        List<Vector3> Line = new List<Vector3>();
        Line.Add(new Vector3(start.gameObject.transform.position.x, start.gameObject.transform.position.y, start.gameObject.transform.position.z));
        Line.Add(new Vector3(end.gameObject.transform.position.x, end.gameObject.transform.position.y, end.gameObject.transform.position.z));

        VectorLine myLine = new VectorLine("Line", Line, lineMat, 2);

        myLine.Draw();

        //GenerateLandscape(start, end, 0.5f);
    }

    public void GenerateEvents(POI poi)
    {
        int rnd = Random.Range(0, GM.AllEvents.Count);
        poi.Events.Add(GM.AllEvents[rnd]);
        //Debug.Log((EnumSpace.EventType)rnd);
    }

    public void GenerateLandscape(POI start, POI end, float dist)
    {
        Vector3 dir = end.transform.position - start.transform.position;
        float distance = Vector3.Distance(start.transform.position, end.transform.position);
        Vector3 Point = Vector3.Normalize(start.transform.position + dir * (distance * dist));

        //Vector3 Point = dist * Vector3.Normalize(end.transform.position - start.transform.position) + start.transform.position;
        GameObject obstacle = Instantiate(Resources.Load("prefab/land/Models/naturePack_028"), Point, Quaternion.identity) as GameObject;
        obstacle.transform.localScale.Set(0.3f,0.3f,0.3f) ;
    }
}
