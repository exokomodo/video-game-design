using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Bunny Level Controller
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
    GameObject startDoor;
    [SerializeField]
    GameObject endDoor;

    [SerializeField]
    GameObject TireStack;

    [SerializeField]
    bool Testing = false;

    protected Room startRoom;
    protected Room endRoom;
    protected Vector3 startRoomPos;
    protected Vector3 endRoomPos;

    PlayerInventory inventory;

    string ObjectiveName = "BunnyObjective";


    private void Awake() {
        if (Testing) {
            Generator.minRoomCount = 4;
            Generator.roomCount = 4;
        }
    }

    private void Start() {
        // Place characters in dungeon
        EventManager.StartListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);
        EventManager.StartListening<PlayerDeathEvent>(OnPlayerDie);

        FindStartAndEnd();
        PlacePlayer();
        PlaceBunnies();
        PlaceEnemies();
        PlaceGoal();
        Generator.CreateDoorways();
        PlaceObstacles();

        inventory = PlayerController.GetComponent<PlayerInventory>();
        inventory.Bunnies = 0;
        inventory.BunniesTotal = Generator.Rooms.Count - 2;
    }

    private void OnLevelEvent(string eventType, Collider c) {
        if (eventType == LevelEvent<Collider>.BUNNY_COLLIDER_ENTERED) {
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
                EventManager.TriggerEvent<AudioEvent, Vector3, string>(Player.transform.position, "success1");
                inventory.Bunnies += followerCount;
                if (inventory.Bunnies == inventory.BunniesTotal) {
                    LevelComplete();
                }

            }
        }
    }

    private void LevelComplete() {
        Debug.Log("YOU WIN!!");
        Invoke("TriggerLevelCompleteSound", 1.25f);
        Invoke("TriggerBunnyObjective", 4f);

    }

    private void TriggerLevelCompleteSound() {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(Player.transform.position, "success-fanfare-trumpets");
    }

    private void TriggerBunnyObjective() {
        Debug.Log("BunnyObjective ObjectiveStatus.Completed");
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(ObjectiveName, ObjectiveStatus.Completed);
    }

    private List<BabyBunny> GetFollowers() {
        List<BabyBunny> followers = new List<BabyBunny>();
        for (int i=0; i<babyBunnies.Count; i++) {
            BabyBunny b = babyBunnies[i];
            if (b.followMode) followers.Add(b);
        }
        return followers;
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
        Player.transform.position = new Vector3(startRoomPos.x, 0, startRoomPos.z - startRoom.size.y/2 + 2);
        startDoor.transform.position =  new Vector3(startRoomPos.x, 0, startRoom.position.z + 0.3f);
    }

    private void PlaceBunnies() {
        int limit = Math.Min(Generator.Rooms.Count, babyBunnies.Count);
        for (int i=0; i<limit; i++) {
            Room room = Generator.Rooms[i];
            BabyBunny baby = babyBunnies[i];
            if (room.position == startRoom.position || room.position == endRoom.position) {
                baby.Disable();
                continue;
            };

            Vector3 center = new Vector3(room.center.x, 0, room.center.y);
            baby.position = center;
            baby.Waypoints = room.GenerateWaypoints();
        }
    }

    private void PlaceObstacles() {
        List<Room> rooms = Generator.Rooms;
        int roomCount = rooms.Count;
        for (int i=0; i<roomCount; i++) {
            Vector3 pos = GetRoomCenter(rooms[i]);
            if (pos == startRoomPos || pos == endRoomPos) continue;
            // if (Random.Range(0f, 1f) > 0.1f) {
                GameObject tire = Instantiate(TireStack, new Vector3(pos.x + Random.Range(-1,1), 0, pos.z + Random.Range(-1,1)), Quaternion.Euler(0, Random.Range(0, 360), 0));
            // }
        }
        TireStack.SetActive(false);
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
        endDoor.transform.position = new Vector3(endRoomPos.x, 0, endRoom.position.z + endRoom.size.y - 0.3f);
    }

    private Vector3 GetRoomCenter(Room room)
    {
        return new Vector3(room.center.x, 0.1f, room.center.y);
    }

    private void OnPlayerDie() {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                ObjectiveName,
                ObjectiveStatus.Failed);
    }

    private void OnDestroy() {
        EventManager.StopListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);
        EventManager.StopListening<PlayerDeathEvent>(OnPlayerDie);
    }
}
