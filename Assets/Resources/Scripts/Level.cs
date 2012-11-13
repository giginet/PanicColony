using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
    private Dictionary<Vector2, char> map;
    private List<Room> rooms;
    private int level = 0;
    private int width = 0;
    private int height = 0;    
    
    public Level (int level, int width, int height, Dictionary<Vector2, char> map) {
        this.map = map;
        this.width = width;
        this.height = height;
        this.rooms = new List<Room>();
    }
    
    public char GetMap (int x, int y) {
        Vector2 v = new Vector2(x, y);
        if (this.map.ContainsKey(v) ) {
            return this.map[v];
        }
        return ' ';
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
        Debug.Log (rooms.Count);
    }
    
    public bool IsFloor (int x, int y) {
        char c = this.GetMap(x, y);
        char[] floorList = {'.', 'S'};
        List<char> floors = new List<char>(floorList);
        return floors.Contains(c); 
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
