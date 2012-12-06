using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Route : Unit { 
    public Route () {
        this.neighbors = new List<KeyValuePair<Vector2, Unit>>();
        this.floors = new List<Vector2>();
        this.enable = true;
    }   
}