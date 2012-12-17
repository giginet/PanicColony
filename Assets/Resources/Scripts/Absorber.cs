using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Absorber : MonoBehaviour {
    public float absorbSpeed = 70.0f;
    private LevelManager levelManager = null;
    private List<GameObject> absorbed;

    // Use this for initialization
    void Start () {
        this.levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        this.transform.parent = this.levelManager.GetLevelObject().transform;
        this.absorbed = new List<GameObject>();
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Player")) {
            this.Absorb (target);
        } 
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Enemy")) {
            this.Absorb (target);
        }
    }
    
    void Absorb (GameObject target) {
        CharacterController controller = target.GetComponent<CharacterController>();
        if (this.absorbed.Contains(target)) {
            Vector2 pos = this.levelManager.PositionToMatrix(this.transform.position);
            int x = (int)pos.x;
            int y = (int)pos.y;
            if (this.levelManager.GetLevel().IsFloor(x, y + 1)) {
                controller.Move(Vector3.forward * 10); 
            } else {
                controller.Move(Vector3.back * 10); 
            }
        } else {
            Vector3 position = this.transform.position;
            position.y = 0;
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = 0;
            float distance = Vector3.Distance (targetPosition, position);
            Room absorberRoom = this.levelManager.GetRoom(this.transform.position);
            Room targetRoom = this.levelManager.GetRoom(target.transform.position);
            if (absorberRoom == targetRoom) {
                if (controller != null) {
                    Vector3 absorb = this.transform.position - target.transform.position;
                    absorb = Vector3.Normalize(absorb) * (absorbSpeed / Vector3.Distance(this.transform.position, target.transform.position)) * Time.deltaTime;
                    controller.Move(absorb);
                    if (distance < 1.0) {
                        this.absorbed.Add(target);
                    }
                }
            }
        }
    }
}