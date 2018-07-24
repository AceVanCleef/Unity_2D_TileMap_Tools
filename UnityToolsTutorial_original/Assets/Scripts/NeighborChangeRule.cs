using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborChangeRule : MonoBehaviour {

	public Sprite myTile,neighbourTile,changeTo;
	public myDir directionToTile;//have to be in this direction from the original tile e.g. if it was (0,1,0) the second tile would have to be above the original tile
	//public NeighborChangeRule(Sprite tile,Sprite neighbour,Sprite newOne,myDir direction)
	//{
	//	myTile = tile;
	//	neighbourTile = neighbour;
	//	changeTo = newOne;
	//	directionToTile = direction;
	//}

	public void initialiseRule(Sprite tile,Sprite neighbour,Sprite newOne,myDir direction)
	{
		myTile = tile;
		neighbourTile = neighbour;
		changeTo = newOne;
		directionToTile = direction;
	}

	public void checkRule(GameObject tileToCheck,GameObject neighbour,myDir dir)
	{
		SpriteRenderer sr = tileToCheck.GetComponent<SpriteRenderer> ();
		SpriteRenderer sr2 = neighbour.GetComponent<SpriteRenderer> ();
		//Vector3 myDirection = sr.transform.position - sr2.transform.position;//need to work out some way to implement


		if (sr.sprite == myTile && sr2.sprite == neighbourTile && dir == directionToTile) {
			sr.sprite = changeTo;
		}
	}


}

public enum myDir{
	up,
	down,
	left,
	right,
	invalid
}