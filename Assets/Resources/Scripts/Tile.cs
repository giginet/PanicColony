using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public TileType type = TileType.None;

    public enum TileType {
        None,
        Floor,
        Wall,
        Route
    }
    
    void Start () {
    }
    
    void Update () {
        if (this.transform.position.y < -100) {
            Destroy(gameObject);
        }
    }
}
