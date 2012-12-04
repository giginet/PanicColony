using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit {
    
    protected Dictionary<Vector2, Unit> neighbors;
    protected List<Vector2> floors;
    protected bool enable = true;
    
    public Unit () {
        this.floors = new List<Vector2>();
        this.neighbors = new Dictionary<Vector2, Unit>();
        this.enable = true;
    }
    
    public bool ContainsFloor (int x, int y) {
        Vector2 p = new Vector2(x, y);
        return this.floors.Contains(p);
    }
    
    public void AddFloor(int x, int y) {
        Vector2 p = new Vector2(x, y);
        this.floors.Add(p);
    }
    
    public List<Vector2> GetFloors () {
        return this.floors;
    }
    
    public bool IsEnable () {
        return this.enable;
    }
    
    public void SetEnable (bool enable) {
        this.enable = enable;
    }
    
    public Vector2 GetCenter() {
        Vector2 center = Vector3.zero;
        foreach (Vector2 floor in this.floors) {
            center += floor;
        }
        center = center / this.floors.Count;
        center.x = (int)center.x;
        center.y = (int)center.y;
        return center;
    }
    
    virtual public bool IsProtect () {
        return false;
    }
    
    public void AddNeighbor (Vector2 position, Unit neighbor) {
        this.neighbors.Add(position, neighbor);
    }
    
    public Dictionary<Vector2, Unit> GetNeighbors () {
        return this.neighbors;
    }
    
    public List<Unit> GetNeighborUnits (bool enableOnly) {
        List<Unit> neighborUnits = new List<Unit>();
        if (this.IsEnable() || !enableOnly ) {
            foreach (Unit neighbor in this.neighbors.Values) {
                if (neighbor.IsEnable() || !enableOnly) {
                    neighborUnits.Add(neighbor);
                }
            }
        }
        return neighborUnits;
    }
}
