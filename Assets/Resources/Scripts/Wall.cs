using UnityEngine;
using System.Collections;

public class Wall : Tile {

    void Start () {
        this.type = TileType.Wall; 
    }
    
    // Update is called once per frame
    void Update () {
    
    }
    
    virtual protected void Damage () {
        // burn the wall.
    }
}
