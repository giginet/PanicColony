using UnityEngine;
using System.Collections;

public class BrokenWall : Wall {
    
    void Start () {
    }

    protected void Damage () {
        Destroy (this.gameObject);
    }
    
}
