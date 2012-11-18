using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Route : Room {
 
    List<Room> rooms;
    
    public Route () {
        this.floors = new List<Vector2>();
        this.rooms = new List<Room>(2);
    }
    
    public void SetRoom (int index, Room room) {
        this.rooms[index] = room;
    }
    
    public List<Room> GetRooms () {
        return this.rooms;
    }
}