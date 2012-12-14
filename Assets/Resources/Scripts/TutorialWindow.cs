using UnityEngine;
using System.Collections;

public class TutorialWindow {

    private int page = 0;
    private int maxPage = 0;
    private Rect rect;
    private string prefix = null;
    private Texture2D texture;
 
    public TutorialWindow (string prefix, int maxPage, Rect rect) {
        this.prefix = prefix;
        this.maxPage = maxPage;
        this.rect = rect;
        this.page = 0;
        this.texture = this.LoadTexture(this.page);
    }

    public void Draw () {
        GUI.DrawTexture(this.rect, this.texture, ScaleMode.ScaleToFit, true, this.rect.width / this.rect.height);
    }
    
    public void Next () {
        if (this.page < this.maxPage) {
            this.page += 1;
            this.texture = this.LoadTexture(this.page);
        }
    }
    
    public int GetPage () {
        return this.page;
    }
    
    private Texture2D LoadTexture (int page) {
        string filename = this.prefix + page.ToString();
        return (Texture2D)Resources.Load(filename, typeof(Texture2D));
    }

}
