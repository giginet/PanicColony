using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : Unit {
    private Dictionary<Vector2, Route> routes;
    private List<Vector2> walls;
    private bool protect = false;
    
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
    
    public bool ContainsWall(int x, int y) {
        Vector2 p = new Vector2(x, y);
        return this.walls.Contains(p);
    }
    
    public Dictionary<Vector2, Route> GetRoutes () {
        return this.routes;
    }
    
    public List<Room> GetNeighborRooms (bool enableOnly) {
        List<Room> neighborRooms = new List<Room>();
        foreach (Route route in this.routes.Values) {
            foreach (Room room in route.GetRooms().Values) {
                if (room != this && !neighborRooms.Contains(room)) {
                    if (!enableOnly || (enableOnly && route.IsEnable() && room.IsEnable() && this.IsEnable())) {
                        neighborRooms.Add(room);
                    }
                }
            }
        }
        return neighborRooms;
    }
    
    public bool IsProtect () {
        return this.protect;
    }
    
    public void SetProtect (bool p) {
        this.protect = p;
    }
    
}