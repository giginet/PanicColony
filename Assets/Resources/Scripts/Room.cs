using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : Unit {
    private List<Vector2> walls;
    private bool protect = false;
    
    public Room () {
        this.neighbors = new Dictionary<Vector2, Unit>();
        this.floors = new List<Vector2>();
        this.enable = true;
        walls = new List<Vector2>();
    }
    
    public void AddWalls(Vector2 p) {
        this.walls.Add(p);
    }
    
    public List<Vector2> GetWalls () {
        return this.walls;
    }  
    
    public bool ContainsWall(int x, int y) {
        Vector2 p = new Vector2(x, y);
        return this.walls.Contains(p);
    } 
    
    public bool IsProtect () {
        return this.protect;
    }
    
    public void SetProtect (bool p) {
        this.protect = p;
    }
    
}