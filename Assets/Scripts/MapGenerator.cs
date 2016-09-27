using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Vectrosity;
using System.Linq;
using Delaunay;
using Delaunay.Geo;


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
        //MapGenerate();
        //GenDelaunay();
        MapGenerateDelaunay();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MapGenerateDelaunay()
    {
        Dictionary<Vector2, List<Vector2>> points = GenDelaunay();
        lineMat.color = Color.green;
        mapTransform = GameObject.Find("MapController").transform;
        int id = 0;
        foreach (Vector2 point in points.Keys)
        {
            GameObject poi = Instantiate(Resources.Load("prefab/Cube"), point, Quaternion.identity) as GameObject;
            /*foreach (Vector2 neighbourPoint in points[point])
            {
                GameObject neighbour = Instantiate(Resources.Load("prefab/Cube"), neighbourPoint, Quaternion.identity) as GameObject;
                AllPoi.Add(neighbour);
                if (Vector2.Distance(poi.transform.position, neighbour.transform.position) < 100f)
                {
                    ConnectPOIs(poi.GetComponent<POI>(), neighbour.GetComponent<POI>());
                }
            }*/

            poi.GetComponent<POI>().ID = id;
            GenerateEvents(poi.GetComponent<POI>());
            AllPoi.Add(poi);
            id++;
        }

        GameObject endPOI = AllPoi[8];
        endPOI.GetComponent<Renderer>().material.color = Color.green;
        //endPOI.SetActive(false);
        endPOI.GetComponent<POI>().Events.Clear();
        endPOI.GetComponent<POI>().Victory = true;

        foreach (GameObject poi in AllPoi)
        {
            foreach (Vector2 neighbourPoint in points[poi.transform.position])
            {
                if (Vector2.Distance(poi.transform.position, neighbourPoint) < 5f)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(neighbourPoint, 1f);
                    if (hitColliders.Length > 0 && hitColliders[0].gameObject.GetComponent<POI>().ConnectedNodes.Count <= 5 || hitColliders.Length > 0 && hitColliders[0] == endPOI)
                    {
                        ConnectPOIs(poi.GetComponent<POI>(), hitColliders[0].gameObject.GetComponent<POI>());
                    }
                }
            }
        }

        GameObject startPOI = AllPoi[0];
        startPOI.SetActive(true);
        Debug.Log(startPOI.GetComponent<POI>().ConnectedNodes.Count);

        foreach (GameObject go in AllPoi)
        {
            if (go != startPOI && !startPOI.GetComponent<POI>().ConnectedNodes.Contains(go.GetComponent<POI>()))
            {
                //go.SetActive(false);
            }
        }

        foreach (POI go in startPOI.GetComponent<POI>().ConnectedNodes)
        {
            go.gameObject.SetActive(true);
        }



        foreach (POI connectedPoi in startPOI.GetComponent<POI>().ConnectedNodes)
            if (connectedPoi.ConnectedNodes.Count > 0)
                DrawConnections(startPOI.GetComponent<POI>(), connectedPoi);


        GameObject player = Instantiate(Resources.Load("prefab/Capsule"), AllPoi[0].transform.position, Quaternion.identity) as GameObject;
        GM.SetPlayer(player);
        player.GetComponent<Player>().CurrentPOI = AllPoi[0];
    }


    void MapGenerate()
    {
        /*
        Vector3 basePoint = new Vector3(0f, 0, 0);
        lineMat.color = Color.green;
        mapTransform = GameObject.Find("MapController").transform;
        GameObject startPOI = Instantiate(Resources.Load("prefab/Cube"), basePoint, Quaternion.identity) as GameObject;
        startPOI.GetComponent<POI>().ID = 0;
        AllPoi.Add(startPOI);

        int id = 1;
        float x = 0f;
        float z = 0f;
        for (int i = 0; i < rowCount; i++)
        {
            List<GameObject> smallList = new List<GameObject>();

            for (int j = 0; j < POIcount; j++)
            {
                GameObject poi = Instantiate(Resources.Load("prefab/Cube"), new Vector3(x+Random.Range(-2f, 2f), 0, z+ Random.Range(-2f, 5f)), Quaternion.identity) as GameObject; //PlacePoi(x, 0, startPOI, 0);

                poi.GetComponent<POI>().ID = id;
                GenerateEvents(poi.GetComponent<POI>());
                id++;
                AllPoi.Add(poi);
                smallList.Add(poi);
                if (i != 0)
                    poi.gameObject.SetActive(false);
            }
            All.Add(smallList);
            x += 2.5f;
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

        foreach (List<GameObject> Listpoint in All)
        {
            for (int i = 0; i <= Listpoint.Count-3; i++) //foreach (GameObject point in Listpoint)
            {
                GameObject point = Listpoint[i];
                int conn = point.GetComponent<POI>().ConnectedNodes.Count;
                if (conn > 4 && point != startPOI && point != endPOI)
                {
                    Debug.Log(point.GetComponent<POI>().ConnectedNodes.Count);
                    int remove = Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count - 1);
                    POI randomnode = point.GetComponent<POI>().ConnectedNodes[remove];
                    if (randomnode != endPOI && point != endPOI)
                    {
                        point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    }

                    Debug.Log(point.GetComponent<POI>().ConnectedNodes.Count);
                    remove = Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count - 1);
                    randomnode = point.GetComponent<POI>().ConnectedNodes[remove];
                    if (randomnode != endPOI && point != endPOI)
                    {
                        point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    }

                    Debug.Log(point.GetComponent<POI>().ConnectedNodes.Count);
                    remove = Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count - 1);
                    randomnode = point.GetComponent<POI>().ConnectedNodes[remove];
                    if (randomnode != endPOI && point != endPOI)
                    {
                        point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    }
                } else
                if (conn > 3 && point != startPOI && point != endPOI)
                {
                    Debug.Log(point.GetComponent<POI>().ConnectedNodes.Count);
                    int remove = Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count - 1);
                    POI randomnode = point.GetComponent<POI>().ConnectedNodes[remove];
                    if (randomnode != endPOI && point != endPOI)
                    {
                        point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    }

                    Debug.Log(point.GetComponent<POI>().ConnectedNodes.Count);
                    remove = Random.Range(0, point.GetComponent<POI>().ConnectedNodes.Count - 1);
                    randomnode = point.GetComponent<POI>().ConnectedNodes[remove];
                    if (randomnode != endPOI && point != endPOI)
                    {
                        point.GetComponent<POI>().ConnectedNodes.Remove(randomnode);
                    }
                }
            }
        }

        /*foreach (GameObject point in AllPoi)
        {
            foreach (POI connectedPoi in point.GetComponent<POI>().ConnectedNodes)
                DrawConnections(point.GetComponent<POI>(), connectedPoi);
        }*/

        /*
        foreach (POI connectedPoi in startPOI.GetComponent<POI>().ConnectedNodes)
            if (connectedPoi.ConnectedNodes.Count > 0)
                DrawConnections(startPOI.GetComponent<POI>(), connectedPoi);
        

        GameObject player = Instantiate(Resources.Load("prefab/Capsule"), basePoint + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        GM.SetPlayer(player);
        player.GetComponent<Player>().CurrentPOI = AllPoi[0];
        */


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
    /*
    private static GameObject PlacePoi(float x, float z, float randX, GameObject start, int counter)
    {
        counter += 1;
        Vector3 location = new Vector3(x+Random.Range(0f, 3f), 0, z+Random.Range(-1f, 3f));
        Collider[] hitColliders = Physics.OverlapSphere(location, 2f);
        Debug.Log(hitColliders.Length);
        //return Instantiate(Resources.Load("prefab/Cube"), location, Quaternion.identity) as GameObject;
        if (hitColliders.Length == 0 || counter > 10)
            return Instantiate(Resources.Load("prefab/Cube"), location, Quaternion.identity) as GameObject;
        else
            return PlacePoi(x, z, randX, start, counter);
    }
    */
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

    public Dictionary<Vector2, List<Vector2>> GenDelaunay()
    {
        Dictionary<Vector2, List<Vector2>> points = new Dictionary<Vector2, List<Vector2>>();
        int m_pointCount = 20;

        List<Vector2> m_points;
        float m_mapWidth = 25;
        float m_mapHeight = 12;
        List<LineSegment> m_edges = null;
        List<LineSegment> m_spanningTree;
        List<LineSegment> m_delaunayTriangulation;
        List<uint> colors = new List<uint>();
        m_points = new List<Vector2>();

        for (int i = 0; i < m_pointCount; i++)
        {
            colors.Add(0);
            m_points.Add(new Vector2(
                    Random.Range(2f, m_mapWidth - 2f),
                    Random.Range(2f, m_mapHeight))
            );
        }
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(2f, 2f, m_mapWidth, m_mapHeight));
        m_edges = v.VoronoiDiagram();
        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();
        
        foreach (Vector2 coord in m_points)
        {
            points.Add(coord, v.NeighborSitesForSite(coord));
        }

       return points;
    }

    private void DrawConnections(Vector2 coord, Vector2 vector2)
    {
        VectorLine.canvas.sortingOrder = -1;
        List<Vector3> Line = new List<Vector3>();
        Line.Add(coord);
        Line.Add(vector2);

        VectorLine myLine = new VectorLine("Line", Line, lineMat, 2);

        myLine.Draw();
    }
}
