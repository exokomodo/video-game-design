using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Construct a single hallway and its collection of grid cells
/// Author: Geoffrey Roth
/// </summary>
public class Hallway: Object {
    public Vector2Int startPos { get; private set; }
    public Vector2Int endPos { get; private set; }

    public Room startRoom { get; private set; }
    public Room endRoom { get; private set; }
    int width;
    float height;
    Transform rootTransform;
    protected float wallWidth = 0.3f;
    protected Grid2D<Generator2D.CellType> grid;
    protected List<Vector2Int> path;

    public static Grid2D<HallwayCell> HallwayCells;

    public Hallway(
        Grid2D<Generator2D.CellType> grid,
        List<Vector2Int> path,
        Room startRoom,
        Room endRoom,
        int width,
        float height,
        Transform rootTransform,
        GameObject hallPrefab,
        GameObject ceilingPrefab,
        GameObject floorPrefab
    ) {
        this.grid = grid;
        this.path = path;
        this.startRoom = startRoom;
        this.endRoom = endRoom;
        this.width = width;
        this.height = height;
        this.rootTransform = rootTransform;

        for (int i=0; i<path.Count; i++) {
            Vector2Int pos = path[i];
            if (grid[pos] == Generator2D.CellType.Hallway) {
                if (!CellExistsAtPos(pos)) {
                    HallwayCell cell = new HallwayCell(rootTransform, pos, width, height, wallWidth, hallPrefab, ceilingPrefab, floorPrefab, i);
                    HallwayCells[pos] = cell;
                }
            }
        }


    }

    /*
    void PlaceCube(Vector2Int location, Vector2Int size) {
        GameObject go = Instantiate(cellPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, rootTransform);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * .99f, height, size.y * .99f);
    }

    void PlaceHallway(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1));
    }
    */

    private static bool CellExistsAtPos(Vector2Int pos) {
        HallwayCell cell = HallwayCells[pos];
        if (cell?.GetType() == typeof(HallwayCell)) {
            return true;
        }
        return false;
    }

    private static bool WallExistsAtPos(Vector2Int pos, Grid2D<Generator2D.CellType> grid) {
        if (grid.InBounds(pos))
            return grid[pos] != Generator2D.CellType.None;
        return false;
    }

    public void CreateDoorways() {
        for (int i=0; i<path.Count; i++) {
            Vector2Int pos = path[i];
            if (grid[pos] == Generator2D.CellType.Hallway) {
                Vector2Int endPoint = i == 0? path[i] : path[i-1];
                startRoom.RemoveSegment(endPoint);
                break;
            }
        }

        for (int i=path.Count-1; i>0; i--) {
            Vector2Int pos = path[i];
            if (grid[pos] == Generator2D.CellType.Hallway) {
                Vector2Int endPoint = i == path.Count-1? path[i] : path[i+1];
                endRoom.RemoveSegment(endPoint);
                break;
            }
        }
    }

    public static void RemoveWalls(Grid2D<Generator2D.CellType> grid) {
        int xlen = HallwayCells.Size.x;
        int ylen = HallwayCells.Size.y;
        for (int x=0; x<xlen; x++) {
            for (int y=0; y<ylen; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                if (CellExistsAtPos(pos)) {
                    HallwayCell cell = HallwayCells[pos];

                    Vector2Int top = new Vector2Int(pos.x, pos.y + 1);
                    bool removeTop = WallExistsAtPos(top, grid);

                    Vector2Int right = new Vector2Int(pos.x + 1, pos.y);
                    bool removeRight = WallExistsAtPos(right, grid);

                    Vector2Int bot = new Vector2Int(pos.x, pos.y - 1);
                    bool removeBot = WallExistsAtPos(bot, grid);

                    Vector2Int left = new Vector2Int(pos.x -1, pos.y);
                    bool removeLeft = WallExistsAtPos(left, grid);

                    cell.RemoveWalls(removeTop, removeRight, removeBot, removeLeft);
                }
            }
        }
    }
}
