using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile  : ProgrammableTiles {
	public List<Sprite> mySprites;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initialise(List<Sprite> sprites)
	{
		mySprites = sprites;
	}

	public override void OnPlaceTile ()
	{
		int r = Random.Range (0, mySprites.Count - 1);
		Debug.Log ("Placing random tile " + r);
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = mySprites [r];
	}

	public override string getInfoOnTile (){
		return "Tile randomiser with " + mySprites.Count + " Possibilities.";
	}
}
