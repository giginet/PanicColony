using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
    private bool ground = false;

    void Start () {
        this.animation.Play("setup");
    }
    
    void Update () {
        Ray floorRay = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(floorRay, out hit, this.transform.localScale.y / 2) && !this.ground) {
            this.animation.Play("setup");
            this.ground = true;
        } else if (!this.ground) {
            this.animation.Stop("setup");
        }
    
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
