using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : Unit {
    Dictionary<Vector2, Route> routes;
    List<Vector2> walls;
    
    public Room () {
        this.floors = new List<Vector2>();
        this.enable = true;
        routes = new Dictionary<Vector2, Route>();
        walls = new List<Vector2>();
    }
    
    public void AddWalls(Vector2 p) {
        this.walls.Add(p);
    }
    
    public List<Vector2> GetWalls () {
        return this.walls;
    }
    
    public void AddRoute (Vector2 position, Route route) {
        this.routes.Add(position, route);
    }
    
    public Dictionary<Vector2, Route> GetRoutes () {
        return this.routes;
    }
    
    public List<Room> GetNeighborRooms (bool enableOnly) {
        List<Room> neighborRooms = new List<Room>();
        foreach (Route route in this.routes.Values) {
            foreach (Room room in route.GetRooms().Values) {
                if (room != this && !neighborRooms.Contains(room)) {
                    if (!enableOnly || (enableOnly && route.GetEnable() && room.GetEnable()))
                    neighborRooms.Add(room);
                }
            }
        }
        return neighborRooms;
    }
    
}