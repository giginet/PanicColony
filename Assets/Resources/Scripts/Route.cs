using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Route : Room {
 
    Dictionary<Vector2, Room> rooms;
    
    public Route () {
        this.floors = new List<Vector2>();
        this.rooms = new Dictionary<Vector2, Room>();
        this.enable = true;
    }
    
    public void AddRoom (Vector2 position, Room room) {
        this.rooms.Add(position, room);
    }
    
    public Dictionary<Vector2, Room> GetRooms () {
        return this.rooms;
    }
}