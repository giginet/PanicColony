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
        AudioClip clip = (AudioClip)Resources.Load("Sounds/title");
        this.audio.clip = clip;
        this.audio.loop = true;
        this.audio.Play();
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Start") && !this.pressed) {
            this.audio.Stop();
            AudioClip clip = (AudioClip)Resources.Load("Sounds/decide");
            this.audio.PlayOneShot(clip);
            this.pressed = true;
            GameObject explosion = (GameObject)Resources.Load("Prefabs/titleExplosionPrefab");
            Instantiate(explosion, new Vector3(5.31f, -4.588f, 47.17f), Quaternion.identity);
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
        float alpha = 1.0f;
        if (this.pressed) {
            alpha = 0.25f + Mathf.PingPong(Time.time * 10, 0.75f);
        } else {
            alpha = 0.25f + Mathf.PingPong(Time.time / 2.0f, 0.75f);
        }
        Color previous = GUI.color;
        Color color = new Color(previous.r, previous.g, previous.b, alpha);
        GUI.color = color;
        width = this.pressTexture.width * 0.75f;
        height = this.pressTexture.height * 0.75f;
        GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 1.5f, width, height), this.pressTexture);
        GUI.color = previous;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 12;
        GUI.Label(new Rect((Screen.width - 400) / 2, Screen.height - 40, 400, 20), "(c)2009-2012 Kawaz All right reserved.", style);    
    } 
}