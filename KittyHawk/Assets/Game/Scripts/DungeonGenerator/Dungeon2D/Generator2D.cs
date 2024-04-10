#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System;

/// <summary>
/// Dungeon Generator adapted from
/// https://github.com/vazgriz/DungeonGenerator
/// Author: Geoffrey Roth
/// </summary>
public class Generator2D : MonoBehaviour {
    public enum CellType {
        None,
        Room,
        Hallway
    }

    [SerializeField]
    public Vector2Int size = new Vector2Int(30, 30);
    [SerializeField]
    public int minRoomCount = 6;

    [SerializeField]
    public int roomCount = 20;
    [SerializeField]
    Vector2Int roomMinSize = new Vector2Int(5, 5);
    [SerializeField]
    Vector2Int roomMaxSize = new Vector2Int(20, 20);
    // [SerializeField]
    // GameObject cellPrefab;
    [SerializeField]
    GameObject hallPrefab;
    [SerializeField]
    GameObject hallFloorPrefab;
    [SerializeField]
    GameObject wallPrefab;
    [SerializeField]
    GameObject floorPrefab;
    [SerializeField]
    GameObject ceilingPrefab;
    [SerializeField]
    GameObject roomProps;

    [SerializeField]
    int RoomHeight = 10;
    [SerializeField]
    float redundantHallwaysPct = 0.125f;

    private int hallWidth = 1;
    [HideInInspector]
    public int retries = 0;
    [SerializeField]
    public int maxRetries = 1000;
    [SerializeField]
    public float scale = 1.0f;

    Random random;
    Grid2D<CellType> grid;
    public List<Room> Rooms { get; private set; }
    public List<Hallway> Hallways { get; private set; }
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    GameObject root;
    private const string rootName = "Generator2DRoot";


    protected void Start() {
        retries = 0;
        Generate();
    }

    public void Generate() {
        // Debug.Log($"retries: {retries}");
        if (++retries >= maxRetries) {
            Debug.LogWarning("Could not generate a dungeon with the given parameters in the available number of retries. Please adjust generation settings and try again.");
            return;
        }
        random = new Random();
        grid = new Grid2D<CellType>(size, Vector2Int.zero);
        Rooms = new List<Room>();
        Hallways = new List<Hallway>();
        Room.Segments = new Grid2D<GameObject>(size, Vector2Int.zero);
        Hallway.HallwayCells = new Grid2D<HallwayCell>(size, Vector2Int.zero);

        GameObject r = GameObject.Find(rootName);
        if (r != null) DestroyImmediate(r);

        if (root != null) DestroyImmediate(root);
        root = new GameObject(rootName);


        PlaceRooms();
        if (Rooms.Count < minRoomCount) {
            // Debug.Log($"Rooms: {Rooms.Count}, {roomCount}");
            Generate();
            return;
        }
        try {
            Triangulate();
            CreateHallways();
            PathfindHallways();
        } catch (Exception e) {
            Debug.LogWarning(e);
            Generate();
            return;
        }

        // Debug.Log($"Rooms: {Rooms.Count}, Hallways: {Hallways.Count}");
        if (Hallways.Count < Rooms.Count-1) {
            Generate();
            return;
        }
        Hallway.RemoveWalls(grid);
        root.transform.localScale = Vector3.one * scale;
        PlaceProps();



        Debug.Log($"Final RoomCount: {Rooms.Count}");
    }

    public void CreateDoorways() {
        for (int i=0; i<Hallways.Count; i++) {
            Hallway h = Hallways[i];
            h.CreateDoorways();
        }
    }

    protected void PlaceRooms() {
        for (int i = 0; i < 1000; i++) {
            // Debug.Log("PlaceRooms:" + i);
            Vector2Int location = new Vector2Int(
                random.Next(0, size.x),
                random.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(
                location,
                roomSize,
                root.transform,
                RoomHeight,
                wallPrefab,
                floorPrefab,
                ceilingPrefab,
                roomProps,
                scale,
                i
            );
            Room buffer = new Room(location + new Vector2Int(-hallWidth, -hallWidth), roomSize + new Vector2Int(hallWidth * 2, hallWidth * 2), root.transform);

            foreach (var room in Rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }

            if (add) {
                Rooms.Add(newRoom);
                newRoom.Build();
                // PlaceCube(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
                if (Rooms.Count >= roomCount) return;
            }
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in Rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < redundantHallwaysPct) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);

        foreach (Prim.Edge edge in selectedEdges) {
            Room startRoom = (edge.U as Vertex<Room>).Item;
            Room endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            List<Vector2Int> path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room) {
                    pathCost.cost += 10;
                } else if (grid[b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    Vector2Int current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }
                }

                Hallways.Add(new Hallway(
                    grid,
                    path,
                    startRoom,
                    endRoom,
                    hallWidth,
                    RoomHeight,
                    root.transform,
                    hallPrefab,
                    ceilingPrefab,
                    hallFloorPrefab,
                    scale
                ));
            }
        }
    }

    // void PlaceCube(Vector2Int location, Vector2Int size) {
    //     GameObject go = Instantiate(cellPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, root.transform);
    //     go.GetComponent<Transform>().localScale = new Vector3(size.x, RoomHeight, size.y);
    // }

    // void PlaceHallway(Vector2Int location) {
    //     PlaceCube(location, new Vector2Int(1, 1));
    // }

    protected void PlaceProps() {
        foreach (Room room in Rooms) {
            room.PlaceProps(scale);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Generator2D))]
public class RoomGraphGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Generator2D script = (Generator2D)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            script.retries = 0;
            script.Generate();
        }
    }
}
#endif

