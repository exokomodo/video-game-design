using UnityEngine;
/// <summary>
/// A Player Controller that manages the Kitty Hawk Player model, finite state machine,
/// animation controller, events, and input controls.
/// Author: Geoffrey Roth
/// </summary>
///


public class LevelController : MonoBehaviour {

    [SerializeField]
    Generator2D Generator;

    [SerializeField]
    Rigidbody Player;

    [SerializeField]
    GameObject Bunny;

    [SerializeField]
    GameObject BunnyBabies;

    [SerializeField]
    GameObject GoosePrefab;

    protected Vector3 startRoomPos;
    protected Vector3 endRoomPos;


    private void Start() {
        // Place characters in dungeon
        FindStartAndEnd();
        PlaceCharacters();
        PlaceEnemies();
        PlaceGoal();
    }

    private void FixedUpdate() {

    }

    protected void FindStartAndEnd() {
        float low = Generator.size.x + Generator.size.y;
        float high = 0;
        Room start = null;
        Room end = null;
        for (int i=0; i<Generator.Rooms.Count; i++) {
            Room r = Generator.Rooms[i];
            Vector2 center = r.center;
            float totalXY = center.x + center.y;
            if (totalXY > high) {
                high = totalXY;
                end = r;
            }
            if (totalXY < low) {
                low = totalXY;
                start = r;
            }
        }
        startRoomPos = GetRoomCenter(start);
        endRoomPos = GetRoomCenter(end);
    }

    private void PlaceCharacters() {
        Player.transform.position = startRoomPos;
        Bunny.transform.position = startRoomPos;
        // BunnyBabies.transform.position = startRoomPos;
    }

    private void PlaceEnemies() {
        int roomCount = Generator.Rooms.Count;
        for (int i=0; i<roomCount; i++) {
            Vector3 pos = GetRoomCenter(Generator.Rooms[i]);
            if (pos == startRoomPos || pos == endRoomPos) continue;
            pos.y = 0.5f;
            GameObject goose = Instantiate(GoosePrefab, pos, Quaternion.identity);
        }
    }

    private void PlaceGoal() {
        BunnyBabies.transform.position = endRoomPos;
    }

    private Vector3 GetRoomCenter(Room room)
    {
        return new Vector3(room.center.x, 0.1f, room.center.y);
    }
}
