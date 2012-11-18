using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
    protected List<Vector2> floors;
    protected bool enable;
    
    public Room () {
        this.floors = new List<Vector2>();
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
    
    public bool GetEnable () {
        return this.enable;
    }
    
    public void SetEnable (bool enable) {
        this.enable = enable;
    }
}