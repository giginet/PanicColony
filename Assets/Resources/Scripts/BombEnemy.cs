using UnityEngine;
using System.Collections;

public class BombEnemy : Enemy {

    private GameObject robot = null;
    private bool slip = false;

    virtual protected void Start () {
        base.Start();
        this.robot = this.transform.Find("robot").gameObject;
        this.robot.animation.Play("run");
        this.slip = false;
        this.robot.collider.enabled = false;
    }
    
    override public void Attack () {
    }

    // Use this for initialization
    virtual protected void Update () {
        base.Update();
        CharacterController controller = this.GetComponent<CharacterController>();
        if (controller != null && !this.slip) {
            if (Vector3.Distance(Vector3.zero, controller.velocity) > 1.0f) {
                if (!this.robot.animation.IsPlaying("run")) {
                    this.robot.animation.Play("run");
                }
            } else {
                if (!this.robot.animation.IsPlaying("idle")) {
                    this.robot.animation.Play("idle");
                }
            }
        }
    }
    
    override public void Shock () {
        this.aiPath.canMove = false;
        if (!this.slip) {
            this.robot.animation.Play("slip");
            this.slip = true;
            StartCoroutine(this.BombRoom());
            this.SendMessage("UpdateRadar");
            this.gameObject.tag = "";
            GameObject radar = GameObject.FindWithTag("Radar");
            radar.SendMessage("DestroyChip", this.gameObject);
            this.gameObject.GetComponent<CharacterController>().radius = 0.01f;
            this.gameObject.GetComponent<CharacterController>().height = 0.01f;
        } 
    }
    
    private IEnumerator BombRoom () {
        Room room = this.levelManager.GetRoom(this.transform.position);
        if (room != null && !room.IsProtect()) {
            yield return new WaitForSeconds(10.0f);
            this.SendMessage("Explode");
        }
        yield return null;
    }
}
