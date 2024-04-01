using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Player Controller that manages the Kitty Hawk Player model, finite state machine,
/// animation controller, events, and input controls.
/// Author: Geoffrey Roth
/// </summary>

public class BunnyLevelController : MonoBehaviour {

    [SerializeField]
    Generator2D Generator;

    [SerializeField]
    Rigidbody Player;
    [SerializeField]
    PlayerController PlayerController;

    [SerializeField]
    Bunny bunnyController;

    [SerializeField]
    List<BabyBunny> babyBunnies;

    [SerializeField]
    GameObject GoosePrefab;

    [SerializeField]
    bool Testing = false;

    protected Room startRoom;
    protected Room endRoom;
    protected Vector3 startRoomPos;
    protected Vector3 endRoomPos;

    PlayerInventory inventory;


    private void Awake() {
        if (Testing) {
            Generator.minRoomCount = 4;
            Generator.roomCount = 4;
        }
    }

    private void Start() {
        // Place characters in dungeon
        // EventManager.StartListening<LevelEvent<BabyBunny>, string, BabyBunny>(OnLevelEvent);
        EventManager.StartListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);

        FindStartAndEnd();
        PlacePlayer();
        PlaceBunnies();
        PlaceEnemies();
        PlaceGoal();

        inventory = PlayerController.GetComponent<PlayerInventory>();
        inventory.Bunnies = 0;
        inventory.BunniesTotal = Generator.Rooms.Count - 2;
    }

    private void OnLevelEvent(string eventType, Collider c) {
        if (eventType == LevelEvent<Collider>.END_ROOM_ENTERED) {
            bool hasFollowers = false;
            int followerCount = 0;
            GameObject go = GameObject.FindWithTag("Bunny");
            List<GameObject> waypoints = new List<GameObject>{go};
            for (int i=0; i<babyBunnies.Count; i++) {
                BabyBunny b = babyBunnies[i];
                if (b.followMode) {
                    hasFollowers = true;
                    b.currWaypoint = 0;
                    b.Waypoints = waypoints;
                    b.Patrol();
                    followerCount++;
                }
            }
            if (hasFollowers) {
                inventory.Bunnies += followerCount;
                EventManager.TriggerEvent<AudioEvent, Vector3, string>(go.transform.position, "success1");
            }
        }
    }

    private List<BabyBunny> GetFollowers() {
        List<BabyBunny> followers = new List<BabyBunny>();
        for (int i=0; i<babyBunnies.Count; i++) {
            BabyBunny b = babyBunnies[i];
            if (b.followMode) followers.Add(b);
        }
        return followers;
    }

    private void FixedUpdate() {

    }

    protected void FindStartAndEnd() {
        float low = Generator.size.x + Generator.size.y;
        float high = 0;
        startRoom = null;
        endRoom = null;
        for (int i=0; i<Generator.Rooms.Count; i++) {
            Room r = Generator.Rooms[i];
            Vector2 center = r.center;
            float totalXY = center.x + center.y;
            if (totalXY > high) {
                high = totalXY;
                endRoom = r;
            }
            if (totalXY < low) {
                low = totalXY;
                startRoom = r;
            }
        }
        startRoom.isStart = true;
        endRoom.isEnd = true;
        startRoomPos = GetRoomCenter(startRoom);
        endRoomPos = GetRoomCenter(endRoom);
    }

    private void PlacePlayer() {
        Player.transform.position = startRoomPos;
    }

    private void PlaceBunnies() {
        int limit = Math.Min(Generator.Rooms.Count, babyBunnies.Count);
        // Debug.Log($"PlaceBunnies > limit: {limit}");
        for (int i=0; i<limit; i++) {
            Room room = Generator.Rooms[i];
            // Debug.Log($"PlaceBunnies > room: {room}");
            if (room.position == startRoom.position || room.position == endRoom.position) continue;
            BabyBunny baby = babyBunnies[i];
            // Debug.Log($"PlaceBunnies > i: {i}");
            baby.position = new Vector3(room.center.x, 0, room.center.y);
            baby.Waypoints = room.GenerateWaypoints();
            // Debug.Log($"PlaceBunnies > position: {baby.position}");
        }
    }

    private void PlaceEnemies() {
        int roomCount = Generator.Rooms.Count;
        for (int i=0; i<roomCount; i++) {
            Vector3 pos = GetRoomCenter(Generator.Rooms[i]);
            if (pos == startRoomPos || pos == endRoomPos) continue;
            pos.y = 0.5f;
            GameObject goose = Instantiate(GoosePrefab, pos, Quaternion.identity);
        }
        GoosePrefab.SetActive(false);
    }

    private void PlaceGoal() {
        bunnyController.position = endRoomPos;
    }

    private Vector3 GetRoomCenter(Room room)
    {
        return new Vector3(room.center.x, 0.1f, room.center.y);
    }

    private void OnDestroy() {
        EventManager.StopListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);
    }
}
