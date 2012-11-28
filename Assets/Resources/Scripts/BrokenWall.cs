using UnityEngine;
using System.Collections;

public class BrokenWall : Wall {
    
    void Start () {
    }

    protected void Damage () {
        Destroy (this.gameObject);
        GameObject absorberPrefab = (GameObject)Resources.Load ("Prefabs/absorberPrefab", typeof(GameObject));
        GameObject absorber = (GameObject)Instantiate (absorberPrefab, this.transform.position, Quaternion.identity);
    }
    
}
