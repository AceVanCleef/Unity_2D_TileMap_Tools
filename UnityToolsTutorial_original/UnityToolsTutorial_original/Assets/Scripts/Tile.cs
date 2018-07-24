using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile : MonoBehaviour {
	public bool isBlank = true;//acts as a marker, shows the tile doesn't have anything on it, its just representing where a tile could be
	public GameObject myTile;

	public void setTileSprite(Sprite toSet)
	{
		this.GetComponent<SpriteRenderer> ().sprite = toSet;
		isBlank = false;
	}

	public void reInitialise(int sortingOrder)
	{
		isBlank = true;
		SpriteRenderer sr = this.GetComponent<SpriteRenderer> ();
		sr.sortingOrder = sortingOrder;
		sr.sprite = LevelCreator.me.blankSprite;
	}

	public void setSortingOrder(int sortingOrder)
	{
		SpriteRenderer sr = this.GetComponent<SpriteRenderer> ();
		sr.sortingOrder = sortingOrder;
	}


}
