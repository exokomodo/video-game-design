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


    private void Start() {
        // Place characters in dungeon
        PlaceCharacters();
        PlaceEnemies();
        PlaceGoal();
    }

    private void FixedUpdate() {

    }

    private void PlaceCharacters() {
        Vector3 pos = GetRoomCenter(Generator.Rooms[0]);
        Player.transform.position = pos;
        Bunny.transform.position = pos;
        BunnyBabies.transform.position = pos;
    }

    private void PlaceEnemies() {
        int roomCount = Generator.Rooms.Count;
        for (int i=1; i<roomCount-1; i++) {
            Vector3 pos = GetRoomCenter(Generator.Rooms[i]);
            pos.y = 0.5f;
            GameObject goose = Instantiate(GoosePrefab, pos, Quaternion.identity);
        }
    }

    private void PlaceGoal() {
        Vector3 pos = GetRoomCenter(Generator.Rooms[Generator.Rooms.Count-1]);
        // BunnyBabies.transform.position = pos;
    }

    private Vector3 GetRoomCenter(Room room)
    {
        RectInt roomBounds = room.bounds;
        Vector2Int roomPos = roomBounds.position;
        Vector2Int roomSize = roomBounds.size;
        Vector2 roomCenter = roomPos + roomSize / 2;
        return new Vector3(roomCenter.x, 0.1f, roomCenter.y);
    }
}
