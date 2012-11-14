using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
    private Dictionary<Vector2, GameObject> map;
    private List<Room> rooms;
    private int level = 0;
    private int width = 0;
    private int height = 0;    
    
    public Level (int level, int width, int height) {
        this.map = new Dictionary<Vector2, GameObject>();
        this.width = width;
        this.height = height;
        this.rooms = new List<Room>();
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
    
    public void AddRoom (Room room) {
        this.rooms.Add(room);
    }
    
    public Room GetRoom (Vector2 position) {
        foreach (Room room in this.rooms) {
            if (room.ContainsFloor((int)position.x, (int)position.y) ) {
                return room;
            }
        }
        return null;
    }
    
    public void RemoveRoom (Room room) {
        if (room == null) return; 
        this.rooms.Remove(room);
    }
    
    public bool IsFloor (int x, int y) {
        GameObject obj = this.GetObject(x, y);
        if (obj == null) return false;
        return obj.CompareTag("Floor");
    }
    
    public bool ContainsInRooms (int x, int y) {
        foreach (Room room in this.rooms) {
            if ( room.ContainsFloor(x, y) ) {
                return true;
            }
        }
        return false;
    }
}
