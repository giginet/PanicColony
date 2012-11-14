using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
 
    void Explode () {
        GameObject explosionPrefab = (GameObject)Resources.Load("Prefabs/bombExplosionPrefab");
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
        GameObject manager = GameObject.FindWithTag("LevelManager");
        manager.SendMessage("DestroyRoom", this.transform.position);
    }
}
