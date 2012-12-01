using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit {
    
    protected List<Vector2> floors;
    protected bool enable = true;
    
    public Unit () {
        this.floors = new List<Vector2>();
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
        return center / this.floors.Count;
    }
    
    virtual public bool IsProtect () {
        return false;
    }
}
