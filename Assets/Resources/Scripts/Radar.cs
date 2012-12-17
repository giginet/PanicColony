using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class Radar : MonoBehaviour {
    public GameObject radarCamera = null;
    private Level level;
    private LevelManager levelManager;
    private Dictionary<Vector2, GameObject> tiles;
    private Dictionary<GameObject, GameObject> chips;
    
    public enum FloorColor {
        Normal,
        Protected,
        Warning,
        Start
    };
    
    void Start () {
        this.levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        this.transform.position = new Vector3(1000, 0, 1000);
        this.radarCamera = GameObject.Find("TopCamera");
        this.DestroyRadar(); 
        this.CreateRadar();
    }
    
    void Update () {
        this.UpdateChip();
        GameObject player = GameObject.FindWithTag("Player");
        this.radarCamera.transform.position = this.transform.TransformPoint(player.transform.localPosition / levelManager.WIDTH + Vector3.up * 50);
    }
    
    void DestroyRadar () {
        this.tiles = new Dictionary<Vector2, GameObject>();
        this.chips = new Dictionary<GameObject, GameObject>();
    }
    
    void CreateRadar () {
        this.level = levelManager.GetLevel();
        Dictionary<Vector2, char> charMap = this.level.GetCharMap();
        foreach (Vector2 key in charMap.Keys) {
            Vector3 position = new Vector3 (key.x, 0, -key.y);
            int x = (int)key.x;
            int y = (int)key.y;
            Vector2 p = new Vector2(x, y);
            char c = charMap[p];
            if (this.level.IsFloor(x, y) || this.level.IsRoute(x, y)) {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/Radar/floorRadarPrefab", typeof(GameObject));
                GameObject floor = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
                floor.transform.parent = this.transform;
                floor.transform.localPosition = position;
                this.tiles.Add(p, floor);
            }
        } 
        string[] tags = {"Player", "Enemy", "Gate", "Switch", "BrokenWall"};
        foreach (string name in tags) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(name)) {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/Radar/" + name.ToLower() + "RadarPrefab", typeof(GameObject));
                this.AddChip(obj, prefab);
            }
        }
        foreach (Route route in this.level.GetRoutes()) {
            this.SetUnitColor(route, FloorColor.Protected);
        }
        foreach (Room room in this.level.GetRooms()) {
            if (this.level.IsStartUnit(room)) {
                this.SetUnitColor(room, FloorColor.Start);
            } else if (room.IsProtect()) {
                this.SetUnitColor(room, FloorColor.Protected);
            }
        }
    }
    
    private void UpdateChip () {
        string[] tags = {"Player", "Enemy", "Bomb", "Shutter", "Gate", "Switch", "BrokenWall", "Absorber", "Vegetable"};
        GameObject levelObject = this.levelManager.GetLevelObject();
        foreach (string name in tags) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(name)) {
                if (chips.ContainsKey(obj)) {
                    GameObject chip = this.chips[obj];
                    chip.transform.localPosition = levelObject.transform.TransformPoint(obj.transform.position) / this.levelManager.WIDTH;
                    if (name == "Player" || name == "Enemy") {
                        chip.transform.localRotation = obj.transform.localRotation;
                    }
                    if (name == "Gate") {
                        Gate gate = obj.GetComponent<Gate>();
                        chip.renderer.enabled = !gate.IsOpen();
                    }
                } else {
                    string camel = char.ToLower(name[0]) + name.Substring(1, name.Length - 1);
                    GameObject prefab = (GameObject)Resources.Load("Prefabs/Radar/" + camel + "RadarPrefab", typeof(GameObject));
                    this.AddChip(obj, prefab);
                }
                
            }
        }
        List<GameObject> outdated = new List<GameObject>();
        foreach (GameObject obj in this.chips.Keys) {
            if (obj == null) {
                outdated.Add (obj);
            }
        }
        foreach (GameObject obj in outdated) this.DestroyChip(obj);
    }
    
    private void DestroyUnit (Unit unit) {
        Vector2 center = unit.GetCenter();
        GameObject explosionPrefab = (GameObject)Resources.Load ("Prefabs/Radar/explosionRadarPrefab", typeof(GameObject));
        GameObject explosion = (GameObject)Instantiate(explosionPrefab, Vector3.zero, Quaternion.identity);
        explosion.transform.parent = this.transform;
        explosion.transform.localPosition = this.levelManager.MatrixToPosition(center) / this.levelManager.WIDTH;
        foreach (Vector2 floor in unit.GetFloors()) {
            if (!tiles.ContainsKey(floor)) continue;
            GameObject tile = this.tiles[floor];
            this.tiles.Remove(floor);
            Destroy(tile);
        }
    }
    
    private void AddChip (GameObject obj, GameObject prefab) {
        GameObject chip = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
        chip.transform.parent = this.transform;
        chip.transform.localPosition = obj.transform.localPosition / levelManager.WIDTH; 
        chip.transform.eulerAngles = Vector3.up * 180;
        this.chips.Add(obj, chip);
    }
    
    public void DestroyChip (GameObject obj) {
        if (this.chips.ContainsKey(obj)) {
            GameObject chip = this.chips[obj];
            Destroy(chip);
            this.chips.Remove(obj);
        }
    }
    
    public void SetWarning (Unit unit) {
        if (unit.IsProtect()) return;
        bool enable = unit.IsEnable();
        if (enable && !unit.IsProtect()) {
            unit.SetEnable(false);
        }
        foreach (Room other in this.level.GetRooms()) {
            if (!this.level.IsReachFromStart(other, true)) {
                this.SetUnitColor(other, FloorColor.Warning);
            }
        }
        foreach (Route other in this.level.GetRoutes()) {
            if (!this.level.IsReachFromStart(other, true)) {
                this.SetUnitColor(other, FloorColor.Warning);
            }
        }
        if (enable) {
            unit.SetEnable(true);
        }
    }
    
    public void SetUnitColor (Unit unit, FloorColor color) {
        Material material;
        if (color == FloorColor.Warning) {
            material = (Material)Resources.Load("Materials/warningFloorRadarMaterial", typeof(Material));
        } else if (color == FloorColor.Protected) {
            material = (Material)Resources.Load("Materials/protectedRoomRadarMaterial", typeof(Material));
        } else if (color == FloorColor.Start) {
            material = (Material)Resources.Load("Materials/startRoomRadarMaterial", typeof(Material));
        } else {
            material = (Material)Resources.Load("Materials/floorRadarMaterial", typeof(Material));
        }
        foreach (Vector2 pos in unit.GetFloors()) {
            if (this.tiles.ContainsKey(pos)) {
                GameObject tile = this.tiles[pos];
                tile.renderer.material = material;
            }
        }
    }
}
