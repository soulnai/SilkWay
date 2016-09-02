using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Vectrosity;

public class MapGenerator : MonoBehaviour {

    public Transform mapTransform;
    public List<GameObject> AllPoi = new List<GameObject>();
    public List<PointNode> AllPoiNodes = new List<PointNode>();

    public Material lineMat;

    public int POIcount = 9;
    // Use this for initialization
    void Start () {
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
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), basePoint, Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(1.5f, 0, 2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(1.5f, 0, 0), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(1.5f, 0, -2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(3f, 0, 2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(3f, 0, 0), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(3f, 0, -2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(4.5f, 0, 2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(4.5f, 0, 0), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(4.5f, 0, -2.5f), Quaternion.identity) as GameObject);
        AllPoi.Add(Instantiate(Resources.Load("prefab/Cube"), new Vector3(6f, 0, 0), Quaternion.identity) as GameObject);

        AllPoi[4].GetComponent<POI>().SetWalkableFalse();
        AllPoi[2].GetComponent<POI>().SetWalkableFalse();
        AllPoi[6].GetComponent<POI>().SetWalkableFalse();


        for (int i = 0; i < AllPoi.Count; i++)
        {
            if (i + 1 <= AllPoi.Count - 1)
            {
                ConnectPOIs(AllPoi[i].GetComponent<POI>(), AllPoi[i + 1].GetComponent<POI>());
                if (i + 2 <= AllPoi.Count - 1)
                {
                    ConnectPOIs(AllPoi[i].GetComponent<POI>(), AllPoi[i + 2].GetComponent<POI>());
                    if (i + 3 <= AllPoi.Count - 1)
                        ConnectPOIs(AllPoi[i].GetComponent<POI>(), AllPoi[i + 3].GetComponent<POI>());
                }
            }
        }

        foreach (GameObject point in AllPoi)
        {
            foreach (POI connectedPoi in point.GetComponent<POI>().ConnectedNodes)
                DrawConnections(point.GetComponent<POI>(), connectedPoi);
        }
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
        List<Vector3> Line = new List<Vector3>();
        Line.Add(new Vector3(start.gameObject.transform.position.x, start.gameObject.transform.position.y, start.gameObject.transform.position.z));
        Line.Add(new Vector3(end.gameObject.transform.position.x, end.gameObject.transform.position.y, end.gameObject.transform.position.z));

        VectorLine myLine = new VectorLine("Line", Line, lineMat, 2);

        myLine.Draw();
    }
}
