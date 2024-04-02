using System.Collections.Generic;
using UnityEngine;

public class GeoffSandboxLevelController : MonoBehaviour {

    [SerializeField]
    public Bunny bunny;
    public GameObject kitty;
    List<GameObject> Waypoints;
    float timer = 0;

    public void Start() {
        CreateWaypoints();
    }

    protected void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > 20) {
            timer = 0;
            if (bunny.followMode) {
                bunny.Patrol();
            } else {
                bunny.Follow(kitty);
            }
        }
    }

    private void CreateWaypoints() {
        Waypoints = new List<GameObject>();
        for (int i=0; i<4; i++) {
            float x = Random.Range(0, 20);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(x, 0.5f, 5*i);
            cube.GetComponent<BoxCollider>().enabled = false;
            Waypoints.Add(cube);
        }
        bunny.Waypoints = Waypoints;
        bunny.Patrol();
    }

}
