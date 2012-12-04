using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
    private bool ground = false;

    void Start () {
        this.animation.Play("setup");
        GameObject radar = GameObject.FindWithTag("Radar");
        LevelManager manager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        Room room = manager.GetRoom(this.transform.position);
        if (room != null) {
            radar.SendMessage("SetWarning", room);
        }
        this.transform.parent = manager.GetLevelObject().transform;
    }
    
    void Update () {
        Ray floorRay = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(floorRay, out hit, this.transform.localScale.y / 2) && !this.ground) {
            this.animation.Play("setup");
            this.audio.Play();
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
