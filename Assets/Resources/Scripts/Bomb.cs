using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

    void Start () {
    
    }
    
    void Update () {
    
    }
 
    void Explode () {
        GameObject manager = GameObject.FindWithTag("LevelManager");
        Room room = manager.GetComponent<LevelManager>().GetRoom(this.transform.position);
        if (room != null && !room.IsProtect()) {
            GameObject explosionPrefab = (GameObject)Resources.Load("Prefabs/bombExplosionPrefab");
            Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            manager.SendMessage("BombRoom", this.transform.position);
        }
        Destroy(gameObject);
    }
}
