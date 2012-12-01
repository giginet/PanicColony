using UnityEngine;
using System.Collections;

public class Absorber : MonoBehaviour {
    public float absorbSpeed = 70.0f;
    private LevelManager levelManager = null;

    // Use this for initialization
    void Start () {
        this.levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
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
        Vector3 position = this.transform.position;
        position.y = 0;
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = 0;
        float distance = Vector3.Distance (targetPosition, position);
        Room absorberRoom = this.levelManager.GetRoom(this.transform.position);
        Room targetRoom = this.levelManager.GetRoom(target.transform.position);
        if (absorberRoom == targetRoom) {
            CharacterController controller = target.GetComponent<CharacterController>();
            if (controller != null) {
                Vector3 absorb = this.transform.position - target.transform.position;
                absorb = Vector3.Normalize(absorb) * (absorbSpeed / Vector3.Distance(this.transform.position, target.transform.position)) * Time.deltaTime;
                controller.Move(absorb);
                if (distance < 0.5) {
                    controller.Move(Vector3.down * 10);
                }
            }
        }
    }
}