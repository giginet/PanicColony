using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public enum PlayerState {
        Normal,
        Shock,
        DeathAnimation,
        Death,
        Clear
    };
   
    public int playerNumber = 0;
    public int rotateThreshold = 150;
    public int rotateSpeed = 5;
    public float speed = 10;
    public GameObject muzzle = null;
    private GameObject bomb = null;
    private GameObject shockEffect = null;
    private bool canControl = true;
    private JoyStickController controller = null;
    private PlayerState state = PlayerState.Normal;
    private float preYAxis = 0;
    private float deathTimer = 0;
    private GameObject head = null;
    private LineRenderer renderer = null;
    private float shootingTimer = 0.0f;
    private bool isShooting = false;

    void Start () {
        this.muzzle = this.transform.Find ("Armature/Bone/hip_0/chest_0/arm_R_0/forearm_R/muzzle").gameObject;
        this.state = PlayerState.Normal;
        this.controller = this.GetComponent<JoyStickController> ();
        this.controller.cameraControl.cameraTransform = Camera.main.transform;
        this.controller.cameraControl.cameraTransform.position = this.transform.position;
        this.head = this.transform.Find ("Armature/Bone/hip_0/chest_0/head").gameObject;
        this.renderer = this.GetComponent<LineRenderer> (); 
    }
    
    void AttachBomb () {
        // Attach Bomb
        if (Input.GetButtonDown ("Bomb0")) {
            if (bomb == null) {
                // place Bomb
                GameObject bombPrefab = (GameObject)Resources.Load ("Prefabs/bombPrefab");
                this.animation.Play ("attach");
                bomb = (GameObject)Instantiate (bombPrefab, this.transform.position + this.transform.forward * 1, Quaternion.identity);
            } else {
                bomb.SendMessage ("Explode");
                bomb = null;
            }
        }
    }
    
    bool Shot (Ray ray, RaycastHit hit) {
        if (Input.GetButtonDown ("Shock") && !this.isShooting) {
            AudioClip shot = (AudioClip)Resources.Load("Sounds/gun");
            this.transform.Find("gun").audio.PlayOneShot(shot);
            this.isShooting = true;
            if (shockEffect == null) {
                GameObject prefab = (GameObject)Resources.Load ("Prefabs/shockEffectPrefab", typeof(GameObject));
                shockEffect = (GameObject)Instantiate (prefab, this.muzzle.transform.position, Quaternion.identity);
                LevelManager manager = GameObject.FindWithTag ("LevelManager").GetComponent<LevelManager> ();
                shockEffect.transform.parent = manager.GetLevelObject ().transform;
            }
            if (hit.collider.gameObject.CompareTag ("Enemy")) {
                hit.collider.gameObject.SendMessage ("Shock");
            } else if (hit.collider.gameObject.CompareTag ("Bomb")) {
                hit.collider.gameObject.SendMessage ("Explode");
            } else if (hit.collider.gameObject.CompareTag ("Switch")) {
                hit.collider.gameObject.SendMessage ("Toggle");
            } else if (hit.collider.gameObject.CompareTag ("Wall")) {
                hit.collider.gameObject.SendMessage ("Damage");
            }
            return true;
        }
        return false;
    }
    
    void Control () { 
        this.AttachBomb ();
        Vector3 screenPoint = Camera.main.WorldToScreenPoint (this.transform.position);
        Ray ray = new Ray (this.muzzle.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 100)) {
            this.Shot (ray, hit); 
        } 
        if (this.isShooting) {
            this.shootingTimer += Time.deltaTime;
            renderer.enabled = false;
            if (this.shockEffect != null) {
                shockEffect.transform.position = this.muzzle.transform.position;
                shockEffect.transform.LookAt (hit.point);
            }
            if (this.shootingTimer > 0.5f) {
                Destroy (this.shockEffect);
                this.shockEffect = null;
                this.isShooting = false;
                this.shootingTimer = 0.0f;
            }
        } else {
            renderer.enabled = true;
            renderer.SetVertexCount (2);
            renderer.SetPosition (0, this.muzzle.transform.position);
            if (hit.collider != null) {       
                renderer.SetPosition (1, hit.point);
            } else {
                renderer.SetPosition (1, this.muzzle.transform.position + this.transform.forward * 100);
            }
            if (hit.collider != null) {
                if (hit.collider.gameObject.CompareTag ("Enemy")) {
                    renderer.material.color = Color.red;   
                } else {
                    renderer.material.color = Color.yellow;   
                }
            }
        }
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        bool isAnimation = this.animation.IsPlaying("attach");
        if (!isAnimation) {
            if (h < -0.2) {
                if (!this.animation.IsPlaying("left")) {
                    this.animation.Play("left");
                }
            } else if (h > 0.2) {
                if (!this.animation.IsPlaying("right")) {
                    this.animation.Play("right");
                }
            } else if (v < -0.2) {
                if (!this.animation.IsPlaying("back")) {
                    this.animation.Play("back");
                }
            } else if (v > 0.2) {
                if (!this.animation.IsPlaying("forward")) {
                    this.animation.Play("forward");
                }
            } else {
                if (!this.animation.IsPlaying("idle")) {
                    this.animation.Play("idle");
                }
            }
        }
        
        CharacterMotor motor = this.GetComponent<CharacterMotor>();
        /*if (motor.GetDirection() != Vector3.zero) {
            this.transform.LookAt(this.transform.position + motor.GetDirection());
        }*/
        this.audio.volume = Vector3.Distance (motor.GetDirection (), Vector3.zero) / 1.0f;
    }
    
    void Update () {
        //this.controller.SetInputType(Input.GetJoystickNames().Length == 0 ? InputType.Mouse : InputType.JoyStick);
        this.controller.SetInputType (InputType.JoyStick);
        if (this.state == PlayerState.Normal && this.canControl) {
            this.Control ();
        } 
        if (this.state == PlayerState.DeathAnimation) {
            this.animation.Stop();
            this.deathTimer += Time.deltaTime;
            this.head.transform.Rotate (Vector3.right * (1.0f - deathTimer / 2.9f) * 120); 
        }
        if (this.transform.position.y < -1 && this.state != PlayerState.DeathAnimation && this.state != PlayerState.Death) {
            this.Death (false);
        }
    }
    
    bool IsGrounded () {
        Ray floorRay = new Ray (this.transform.position + Vector3.down * this.transform.localScale.y / 2.0f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast (floorRay, out hit, 0.5f)) {
            return true;
        }
        return false;
    }
    
    public bool IsDead () {
        return this.state == PlayerState.DeathAnimation || this.state == PlayerState.Death;
    }
    
    void Death (bool damage) {
        if (this.IsDead()) return;
        Destroy (shockEffect);
        shockEffect = null;
        LineRenderer renderer = this.GetComponent<LineRenderer> (); 
        renderer.enabled = false;  
        GameObject controller = GameObject.FindWithTag ("GameController");
        controller.SendMessage ("StopMainMusic");
        controller.SendMessage ("PlaySound", "Sounds/gameover0");
        this.state = PlayerState.DeathAnimation;
        if (damage) {
            this.SetControl (false);
            this.audio.volume = 0;
        } else {
            controller.SendMessage ("Miss", this.playerNumber);
        }
        StartCoroutine(this.Miss());
    }
    
    void DestroyBody () {
        /*Destroy(this.GetComponent<PlatformInputController>());
        Destroy(this.GetComponent<CharacterMotor>());
        Destroy(this.GetComponent<CharacterController>());*/
        Destroy (this.transform.Find ("Armature").gameObject);
        foreach (Transform part in this.transform) {
            part.gameObject.AddComponent ("Rigidbody");
            if (part.GetComponent<BoxCollider> () != null) {
                part.GetComponent<BoxCollider> ().enabled = true;
            }
            part.rigidbody.velocity = Random.onUnitSphere;
        }
    }
    
    void SetControl (bool c) {
        this.canControl = c;
        this.GetComponent<CharacterMotor>().enabled = c;
        this.GetComponent<JoyStickController>().cameraControl.horizontalControl.enabled = c;
        this.GetComponent<JoyStickController>().cameraControl.verticalControl.enabled = c;
        if (!c) this.audio.volume = 0;
    }
    
    public PlayerState GetPlayerState () {
        return this.state;
    }
    
    void SetClearState () {
        LineRenderer renderer = this.GetComponent<LineRenderer> (); 
        renderer.enabled = false;  
        this.SetControl(false);
        this.state = PlayerState.Clear;
        JoyStickController controller = this.GetComponent<JoyStickController>();
        controller.cameraControl.distance = 1.5f;
        controller.cameraControl.height = 0.5f;
        Vector3 angle = controller.GetWantedCameraAngle();
        angle.y = -5;
        controller.SetWantedCameraAngle(angle);
        StartCoroutine(this.PlayClearMotion());
    }
    
    IEnumerator PlayClearMotion () {
        yield return new WaitForSeconds(1.0f);
        GameObject.FindWithTag("GameController").SendMessage("PlaySound", "Sounds/clear");
        this.animation.Play("clear");
    }
    
    IEnumerator Miss () {
        yield return new WaitForSeconds(2.9f);
        this.state = PlayerState.Death;
        this.DestroyBody ();
        GameObject controller = GameObject.FindWithTag ("GameController");
        controller.SendMessage ("Miss", this.playerNumber);
        this.SetControl(false);
        this.deathTimer = 0.0f;
    }
}

