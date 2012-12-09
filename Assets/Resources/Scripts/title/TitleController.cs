using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {

    private bool pressed = false;
    private Texture2D logoTexture = null;
    private Texture2D pressTexture = null;

    // Use this for initialization
    void Start () {
        logoTexture = (Texture2D)Resources.Load ("UI/logo", typeof(Texture2D));
        pressTexture = (Texture2D)Resources.Load ("UI/start", typeof(Texture2D));
        Debug.Log(logoTexture);
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Start") && !this.pressed) {
            AudioClip clip = (AudioClip)Resources.Load("Sounds/start");
            this.audio.PlayOneShot(clip);
            this.pressed = true;
            StartCoroutine(this.NextScene());
        }
    }
    
    IEnumerator NextScene () {
        yield return new WaitForSeconds(3.0f);
        Application.LoadLevel("MainScene");
    }
    
    void OnGUI () {
        float width = this.logoTexture.width * 1.5f;
        float height = this.logoTexture.height * 1.5f;
        GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 2.0f, width, height), this.logoTexture);
        width = this.pressTexture.width * 0.75f;
        height = this.pressTexture.height * 0.75f;
        GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 1.5f, width, height), this.pressTexture);
    }
}