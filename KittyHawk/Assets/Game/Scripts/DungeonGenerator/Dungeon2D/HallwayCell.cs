using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Construct a single hallway cell
/// Author: Geoffrey Roth
/// </summary>
public class HallwayCell: Object {
    public int index { get; private set; }
    Transform rootTransform;
    GameObject hallPrefab;
    GameObject ceilingPrefab;
    GameObject floorPrefab;
    Vector2Int position;
    int width;
    float cellWidth;
    float height;
    float wallWidth;
    float scale;
    GameObject topWall;
    GameObject rightWall;
    GameObject botWall;
    GameObject leftWall;
    GameObject hallParent;


    public HallwayCell(
        Transform rootTransform,
        Vector2Int position,
        int width,
        float height,
        float wallWidth,
        GameObject hallPrefab,
        GameObject ceilingPrefab,
        GameObject floorPrefab,
        int index
    ) {
        this.rootTransform = rootTransform;
        this.hallPrefab = hallPrefab;
        this.ceilingPrefab = ceilingPrefab;
        this.floorPrefab = floorPrefab;
        this.position = position;
        this.width = width;
        this.cellWidth = (float)width;
        this.height = height;
        this.wallWidth = wallWidth;
        this.index = index;
        Render();
    }

    public void RemoveWalls(bool top, bool right, bool bot, bool left) {
        if (top) topWall.SetActive(false);
        if (right) rightWall.SetActive(false);
        if (bot) botWall.SetActive(false);
        if (left) leftWall.SetActive(false);
    }

    private GameObject CreateParent(Vector2Int location, int index) {
        GameObject cellParent = new GameObject($"Hallway{index}");
        cellParent.transform.parent = rootTransform;
        cellParent.transform.position = new Vector3(location.x, 0, location.y);
        return cellParent;
    }

    public void Render() {
        Vector2Int pos = position;
        hallParent = CreateParent(pos, index);
        Transform t = hallParent.transform;

        float y = 0;

        topWall = Instantiate(hallPrefab, new Vector3(pos.x, y, pos.y + cellWidth), Quaternion.identity, t);
        topWall.transform.localScale = new Vector3(1, height, wallWidth);
        topWall.tag = "Wall";

        rightWall = Instantiate(hallPrefab, new Vector3(pos.x + cellWidth, y, pos.y), Quaternion.identity, t);
        rightWall.transform.localScale = new Vector3(wallWidth, height, cellWidth);
        rightWall.tag = "Wall";

        botWall = Instantiate(hallPrefab, new Vector3(pos.x, y, pos.y - wallWidth), Quaternion.identity, t);
        botWall.transform.localScale = new Vector3(1, height, wallWidth);
        botWall.tag = "Wall";

        leftWall = Instantiate(hallPrefab, new Vector3(pos.x - wallWidth, y, pos.y), Quaternion.identity, t);
        leftWall.transform.localScale = new Vector3(wallWidth, height, cellWidth);
        leftWall.tag = "Wall";

        // DrawCeiling();
        DrawFloor();
    }

    private void DrawCeiling() {
        Vector3 pos = hallParent.transform.position;
        pos.y = height - wallWidth;
        GameObject ceiling = Instantiate(ceilingPrefab, pos, Quaternion.identity, hallParent.transform);
        ceiling.transform.localScale = new Vector3(cellWidth, wallWidth, cellWidth);
    }

    private void DrawFloor() {
        Vector3 pos = hallParent.transform.position;
        GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity, hallParent.transform);
        floor.transform.localScale = new Vector3(cellWidth, 0.001f, cellWidth);
    }

    public override string ToString() {
        return $"HallwayCell: {position}";
    }
}
