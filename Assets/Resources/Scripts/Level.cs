using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
    private Dictionary<Vector2, GameObject> map;
    private Dictionary<Vector2, char> charMap;
    private List<Vector2> startPoints;
    private List<Room> rooms;
    private List<Route> routes;
    private int level = 0;
    private int width = 0;
    private int height = 0;    
    
    public Level (int level, int width, int height) {
        this.map = new Dictionary<Vector2, GameObject>();
        this.width = width;
        this.height = height;
        this.rooms = new List<Room>();
        this.routes = new List<Route>();
        this.startPoints = new List<Vector2>();
        this.charMap = new Dictionary<Vector2, char>();
    }

    public GameObject GetObject (Vector2 p) {
        return this.GetObject((int)p.x, (int)p.y);
    }
    
    public GameObject GetObject (int x, int y) {
        Vector2 v = new Vector2(x, y);
        if (this.map.ContainsKey(v) ) {
            return this.map[v];
        }
        return null;
    }
    
    public void SetObject (Vector2 position, GameObject obj) {
        this.map[position] = obj;
    }
    
    public void RemoveObject (Vector2 position) {
        this.map.Remove(position);
    }
    
    public int GetLevel () {
        return this.level;
    }
    
    public int GetWidth () {
        return this.width;
    }
    
    public int GetHeight () {
        return this.height;
    }
    
    public Dictionary<Vector2, char> GetCharMap () {
        return this.charMap;
    }
    
    public void SetCharMap (Dictionary<Vector2, char> cm) {
        this.charMap = cm;
    }
    
    public void AddRoom (Room room) {
        this.rooms.Add(room);
    }
    
    public void AddRoute (Route route) {
        this.routes.Add(route);
    }
    
    public void AddStartPoint (Vector2 point) {
        this.startPoints.Add(point);
    }
    
    public List<Room> GetStartRooms () {
        List<Room> startRooms = new List<Room>();
        return startRooms;
    }
    
    public Room GetRoom (Vector2 position) {
        foreach (Room room in this.rooms) {
            if (room.ContainsFloor((int)position.x, (int)position.y) ) {
                return room;
            }
        }
        return null;
    }
    
    public Route GetRoute (Vector2 position) {
        foreach (Route route in this.routes) {
            if (route.ContainsFloor((int)position.x, (int)position.y)) {
                return route;
            }
        }
        return null;
    }
    
    public void DisableRoom (Room room) {
        if (room == null) return; 
        room.SetEnable(false);
    }
    
    public bool IsFloor (int x, int y) {
        GameObject obj = this.GetObject(x, y);
        if (obj == null || !obj.GetComponent<Tile>()) return false;
        return obj.GetComponent<Tile>().type == Tile.TileType.Floor;
    }
    
    public bool IsRoute (int x, int y) {
        GameObject obj = this.GetObject(x, y);
        if (obj == null || !obj.GetComponent<Tile>()) return false;
        return obj.GetComponent<Tile>().type == Tile.TileType.Route;
    }
    
    public List<Room> GetRooms () {
        return this.rooms;
    }
    
    public List<Route> GetRoutes () {
        return this.routes;
    }
    
    public bool ContainsInRooms (int x, int y) {
        foreach (Room room in this.rooms) {
            if ( room.ContainsFloor(x, y) ) {
                return true;
            }
        }
        return false;
    }
    
    public bool ContainsInRoutes (int x, int y) {
        foreach (Route route in this.routes) {
            if ( route.ContainsFloor(x, y) ) {
                return true;
            }
        }
        return false;
    }
    
    public List<GameObject> GetNeighbors (Vector2 position, bool manhattan) {
        List<GameObject> neighbors = new List<GameObject>();
        int length = manhattan ? 4 : 8;
        Vector2[] vectors = {position + Vector2.up, position + Vector2.right, position - Vector2.up, position - Vector2.right, 
            position + Vector2.one, position - Vector2.one, position + Vector2.up - Vector2.right, position - Vector2.up + Vector2.right
        };
        for (int i = 0; i < length; ++i) {
            GameObject obj = this.GetObject(vectors[i]);
            if (obj != null) neighbors.Add(obj);
        }
        return neighbors;
    }
    
    public bool IsStartRoom (Room room) {
        foreach (Vector2 p in this.startPoints) {
            return room.ContainsFloor((int)p.x, (int)p.y);
        }
        return false;
    }
    
    public bool IsReachFromStart (Room room, bool enableOnly) {
        List<Room> neighbors = this.GetAllNeighborRooms(room, null, enableOnly);
        foreach (Room neighbor in neighbors) {
            if (neighbor.IsEnable() && this.IsStartRoom(neighbor)) {
                return true;
            }
        }
        return false;
    }
    
    public List<Room> GetAllNeighborRooms (Room room, List<Room> neighborRooms, bool enableOnly) {
        if (neighborRooms == null) {
            neighborRooms = new List<Room>();
        }
        neighborRooms.Add(room);
        foreach ( Room neighbor in room.GetNeighborRooms(enableOnly) ) {
            if ( !neighborRooms.Contains(neighbor) ) {
                return this.GetAllNeighborRooms(neighbor, neighborRooms, enableOnly);
            }
        }
        return neighborRooms;
    }
    
}
