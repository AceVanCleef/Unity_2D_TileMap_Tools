using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimation  : ProgrammableTiles {
	


	public List<Sprite> mySprites;
	public float timerReset = 0.0f;
	float timer = 0.0f;
	int counter=0;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0) {
			this.GetComponent<SpriteRenderer> ().sprite = mySprites [counter];
			if (counter < mySprites.Count-1) {
				counter++;
			} else {
				counter = 0;
			}
			timer = timerReset;
		}
	}

	public void initialise(List<Sprite> sprites,float timer)
	{
		mySprites = sprites;
		timerReset = timer;
	}

	public override void OnPlaceTile ()
	{
		
	}

	public override string getInfoOnTile (){
		return "Tile animation with " + mySprites.Count + " frames at " + timer.ToString() + " seconds per frame";
	}
}
