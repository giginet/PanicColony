using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour {
	public int WIDTH = 10;
	public int HEIGHT = 10;
	private Level level;

	// Use this for initialization
	void Start () {
		this.CreateLevel(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void CreateLevel (int levelNo) {
		Dictionary<Vector2, char> map = new Dictionary<Vector2, char>();
		TextAsset asset = (TextAsset)Resources.Load("Levels/Level" + levelNo.ToString(), typeof(TextAsset));
		GameObject level = GameObject.CreatePrimitive(PrimitiveType.Cube);
		level.renderer.enabled = false;
		level.name = "Level" + levelNo;
		string[] lines = asset.text.Split( '\n');
		int width = 0;
		int height = lines.Length;
		for (int y = 0; y < lines.Length; ++y) {
			string line = lines[y];
			for (int x = 0; x < line.Length; ++x) {
				if (line.Length > width) width = line.Length;
				char c = line[x];
				map.Add(new Vector2(x, y), c);
			}
		}
		this.level = new Level(levelNo, width, height, map);
		for (int y = 0; y < lines.Length; ++y) {
			string line = lines[y];
			for (int x = 0; x < line.Length; ++x) {
				Vector3 position = new Vector3(x * WIDTH, 0, -y * HEIGHT);
				char c = map[new Vector2(x, y)];
				if (c == '.' || c == 'S') {
					GameObject floorPrefab = (GameObject)Resources.Load("Prefabs/floorPrefab", typeof(GameObject));
					GameObject floor = (GameObject)Instantiate(floorPrefab, position, Quaternion.identity);
					floor.transform.parent = level.transform;
				}
				if (c == '/') {
					GameObject routePrefab = (GameObject)Resources.Load("Prefabs/routePrefab", typeof(GameObject));
					GameObject route = (GameObject)Instantiate(routePrefab, position, Quaternion.identity);
					route.transform.parent = level.transform;
					// if route is placed holizontally, rotate object.
					if (this.level.GetMap(x - 1, y) == '/' || this.level.GetMap(x + 1, y) == '/') {
						route.transform.Rotate(new Vector3(0, 90, 0));
					}
				} else if (c == '#') {
					GameObject wallPrefab = (GameObject)Resources.Load("Prefabs/wallPrefab", typeof(GameObject));
					GameObject wall = (GameObject)Instantiate(wallPrefab, position + Vector3.up * 1, Quaternion.identity);
					wall.transform.parent = level.transform;
				} else if (c == 'S') {
					GameObject player = GameObject.FindWithTag("Player");
					player.transform.position = position + Vector3.up;
				}
			}
		}
	}
}
