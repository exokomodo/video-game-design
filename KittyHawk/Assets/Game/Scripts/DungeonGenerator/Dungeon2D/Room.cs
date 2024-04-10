using System;
using System.Collections.Generic;
using System.Linq;
using KittyHawk.Extensions;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Construct a single hallway and its collection of grid cells
/// Author: Geoffrey Roth
/// </summary>
public class Room: UnityEngine.Object {
    public RectInt bounds;
    protected GameObject props;

    public static Grid2D<GameObject> Segments;
    public GameObject roomParent;
    protected GameObject wallPrefab;
    protected GameObject floorPrefab;
    protected GameObject ceilingPrefab;
    protected int index;
    protected int height = 1;
    protected float wallWidth = 0.3f;
    protected float scale = 1f;

    protected static string RoomName = "Room";
    protected static string BufferName = "Buffer";

    public bool isStart = false;
    public bool isEnd = false;

    public Vector3 position {
        get {
            return new Vector3(bounds.position.x, 0, bounds.position.y);
        }
    }

    public Vector2Int size {
        get { return bounds.size; }
    }

    public Vector2 center {
        get {
            Vector2Int roomPos = bounds.position;
            Vector2Int roomSize = bounds.size;
            Vector2 roomCenter = roomPos + (Vector2)roomSize/2;
            return new Vector2(roomCenter.x, roomCenter.y);
        }
    }

    public Transform transform {
        get {
            return roomParent.transform;
        }
    }

    public Room(
        Vector2Int location,
        Vector2Int size,
        Transform parent,
        int height,
        GameObject wallPreb,
        GameObject floorPrefab,
        GameObject ceilingPrefab,
        GameObject props,
        float scale,
        int index
    ) {
        bounds = new RectInt(location, size);
        CreateParent(location, parent, RoomName);
        this.height = height;
        this.wallPrefab = wallPreb;
        this.floorPrefab = floorPrefab;
        this.ceilingPrefab = ceilingPrefab;
        this.props = props;
        this.scale = scale;
        this.index = index;

        wallWidth /= scale;
    }

    public Room(Vector2Int location, Vector2Int size, Transform parent) {
        bounds = new RectInt(location, size);
        CreateParent(location, parent, BufferName);
    }

    public static bool Intersect(Room a, Room b) {
        return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
            || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
    }

    public void Build() {
        Transform parent = roomParent.transform;
        Vector3 pos = parent.position;

        Vector2 origin = new Vector2(pos.x, pos.z);
        DrawWallCells(wallPrefab, origin, bounds.size.x);

        origin = new Vector2(pos.x, pos.z + bounds.size.y - wallWidth);
        DrawWallCells(wallPrefab, origin, bounds.size.x);

        origin = new Vector2(pos.x, pos.z);
        DrawWallCells(wallPrefab, origin, 0, bounds.size.y);

        origin = new Vector2(pos.x + bounds.size.x - wallWidth, pos.z);
        DrawWallCells(wallPrefab, origin, 0, bounds.size.y);

        // DrawCeiling();
        DrawFloor();
    }

    private void DrawCeiling() {
        Vector3 pos = roomParent.transform.position;
        pos.y = height - wallWidth;
        GameObject ceiling = Instantiate(ceilingPrefab, pos, Quaternion.identity, roomParent.transform);
        ceiling.transform.localScale = new Vector3(bounds.size.x, wallWidth, bounds.size.y);
    }

    private void DrawFloor() {
        Vector3 pos = roomParent.transform.position;
        GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity, roomParent.transform);
        floor.transform.localScale = new Vector3(bounds.size.x, 0.001f, bounds.size.y);
    }

    public void RemoveSegment(Vector2Int point) {
        GameObject segment = Segments[point];
        // segment?.SetActive(false);
        // Debug.Log("isEnd: " + isEnd);
        if (!isEnd) {
            segment?.SetActive(false);
            return;
        }
        BoxCollider c = segment.GetComponent<BoxCollider>();
        NavMeshObstacle ob = segment.GetComponent<NavMeshObstacle>();
        MeshRenderer r = segment.GetComponent<MeshRenderer>();
        r.enabled = false;
        ob.enabled = false;
        c.isTrigger = true;
        segment.tag = "Finish";
    }

    private void DrawWallCells(GameObject wallPrefab, Vector2 origin, int width=0, int depth=0) {
        Transform parent = roomParent.transform;

        bool horiz = width > depth;
        int total = horiz? width : depth;

        for (int i=0; i<total; i++) {
            Vector3 segmentPos = horiz? new Vector3(origin.x + i, 0, origin.y) : new Vector3(origin.x, 0, origin.y + i);
            GameObject segment = Instantiate(wallPrefab, segmentPos, Quaternion.identity, parent);
            segment.transform.localScale = horiz? new Vector3(1, height, wallWidth) : new Vector3(wallWidth, height, 1);
            segment.tag = "Wall";
            Vector2Int segmentXY = new Vector2Int((int)segmentPos.x, (int)segmentPos.z);
            Segments[segmentXY] = segment;
        }
    }

    private void CreateParent(Vector2Int location, Transform parent, string name) {
        roomParent = new GameObject(name);
        roomParent.transform.parent = parent;
        roomParent.transform.position = new Vector3(location.x, 0, location.y);
    }

    public List<GameObject> GenerateWaypoints() {
        List<GameObject> waypoints = new List<GameObject>();
        GameObject wp = CreateWaypoint(new Vector3(center.x, 0, center.y));
        wp.transform.parent = roomParent.transform;
        waypoints.Add(wp);

        for (int x=0; x<2; x++) {
            for (int y=0; y<2; y++) {
                int newY = Math.Abs(x - y);
                Vector3 offset = new Vector3(x == 0? 1 : -1, 0, newY == 0? 1 : -1) * 2f / scale;
                Vector3 wpos = position + new Vector3(size.x * x, 0, size.y * newY) + offset;
                wp = CreateWaypoint(wpos);
                waypoints.Add(wp);
            }
        }
        return waypoints;
    }

    protected GameObject CreateWaypoint(Vector3 pos) {
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // go.GetComponent<Collider>().enabled = false;
        GameObject go = new GameObject($"Waypoint: {pos}");
        go.transform.parent = roomParent.transform;
        go.transform.localScale = Vector3.one * 1/scale;
        go.transform.position = pos * scale;
        return go;
    }

    protected int GetUniqueRandomInt(int min, int max, List<int> exclude) {
        int maxIterations = 0;
        int value = Random.Range(min, max);
        while (exclude.Contains(value) && ++maxIterations < 10000) {
            value = Random.Range(min, max);
        }
        if (exclude.Contains(value)) {
            throw new Exception("Could not produce unique random int within the given range.");
        }
        exclude.Add(value);
        return value;
    }

    public void PlaceProps() {
        List<int> corners = new List<int>();
        List<int> propIndices = new List<int>();
        for (int i=0; i<4; i++) {
            int corner = GetUniqueRandomInt(0, 4, corners);
            int propIndex = GetUniqueRandomInt(0, props.transform.childCount, propIndices);
            AddProp(corner, props.transform.GetChild(propIndex));
        }
        props.SetActive(false);
    }

    protected void AddProp(int corner, Transform prop) {
        Vector3 pos = position;
        Vector3 offset = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        float ww = wallWidth/2;
        switch (corner) {
            case 0:
                offset = new Vector3(ww, 0, size.y - ww);
                break;
            case 1:
                offset = new Vector3(size.x - ww, 0, size.y - ww);
                rot = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                offset = new Vector3(size.x - ww, 0, ww);
                rot = Quaternion.Euler(0, 180, 0);
                break;
            case 3:
                offset = new Vector3(ww, 0, ww);
                rot = Quaternion.Euler(0, -90, 0);
                break;
            default:
                offset = new Vector3(size.x/2, 0, size.y/2);
                break;
        }
        Transform added = Instantiate(prop, (pos + offset) * scale, rot, roomParent.transform);
        added.localScale *= 1/scale;
    }

    public override string ToString() {
        return $"Room: position: {position}, center: {center}";
    }
}
