using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class LevelManager : MonoBehaviour {
    public int WIDTH = 10;
    public int HEIGHT = 10;
    public int initialLevel = 0;
    private GameObject levelObject = null;
    private Level level;

    // Use this for initialization
    void Awake () {
        this.level = this.CreateLevel (this.initialLevel);
        this.CreateRooms();
        this.CreateRoutes();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            this.level.AddStartPoint(PositionToMatrix(player.transform.position));
        }
    }
    
    // Update is called once per frame
    void Update () {
    }
    
    private Level CreateLevel (int levelNo) {
        Dictionary<Vector2, char> map = new Dictionary<Vector2, char> ();
        TextAsset asset = (TextAsset)Resources.Load ("Levels/Level" + levelNo.ToString (), typeof(TextAsset));
        levelObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
        Level level = new Level (levelNo, width, height);
        foreach (Vector2 key in map.Keys) {
            Vector3 position = this.MatrixToPosition(key);
            int x = (int)key.x;
            int y = (int)key.y;
            Vector2 p = new Vector2(x, y);
            char c = map[p];
            if (c != ' ' && c != '/' && c != '#') {
                GameObject floorPrefab = (GameObject)Resources.Load ("Prefabs/floorPrefab", typeof(GameObject));
                GameObject floor = (GameObject)Instantiate (floorPrefab, position, Quaternion.identity);
                floor.transform.parent = levelObject.transform;
                level.SetObject(p, floor);
            }
            if (c == '/') {
                GameObject routePrefab = (GameObject)Resources.Load ("Prefabs/routePrefab", typeof(GameObject));
                GameObject route = (GameObject)Instantiate (routePrefab, position, Quaternion.identity);
                route.transform.parent = levelObject.transform;
                // if route is placed holizontally, rotate object.
                Vector2 left = new Vector2(x - 1, y);
                Vector2 right = new Vector2(x + 1, y);
                if ((map.ContainsKey(left) && map[left] == '/') || (map.ContainsKey(right) && map[right] == '/')) {
                    route.transform.Rotate (new Vector3 (0, 90, 0));
                }
                level.SetObject(p, route);
            } else if (c == '#') {
                GameObject wallPrefab = null;
                GameObject wall = null;
                Vector2 up = new Vector2(x - 1, y);
                Vector2 left = new Vector2(x - 1, y);
                Vector2 right = new Vector2(x + 1, y);
                if ((map.ContainsKey(left) && map[left] == '.') ^
                    (map.ContainsKey(right) && map[right] == '.')) {
                    wallPrefab = (GameObject)Resources.Load ("Prefabs/curveWallPrefab", typeof(GameObject));
                    wall = (GameObject)Instantiate (wallPrefab, position, Quaternion.identity);
                    if ((map.ContainsKey(left) && map[left] == '.')) {
                        wall.transform.Rotate (new Vector3 (0, 180, 0));
                    }
                } else {
                    wallPrefab = (GameObject)Resources.Load ("Prefabs/wallPrefab", typeof(GameObject));
                    wall = (GameObject)Instantiate (wallPrefab, position + Vector3.up * 6, Quaternion.identity);
                }
                wall.transform.parent = levelObject.transform;
                level.SetObject(p, wall);
            } else if (c == 'S') {
                GameObject player = GameObject.FindWithTag ("Player");
                player.transform.position = position + Vector3.up * 5;
            } else if (c == '*') {
                GameObject enemyPrefab = (GameObject)Resources.Load ("Prefabs/enemyPrefab", typeof(GameObject));
                GameObject enemy = (GameObject)Instantiate (enemyPrefab, position + Vector3.up * 1, Quaternion.identity);
                enemy.transform.parent = levelObject.transform;
            }
        }
        AstarPath path = AstarPath.active;
        PointGraph graph = path.graphs[0] as PointGraph;
        graph.Scan();
        return level;
    }
    
    private void CreateRooms () {
        for (int x = 0; x < this.level.GetWidth(); ++x) {
            for (int y = 0; y < this.level.GetHeight(); ++y) {
                if (this.level.IsFloor(x, y)) {
                    if ( !level.ContainsInRooms(x, y) ) {
                        Room room = new Room();
                        this.level.AddRoom(room);
                        this.AddNeighborRoom(room, x, y);
                        Vector3 center = this.MatrixToPosition(room.GetCenter());
                        GameObject lightPrefab = (GameObject)Resources.Load ("Prefabs/roomLightPrefab", typeof(GameObject));
                        GameObject light = (GameObject)Instantiate (lightPrefab, center + Vector3.up * 10, Quaternion.identity);
                        light.transform.parent = levelObject.transform;
                    }
                }
            }
        }
    }
    
    private void CreateRoutes () {
        for (int x = 0; x < this.level.GetWidth(); ++x) {
            for (int y = 0; y < this.level.GetHeight(); ++y) {
                if (this.level.IsRoute(x, y)) {
                    if ( !level.ContainsInRoutes(x, y) ) {
                        Route route = new Route();
                        this.level.AddRoute(route);
                        this.AddNeighborRoute(route, x, y);
                        Vector3 center = this.MatrixToPosition(route.GetCenter());
                        GameObject lightPrefab = (GameObject)Resources.Load ("Prefabs/routeLightPrefab", typeof(GameObject));
                        GameObject light = (GameObject)Instantiate (lightPrefab, center + Vector3.up * 3, Quaternion.identity);
                        light.transform.parent = levelObject.transform;
                    }
                }
            }
        }
    }
       
    private void AddNeighborRoom (Room room, int x, int y) {
        room.AddFloor(x, y);
        int[] xs = {x + 1, x - 1, x, x};
        int[] ys = {y, y, y + 1, y - 1};
        for (int i = 0; i < 4; ++i) {
            if (this.level.IsFloor(xs[i], ys[i]) && !this.level.ContainsInRooms(xs[i], ys[i])) {
                this.AddNeighborRoom(room, xs[i], ys[i]);
            }
        }
    }
    
    private void AddNeighborRoute (Route route, int x, int y) {
        route.AddFloor(x, y);
        int[] xs = {x + 1, x - 1, x, x};
        int[] ys = {y, y, y + 1, y - 1};
        for (int i = 0; i < 4; ++i) {
            Vector2 v = new Vector2(xs[i], ys[i]);
            if (this.level.IsRoute(xs[i], ys[i]) && !this.level.ContainsInRoutes(xs[i], ys[i])) { 
                this.AddNeighborRoute(route, xs[i], ys[i]);
            } else if (this.level.IsFloor(xs[i], ys[i])) {
                Room room = this.level.GetRoom(v);
                if (room != null) {
                    route.AddRoom(new Vector2(x, y), room);
                }
            }
        }
    }
    
    private Vector2 PositionToMatrix (Vector3 position) {
        float x = position.x;
        float y = position.z;
        return new Vector2(Mathf.Floor(x / this.WIDTH), Mathf.Floor(-y / this.HEIGHT));
    }
    
    private Vector3 MatrixToPosition (Vector2 matrix) {
        return new Vector3 (matrix.x * WIDTH, 0, -matrix.y * HEIGHT);
    }
    
    public void BombRoom (Vector3 position) {
        Vector2 p = this.PositionToMatrix (position);
        Room bombRoom = this.level.GetRoom(p);
        if (bombRoom != null) {
            this.DestroyRoom (bombRoom);
        }
    }
    
    public void DestroyRoom(Room room) {
        foreach (Vector2 point in room.GetFloors()) {
            GameObject obj = this.level.GetObject(point);
            foreach (GameObject neighbor in this.level.GetNeighbors(point, false)) {
                if (neighbor.CompareTag("Wall")) {
                    this.level.RemoveObject(this.PositionToMatrix(neighbor.transform.position));
                    neighbor.rigidbody.isKinematic = false;
                    neighbor.rigidbody.useGravity = true;
                }
            }
            this.level.RemoveObject(point);
            if (obj) {
                obj.rigidbody.isKinematic = false;
                obj.rigidbody.useGravity = true;
            }
        }
        this.UpdatePath(room); 
        this.level.DisableRoom(room);
        foreach (Route route in this.level.GetRoutes()) {
            bool bombRoute = true;
            foreach (Vector2 pos in route.GetRooms().Keys) {
                Room r = route.GetRooms()[pos];
                if (r.GetEnable()) {
                    bombRoute = false;
                }
                if (r == room) {
                    GameObject shutterPrefab = (GameObject)Resources.Load("Prefabs/shutterPrefab", typeof(GameObject)); 
                    GameObject routeTile = this.level.GetObject(pos);
                    GameObject shutter = (GameObject)Instantiate(shutterPrefab, routeTile.transform.position, Quaternion.identity);
                    shutter.transform.parent = routeTile.transform;
                    shutter.transform.localPosition = Vector3.up * 2;
                    shutter.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            if (bombRoute) {
                route.SetEnable(false);
                foreach (Vector2 point in route.GetFloors()) {
                    GameObject obj = this.level.GetObject(point);
                    if (obj) {;
                        obj.rigidbody.isKinematic = false;
                        obj.rigidbody.useGravity = true;
                    }
                }
            }
        }
    }
    
    private void UpdatePath (Room room) {
        AstarPath path = AstarPath.active;
        PointGraph graph = path.graphs[0] as PointGraph;
        GraphUpdateObject guo = new GraphUpdateObject(new Bounds (Vector3.zero, Vector3.one * 10000)); 
        guo.addPenalty = 100000;
        guo.updatePhysics = false;
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        AstarPath.active.UpdateGraphs(guo);
        foreach (Node node in graph.nodes) {
            Vector2 p = this.PositionToMatrix(new Vector3(node.position.x / 1000, node.position.y / 1000, node.position.z / 1000));
            if (this.level.GetRoom(p) == room) {
                guo.Apply(node);
            }
        }
    }
}
