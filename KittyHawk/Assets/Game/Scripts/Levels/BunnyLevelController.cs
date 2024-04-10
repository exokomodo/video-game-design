#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Bunny Level Controller
/// Author: Geoffrey Roth
/// </summary>
public class BunnyLevelController : MonoBehaviour {

    #if UNITY_EDITOR
        public static bool IsUnityEditor = true;
    #else
        public static bool IsUnityEditor = false;
    #endif

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
    GameObject SmokingDuck;
    [SerializeField]
    GameObject Catnip;

    [SerializeField]
    bool Testing = false;

    protected Room startRoom;
    protected Room endRoom;
    protected Vector3 startRoomPos;
    protected Vector3 endRoomPos;

    PlayerInventory inventory;
    List<DungeonGooseController> geese;

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

        Generate();

        inventory = PlayerController.GetComponent<PlayerInventory>();
        inventory.Bunnies = 0;
        inventory.BunniesTotal = Generator.Rooms.Count - 2;

        Invoke("DuckStartDialogue", 1f);
    }

    public void Generate() {
        // if (IsUnityEditor)
        Generator.Generate();

        FindStartAndEnd();
        PlacePlayer();
        PlaceBunnies();
        PlaceEnemies();
        PlaceGoal();
        Generator.CreateDoorways();
        PlaceObstacles();
    }

    private void DuckStartDialogue() {
        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "BunnyStartDialogue");
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
        DisableGeese();
        Invoke("TriggerLevelCompleteSound", 1.25f);
        Invoke("TriggerBunnyObjective", 4f);

    }

    private void DisableGeese() {
        // Dont allow geese to hurt Kitty

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
        Player.transform.position = new Vector3(startRoomPos.x, 0, startRoomPos.z - startRoom.size.y/2 + 2/Generator.scale) * Generator.scale;
        // startDoor.transform.position =  new Vector3(startRoomPos.x, 0, startRoom.position.z + 0.3f);
        startDoor.SetActive(false);

        SmokingDuck.transform.position = new Vector3(startRoomPos.x, 0, startRoomPos.z - startRoom.size.y/2 + 3/Generator.scale) * Generator.scale;
    }

    private void PlaceBunnies() {
        int limit = Math.Min(Generator.Rooms.Count, babyBunnies.Count);
        for (int i=0; i<limit; i++) {
            Room room = Generator.Rooms[i];
            BabyBunny baby = babyBunnies[i];
            baby.Enable();
            if (room.position == startRoom.position || room.position == endRoom.position) {
                baby.Disable();
                continue;
            };
            Vector3 center = new Vector3(room.center.x, 0, room.center.y) * Generator.scale;
            baby.position = center;
            baby.Waypoints = room.Waypoints;
            Debug.Log("Place bunny: " + center);
        }
    }

    private void PlaceObstacles() {
        List<Room> rooms = Generator.Rooms;
        int roomCount = rooms.Count;
        for (int i=0; i<roomCount; i++) {
            Room r = rooms[i];
            Vector3 pos = GetRoomCenter(r);
            if (pos == startRoomPos || pos == endRoomPos) continue;
                float dx = Random.Range(0.5f, 1f) * Random.Range(0f, 1f) > 0.5? 1 : -1;
                float dz = Random.Range(0.5f, 1f) * Random.Range(0f, 1f) > 0.5? 1 : -1;
                Vector3 newPos = new Vector3(pos.x, 0, pos.z) * Generator.scale + new Vector3(dx, 0, dz);
                GameObject tire = Instantiate(TireStack, newPos, Quaternion.Euler(0, Random.Range(0, 360), 0), r.roomParent.transform);
                tire.transform.localScale *= 1/Generator.scale;
        }
        if (!IsUnityEditor)
            TireStack.SetActive(false);
    }

    private void PlaceEnemies() {
        geese = new List<DungeonGooseController>();
        int roomCount = Generator.Rooms.Count;
        for (int i=0; i<roomCount; i++) {
            Room room = Generator.Rooms[i];
            Vector3 pos = GetRoomCenter(room);
            if (pos == startRoomPos || pos == endRoomPos) continue;
            pos.y = 0.5f;
            GameObject goose = Instantiate(GoosePrefab, pos * Generator.scale, Quaternion.identity, room.roomParent.transform);
            goose.transform.localScale *= 1/Generator.scale;
            DungeonGooseController gc = goose.GetComponent<DungeonGooseController>();
            GameObject[] tmp = new GameObject[room.Waypoints.Count];
            room.Waypoints.CopyTo(tmp);
            gc.Waypoints = new List<GameObject>(tmp);
            gc.Waypoints.Reverse();
            gc.Waypoints.RemoveAt(gc.Waypoints.Count - 1);
            geese.Add(gc);
        }
        // if (!IsUnityEditor)
        GoosePrefab.SetActive(false);
    }

    private void PlaceGoal() {
        bunnyController.position = endRoomPos * Generator.scale;
        // endDoor.transform.position = new Vector3(endRoomPos.x, 0, endRoom.position.z + endRoom.size.y - 0.3f);
        endDoor.SetActive(false);

        Vector3 radius = new Vector3(1, 0, 0);
        int dtheta = 30;
        Vector3 newPos = Vector3.zero;
        for (int i=0; i<360; i+=dtheta) {
            newPos = (endRoomPos * Generator.scale) + Quaternion.AngleAxis(i, Vector3.up) * radius * 3f;
            newPos.y = 0.33f;
            GameObject catnip = Instantiate(Catnip, newPos, Quaternion.identity, endRoom.transform);
            catnip.transform.localScale *= 1/Generator.scale;
        }
        if (!IsUnityEditor)
            Catnip.SetActive(false);
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

#if UNITY_EDITOR
[CustomEditor(typeof(BunnyLevelController))]
public class BunnyLevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BunnyLevelController script = (BunnyLevelController)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
    }
}
#endif
