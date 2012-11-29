using UnityEngine;
using System.Collections;

public class JoyStickController : MonoBehaviour {
    
    [System.Serializable]
    public class CameraControl {
        public bool enabled = true;
        public bool rigidbodyEnabled = false;
        public Transform cameraTransform = Camera.main.transform;
        public float distance = 3.0f;
        public float height = 5.0f;
        public float damping = 1.0f;
        public float rotationDamping = 3.0f;
        public CameraRotationControl horizontalControl;
        public CameraRotationControl verticalControl;
    }
    
    public class CharacterControl {
    }
    
    [System.Serializable]
    public class CameraRotationControl {
        public bool controllEnabled = true;
        public bool inverse = false;
        public float speed = 3.0f;
        public string axisName = "";
        public float minAngle = -75.0f;
        public float maxAngle = 75f;
    }
    
    public CameraControl cameraControl;
    public CharacterControl characterControl;
    private Vector3 wantedCameraAngle = Vector3.zero;
    
    // Use this for initialization
    void Start () {
        this.cameraControl.cameraTransform.position = this.transform.position + Vector3.up * this.cameraControl.height + this.transform.forward * -this.cameraControl.distance;
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        if (this.cameraControl.enabled) {
            this.ControlCamera();
        }
    }
    
    void ControlCamera () {
        // Calculate the current rotation angles
        Vector3 wantedCameraPosition = this.transform.position + Quaternion.Euler(this.wantedCameraAngle) * (this.transform.forward * -this.cameraControl.distance) + Vector3.up * this.cameraControl.height;
        Vector3 currentCameraPosition = this.cameraControl.cameraTransform.position;
        Quaternion wantedCameraRotation = Quaternion.LookRotation(this.transform.position - wantedCameraPosition);
        Quaternion currentCameraRotation = this.cameraControl.cameraTransform.rotation;

        float xAxis = Input.GetAxisRaw(this.cameraControl.horizontalControl.axisName);
        float yAxis = Input.GetAxisRaw(this.cameraControl.verticalControl.axisName);
        if (this.cameraControl.horizontalControl.controllEnabled && Mathf.Abs (xAxis) > 0.1) {
            this.wantedCameraAngle.y += xAxis * this.cameraControl.horizontalControl.speed * (this.cameraControl.horizontalControl.inverse ? 1 : -1);
            float min = this.cameraControl.horizontalControl.minAngle;
            float max = this.cameraControl.horizontalControl.maxAngle;
            if (min > -180 || max < 180) {
                this.wantedCameraAngle.y = Mathf.Clamp(this.wantedCameraAngle.y, min, max);
            }
        }
        if (this.cameraControl.verticalControl.controllEnabled && Mathf.Abs (yAxis) > 0.1) {
            this.wantedCameraAngle.x += yAxis * this.cameraControl.verticalControl.speed * (this.cameraControl.verticalControl.inverse ? 1 : -1);
            float min = this.cameraControl.verticalControl.minAngle;
            float max = this.cameraControl.verticalControl.maxAngle;
            this.wantedCameraAngle.x = Mathf.Clamp(this.wantedCameraAngle.x, min, max);
        }
        
        Vector3 cameraPosition = Vector3.Lerp(currentCameraPosition, wantedCameraPosition, this.cameraControl.damping * Time.deltaTime);
        Quaternion cameraRotation = Quaternion.Lerp(currentCameraRotation, wantedCameraRotation, this.cameraControl.rotationDamping * Time.deltaTime);
        
        if (this.cameraControl.rigidbodyEnabled && this.cameraControl.cameraTransform.gameObject.rigidbody != null) {
            this.cameraControl.cameraTransform.gameObject.rigidbody.MovePosition(cameraPosition);
            this.cameraControl.cameraTransform.gameObject.rigidbody.MoveRotation(cameraRotation);
        } else {
            this.cameraControl.cameraTransform.position = cameraPosition; 
            this.cameraControl.cameraTransform.rotation = cameraRotation;
        }   
    }
}
