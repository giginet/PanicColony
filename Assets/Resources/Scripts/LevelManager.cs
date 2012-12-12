using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class LevelManager : MonoBehaviour {
    public int WIDTH = 10;
    public int HEIGHT = 10;
    private GameObject levelObject = null;
    private Level level;

    void Awake () {
    }
    
    void Update () {
    }
   
    /**
     * @brief Create level parse from file.
     * 
     * @param int levelNo
     * @return Level
     **/
    public void CreateLevel (int levelNo, List<Enemy> ignoreEnemies) {
        Dictionary<Vector2, char> charMap = new Dictionary<Vector2, char>(); 
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
                charMap.Add (new Vector2 (x, y), c);
            }
        }
        Level level = new Level (levelNo, width, height);
        this.level = level;
        level.SetCharMap(charMap);
        foreach (Vector2 key in charMap.Keys) {
            Vector3 position = this.MatrixToPosition(key);
            int x = (int)key.x;
            int y = (int)key.y;
            Vector2 p = new Vector2(x, y);
            char c = charMap[p];
            if (level.IsFloor(x, y)) {
                GameObject floorPrefab = (GameObject)Resources.Load ("Prefabs/floorPrefab", typeof(GameObject));
                GameObject floor = (GameObject)Instantiate (floorPrefab, position, Quaternion.identity);
                floor.transform.parent = levelObject.transform;
                level.SetObject(p, floor);
            }
            if (level.IsRoute(x, y)) {
                GameObject routePrefab = null;
                if (this.level.IsFloor(x + 1, y) || this.level.IsFloor(x - 1, y) || this.level.IsFloor(x, y + 1) || this.level.IsFloor(x, y - 1)) {
                    routePrefab = (GameObject)Resources.Load ("Prefabs/routeEntrancePrefab", typeof(GameObject));
                } else {
                    routePrefab = (GameObject)Resources.Load ("Prefabs/routePrefab", typeof(GameObject));
                }
                GameObject route = (GameObject)Instantiate (routePrefab, position, Quaternion.identity);
                route.transform.parent = levelObject.transform;
                // if route is placed holizontally, rotate object.
                if (level.IsRoute(x - 1, y) || level.IsRoute(x + 1, y)) {
                    route.transform.Rotate (new Vector3 (0, 90, 0));
                }
                level.SetObject(p, route);
                // add Gate
                if (char.IsUpper(c)) {
                    GameObject gatePrefab = (GameObject)Resources.Load("Prefabs/gatePrefab", typeof(GameObject)); 
                    this.AddGate(route, gatePrefab);
                } else if (c == '|') {
                    GameObject gatePrefab = (GameObject)Resources.Load("Prefabs/autoGatePrefab", typeof(GameObject)); 
                    this.AddGate(route, gatePrefab);
                }
            } else if (level.IsWall(x, y)) {
                GameObject wallPrefab = null;
                GameObject wall = null;               
                bool isHorizontal = level.IsFloor(x - 1, y) ^ level.IsFloor(x + 1, y);
                bool isCurve = false;
                bool isCorner = false;
                if (c == '#') {
                    if ((level.IsWall(x + 1, y) && level.IsWall(x, y + 1)) || 
                        (level.IsWall(x + 1, y) && level.IsWall(x, y - 1)) || 
                        (level.IsWall(x - 1, y) && level.IsWall(x, y + 1)) || 
                        (level.IsWall(x - 1, y) && level.IsWall(x, y - 1)) ) {
                        wallPrefab = (GameObject)Resources.Load ("Prefabs/cornerWallPrefab", typeof(GameObject));
                        isCorner = true;
                    } else if (isHorizontal) {
                        wallPrefab = (GameObject)Resources.Load ("Prefabs/curveWallPrefab", typeof(GameObject));
                        isCurve = true;   
                    } else {
                        // Normal Wall
                        wallPrefab = (GameObject)Resources.Load ("Prefabs/wallPrefab", typeof(GameObject));
                    }
                } else if (char.IsLower(c)) {
                    // corner Wall
                    wallPrefab = (GameObject)Resources.Load ("Prefabs/switchWallPrefab", typeof(GameObject));
                } else if (c == '$') {
                    // Broken Wall
                    wallPrefab = (GameObject)Resources.Load ("Prefabs/brokenWallPrefab", typeof(GameObject));
                } else if (c == '%') {
                    // Window Wall
                    wallPrefab = (GameObject)Resources.Load ("Prefabs/windowWallPrefab", typeof(GameObject));
                }
                wall = (GameObject)Instantiate(wallPrefab, position, Quaternion.identity);
                if (isCurve) {
                    if (level.IsFloor(x - 1, y)) {
                        wall.transform.Rotate (new Vector3 (0, 180, 0));
                    }
                } else {
                    if (isCorner) {
                        if (level.IsFloor(x, y - 1) || level.IsWall(x, y - 1)) {
                            wall.transform.Translate(Vector3.forward * -0.1f);
                        } else if (level.IsFloor(x, y + 1) || level.IsWall(x, y + 1)) {
                            wall.transform.Translate(Vector3.forward * 0.1f);
                        }
                    } else {
                        if (level.IsFloor(x, y - 1) || level.IsFloor(x - 1, y - 1) || level.IsFloor(x + 1, y - 1) ) {
                            wall.transform.Translate(Vector3.forward * (this.HEIGHT / 2.0f));
                        } else if (level.IsFloor(x, y + 1) || level.IsFloor(x - 1, y + 1) || level.IsFloor(x + 1, y + 1) ) {
                            wall.transform.Translate(Vector3.forward * -(this.HEIGHT / 2.0f));
                            wall.transform.Rotate(Vector3.up * 180);
                        }
                    }
                }
                wall.transform.parent = levelObject.transform;
                level.SetObject(p, wall);
            } else if (char.IsDigit(c)) {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/playerPrefab");
                GameObject player = (GameObject)Instantiate(prefab, position + Vector3.up, Quaternion.identity);
                player.transform.parent = levelObject.transform;
                this.level.AddStartPoint(p);
            } else if (c == '!' || c == '?') {
                string prefabName = "enemyPrefab";
                if (c == '?') prefabName = "bombEnemyPrefab";
                bool ignore = false;
                foreach (Enemy enemy in ignoreEnemies) {
                    if (enemy.GetInitialPosition() == p) {
                        ignore = true;
                        break;
                    }
                }
                if (!ignore) {
                    GameObject enemyPrefab = (GameObject)Resources.Load ("Prefabs/" + prefabName, typeof(GameObject));
                    GameObject enemy = (GameObject)Instantiate (enemyPrefab, position + Vector3.up * 2, Quaternion.identity);
                    enemy.transform.parent = levelObject.transform;
                }
            }
        }
        this.CreateRooms();
        this.CreateRoutes();
        this.ConnectGates(); 
        AstarPath path = AstarPath.active;
        path.Initialize();
        PointGraph graph = path.graphs[0] as PointGraph;
        graph.Scan();
        path.FlushGraphUpdates();
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
                        foreach (Vector2 point in room.GetFloors()) {
                            Vector2[] neighbors = {point + Vector2.up, point + -Vector2.up, point + Vector2.right, point + -Vector2.right,
                                point + Vector2.one, point - Vector2.one, point + Vector2.up - Vector2.right, -Vector2.up + Vector2.right
                            };
                            foreach (Vector2 neighbor in neighbors) {
                                if (this.level.IsWall((int)neighbor.x, (int)neighbor.y)) {
                                    room.AddWalls(neighbor);
                                }
                            }
                        }
                        // make start room as protected.
                        if (this.level.IsStartUnit(room) ){
                            room.SetProtect(true);
                        }
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
                        if (this.level.IsRoute(x + 1, y) || this.level.IsRoute(x - 1, y)) {
                            light.transform.Rotate(Vector3.up * 90);
                        }
                        light.transform.parent = levelObject.transform;
                    }
                }
            }
        }
    }
       
    private void AddNeighborRoom (Room room, int x, int y) {
        room.AddFloor(x, y);
        if (this.level.GetCharMap()[new Vector2((int)x, (int)y)] == '*') {
            room.SetProtect(true);
        }
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
                    route.AddNeighbor(new Vector2(x, y), room);
                    room.AddNeighbor(v, route);
                }
            }
        }
    }
    
    public Vector2 PositionToMatrix (Vector3 position) {
        float x = position.x;
        float y = position.z;
        return new Vector2(Mathf.Round(x / this.WIDTH), Mathf.Round(-y / this.HEIGHT));
    }
    
    public Vector3 MatrixToPosition (Vector2 matrix) {
        return new Vector3 (matrix.x * WIDTH, 0, -matrix.y * HEIGHT);
    }
    
    private void DestroyTile (Vector2 point, bool rigidbody) {
        GameObject obj = this.level.GetObject(point);
        if (rigidbody && obj) {
            obj.rigidbody.isKinematic = false;
            obj.rigidbody.useGravity = true;
        } else if (obj){
            Destroy(obj);
        }
        this.level.RemoveObject(point);
    }
    
    public void BombRoom (Vector3 position) {
        Room bombRoom = this.GetRoom(position);
        if (bombRoom != null) {
            this.BombRoom (bombRoom);
        }
    }
    
    private bool IsExistPlayer (Unit unit) {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            Vector2 v = this.PositionToMatrix(player.transform.position);
            if (unit.ContainsFloor((int)v.x, (int)v.y)) {
                return true;
            }
        }
        return false;
    }
    
    public void DestroyRoom (Room room) {
        bool rigidbody = this.IsExistPlayer(room); 
        foreach (Vector2 point in room.GetWalls()) {
            this.DestroyTile(point, rigidbody);
        }
        this.DestroyUnit(room);
        this.level.DisableRoom(room);
    }
    
    public void DestroyUnit (Unit unit) {
        bool rigidbody = this.IsExistPlayer(unit); 
        foreach (Vector2 point in unit.GetFloors()) {
            this.DestroyTile(point, rigidbody);
        }
        
        if (unit.IsEnable()) {
            GameObject radar = GameObject.FindWithTag("Radar");
            radar.SendMessage("DestroyUnit", unit);
            List<GameObject> enemies = new List<GameObject>();
            // Send enemies to controller killed by explosion.
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                if (this.level.GetUnit(this.PositionToMatrix(enemy.transform.position)) == unit) {
                    enemy.SendMessage("Death");
                    enemies.Add(enemy);
                }
            }
            GameObject controller = GameObject.FindWithTag("GameController");
            controller.SendMessage("DestroyEnemy", enemies);
            this.AddExplosion(unit);
        } 
    }
    
    public void BombRoute (Route route) {
        this.DestroyUnit(route);
        this.DestroyIsolatedUnits();
    }
    
    public void BombRoom(Room room) {
        if (room.IsProtect()) return;
        this.DestroyRoom(room); 
        this.DestroyIsolatedUnits();
    }
    
    public void DestroyIsolatedUnits() {
        List<Room> destroyRooms = new List<Room>();
        foreach (Room r in this.level.GetRooms() ) {
            if (!this.level.IsReachFromStart(r, true)) {
                destroyRooms.Add (r);
            }
        }
        foreach (Room r in destroyRooms) { 
            this.DestroyRoom(r); 
        }
        foreach (Route route in this.level.GetRoutes()) {
            foreach (KeyValuePair<Vector2, Unit> pair in route.GetNeighbors()) {
                Vector2 pos = pair.Key;
                Unit u = pair.Value;
                if (!u.IsEnable()) {  
                    GameObject shutterPrefab = (GameObject)Resources.Load("Prefabs/shutterPrefab", typeof(GameObject)); 
                    GameObject routeTile = this.level.GetObject(pos);
                    if (routeTile != null) {
                        this.AddGate(routeTile, shutterPrefab);
                    }
                }
            }
            if (!this.level.IsReachFromStart(route, true)) {
                this.DestroyUnit(route);
            }
        }  
        this.UpdatePath(); 
    }
    
    private void UpdatePath () {
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
            Room r = this.level.GetRoom (p);
            if (r != null && !r.IsEnable()) {
                guo.Apply(node);
            }
        }
    }
    
    private void AddExplosion (Unit unit) {
        GameObject explosionPrefab = (GameObject)Resources.Load("Prefabs/bombExplosionPrefab");
        Instantiate(explosionPrefab, this.MatrixToPosition(unit.GetCenter()), Quaternion.identity);
    }
    
    public void AttachBomb (GameObject bomb) {
        Vector2 pos = this.PositionToMatrix(bomb.transform.position);
        Room room = this.level.GetRoom(pos);
        List<Unit> explodeUnits = new List<Unit>();
    }
    
    public Room GetRoom (Vector3 position) {
        Vector2 p = this.PositionToMatrix (position);
        return this.level.GetRoom(p);
    }
    
    public Unit GetUnit (Vector3 position) {
        Vector2 p = this.PositionToMatrix (position);
        return this.level.GetUnit(p);
    }
    
    public void AddGate (GameObject routeTile, GameObject prefab) {
        GameObject shutter = (GameObject)Instantiate(prefab, routeTile.transform.position, Quaternion.identity);
        shutter.transform.parent = routeTile.transform;
        shutter.transform.localPosition = Vector3.zero;
        shutter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        Vector2 pos = this.PositionToMatrix(routeTile.transform.position);
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (this.level.IsFloor(x, y + 1) || this.level.IsFloor(x - 1, y)) {
            shutter.transform.localPosition += Vector3.back;
        } else if (this.level.IsFloor(x, y - 1) || this.level.IsFloor(x + 1, y)) {
            shutter.transform.localPosition += Vector3.forward;
        }
    }
    
    private void ConnectGates () {
        Dictionary<char, GameObject> gates = new Dictionary<char, GameObject>();
        Dictionary<char, GameObject> switchs = new Dictionary<char, GameObject>();
        foreach (Vector2 position in this.level.GetCharMap().Keys) {
            char c = this.level.GetCharMap()[position];
            if (char.IsUpper(c)) {
                // gate
                gates.Add(c, this.level.GetObject(position));
            } else if (char.IsLower(c)) {
                // switch
                switchs.Add(c, this.level.GetObject(position));
            }
        }
        foreach (char c in switchs.Keys) {
            if (gates.ContainsKey(char.ToUpper(c))) {
                Switch switchComponent = switchs[c].GetComponentInChildren<Switch>();
                Gate gateComponent = gates[char.ToUpper(c)].GetComponentInChildren<Gate>();
                switchComponent.SendMessage("SetGate", gateComponent.gameObject);
            }
        }
    }
    
    public Level GetLevel () {
        return this.level;
    }
    
    public void DestroyLevel () {
        Destroy(this.levelObject);
        this.level = null;
        this.levelObject = null;
    }
    
    public GameObject GetLevelObject () {
        return this.levelObject;
    }
    
    void ResetLevel () {
    }
}
