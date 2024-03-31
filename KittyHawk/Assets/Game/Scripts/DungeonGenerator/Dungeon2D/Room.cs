using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Construct a single hallway and its collection of grid cells
/// Author: Geoffrey Roth
/// </summary>
public class Room: UnityEngine.Object {
    public RectInt bounds;

    public static Grid2D<GameObject> Segments;
    protected GameObject roomParent;
    protected GameObject wallPrefab;
    protected GameObject floorPrefab;
    protected GameObject ceilingPrefab;
    protected int index;
    protected int height = 1;
    protected float wallWidth = 0.3f;

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

    public Room(
        Vector2Int location,
        Vector2Int size,
        Transform parent,
        int height,
        GameObject wallPreb,
        GameObject floorPrefab,
        GameObject ceilingPrefab,
        int index
    ) {
        bounds = new RectInt(location, size);
        CreateParent(location, parent, RoomName);
        this.wallPrefab = wallPreb;
        this.floorPrefab = floorPrefab;
        this.ceilingPrefab = ceilingPrefab;
        this.index = index;
        this.height = height;
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
        BoxCollider c = segment.GetComponent<BoxCollider>();
        NavMeshObstacle ob = segment.GetComponent<NavMeshObstacle>();
        MeshRenderer r = segment.GetComponent<MeshRenderer>();
        r.enabled = false;
        ob.enabled = false;
        c.isTrigger = true;
        segment.tag = "Door";
    }

    private void DrawWallCells(GameObject wallPrefab, Vector2 origin, int width=0, int depth=0) {
        Transform parent = roomParent.transform;

        bool horiz = width > depth;
        int total = horiz? width : depth;

        for (int i=0; i<total; i++) {
            Vector3 segmentPos = horiz? new Vector3(origin.x + i, 0, origin.y) : new Vector3(origin.x, 0, origin.y + i);
            GameObject segment = Instantiate(wallPrefab, segmentPos, Quaternion.identity, parent);
            segment.transform.localScale = horiz? new Vector3(1, height, wallWidth) : new Vector3(wallWidth, height, 1);
            // segment.tag = "Wall";
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
        waypoints.Add(wp);
        for (int x=0; x<2; x++) {
            for (int y=0; y<2; y++) {
                int newY = Math.Abs(x - y);
                Vector3 offset = new Vector3(x == 0? 1 : -1, 0, newY == 0? 1 : -1);
                wp = CreateWaypoint(position + new Vector3(size.x * x, 0, size.y * newY) + offset);
                waypoints.Add(wp);
            }
        }
        return waypoints;
    }

    protected GameObject CreateWaypoint(Vector3 pos) {
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // go.GetComponent<Collider>().enabled = false;
        GameObject go = new GameObject($"Waypoint: {pos}");
        go.transform.localScale = new Vector3(1f, 1, 1f);
        go.transform.position = pos;
        Debug.Log(go.name);
        return go;
    }

    public override string ToString() {
        return $"Room: position: {position}, center: {center}";
    }
}
