using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourChange : ProgrammableTiles {

	public List<NeighborChangeRule> myRules;

	public override void OnPlaceTile ()
	{

		//gets all surrounding tiles of selected tile

		int sizeOfArea = 4;//want to check adjacent tiles to the one we're placing
		int xChunkLowBound = LevelCreator.me.activeLayer.xIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunkLowBound = LevelCreator.me.activeLayer.yIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);

		int xChunkHighBound = LevelCreator.me.activeLayer.xIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunHighBound = LevelCreator.me.activeLayer.yIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);

		Vector3 topRightCorner = LevelCreator.me.activeLayer.mouseInScene + new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);
		Vector3 bottomLeftCorner = LevelCreator.me.activeLayer.mouseInScene - new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);

		for (int x = xChunkLowBound; x < xChunkHighBound; x++) {
			for (int y = yChunkLowBound; y < yChunHighBound; y++) {
				if (x >= 0 && y >= 0 && x< LevelCreator.me.activeLayer.rowsOfTiles[0].GetComponent<Row>().getTilesAsChunks().Count && y<LevelCreator.me.activeLayer.rowsOfTiles.Count) { //can't do if were out of range
					List<Tile> nearby = LevelCreator.me.activeLayer.rowsOfTiles[y].GetComponent<Row>().getTilesAsChunks()[x].getTilesInArea(topRightCorner,bottomLeftCorner);
					foreach (Tile t in nearby) {
						if (Vector3.Distance (t.gameObject.transform.position, this.gameObject.transform.position) < 1.5f) {
							//t.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue; //temp until propperly implemented
							testForRuleMatch(t.gameObject);
						}
					}
				}
			}
		}
	}


	void testForRuleMatch(GameObject neighbour)
	{
		Sprite s = neighbour.GetComponent<SpriteRenderer> ().sprite;
		myDir dir = getDirFromNeighbour (neighbour);

		if (dir != myDir.invalid) {
			foreach (NeighborChangeRule r in myRules) {
				r.checkRule (this.gameObject, neighbour,dir);
			}
		}
	}

	myDir getDirFromNeighbour(GameObject neighbour)
	{
		if (neighbour.transform.position.x > this.gameObject.transform.position.x) {
			//to the right

			if (neighbour.transform.position.y != this.gameObject.transform.position.y) {
				return myDir.invalid;
			} else {
				return myDir.right;
			}

		} else if (neighbour.transform.position.x < this.gameObject.transform.position.x) {
			//to the left

			if (neighbour.transform.position.y != this.gameObject.transform.position.y) {
				return myDir.invalid;
			} else {
				return myDir.left;
			}
		} else {
			//either above or below

			if (neighbour.transform.position.y > this.gameObject.transform.position.y) {
				return myDir.up;
			} else {
				return myDir.down;
			}
		}
	}

	public void addRule(Sprite s1,Sprite s2,Sprite s3,myDir dir)
	{
		if (myRules == null) {
			myRules = new List<NeighborChangeRule> ();
		}

		NeighborChangeRule ncr = this.gameObject.AddComponent<NeighborChangeRule> ();
		ncr.initialiseRule (s1, s2, s3, dir);
		myRules.Add(ncr);
	}

	public override string getInfoOnTile (){
		return "Neighbour change tile with " + myRules.Count + " rules.";
	}
}
