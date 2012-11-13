using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class LevelLoader : MonoBehaviour {
    public int WIDTH = 10;
    public int HEIGHT = 10;
    private Level level;

    // Use this for initialization
    void Awake () {
        this.level = this.CreateLevel (0);
        this.CreateRooms();
    }
    
    // Update is called once per frame
    void Update () {
    }
    
    private Level CreateLevel (int levelNo) {
        Dictionary<Vector2, char> map = new Dictionary<Vector2, char> ();
        TextAsset asset = (TextAsset)Resources.Load ("Levels/Level" + levelNo.ToString (), typeof(TextAsset));
        GameObject levelObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        levelObject.renderer.enabled = false;
        levelObject.name = "Level" + levelNo;
        string[] lines = asset.text.Split ('\n');
        int width = 0;
        int height = lines.Length;
        for (int y = 0; y < lines.Length; ++y) {
            string line = lines [y];
            for (int x = 0; x < line.Length; ++x) {
                if (line.Length > width)
                    width = line.Length;
                char c = line [x];
                map.Add (new Vector2 (x, y), c);
            }
        }
        Level level = new Level (levelNo, width, height, map);
        foreach (Vector2 key in map.Keys) {
            Vector3 position = new Vector3 (key.x * WIDTH, 0, -key.y * HEIGHT);
            int x = (int)key.x;
            int y = (int)key.y;
            char c = level.GetMap(x, y);
            if (c == '.' || c == 'S') {
                GameObject floorPrefab = (GameObject)Resources.Load ("Prefabs/floorPrefab", typeof(GameObject));
                GameObject floor = (GameObject)Instantiate (floorPrefab, position, Quaternion.identity);
                floor.transform.parent = levelObject.transform;
            }
            if (c == '/') {
                GameObject routePrefab = (GameObject)Resources.Load ("Prefabs/routePrefab", typeof(GameObject));
                GameObject route = (GameObject)Instantiate (routePrefab, position, Quaternion.identity);
                route.transform.parent = levelObject.transform;
                // if route is placed holizontally, rotate object.
                if (level.GetMap (x - 1, y) == '/' || level.GetMap (x + 1, y) == '/') {
                    route.transform.Rotate (new Vector3 (0, 90, 0));
                }
            } else if (c == '#') {
                GameObject wallPrefab = (GameObject)Resources.Load ("Prefabs/wallPrefab", typeof(GameObject));
                GameObject wall = (GameObject)Instantiate (wallPrefab, position + Vector3.up * 1, Quaternion.identity);
                wall.transform.parent = levelObject.transform;
            } else if (c == 'S') {
                GameObject player = GameObject.FindWithTag ("Player");
                player.transform.position = position + Vector3.up;
            }
        }
        GameObject aStar = GameObject.Find("A*");
        AstarPath path = aStar.GetComponent<AstarPath>();
        PointGraph graph = path.graphs[0] as PointGraph;
        //graph.root = levelObject.transform;
        graph.ScanGraph();
        Debug.Log (graph.nodes.Length);
        return level;
    }
    
    private void CreateRooms () {
        for (int x = 0; x < this.level.GetWidth(); ++x) {
            for (int y = 0; y < this.level.GetHeight(); ++y) {
                if (this.level.IsFloor(x, y)) {
                    if ( !level.ContainsInRooms(x, y) ) {
                        Room room = new Room();
                        this.level.AddRoom(room);
                        this.AddNeighborFloor(room, x, y);
                    }
                }
            }
        }
    }
    
    private void AddNeighborFloor (Room room, int x, int y) {
        room.AddFloor(x, y);
        int[] xs = {x + 1, x - 1, x, x};
        int[] ys = {y, y, y + 1, y - 1};
        for (int i = 0; i < 4; ++i) {
            if (this.level.IsFloor(xs[i], ys[i]) && !this.level.ContainsInRooms(xs[i], ys[i])) {
                this.AddNeighborFloor(room, xs[i], ys[i]);
            }
        }
    }
}
