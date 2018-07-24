using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Row : MonoBehaviour {

	public List<GameObject> tilesInRow; //have rows cause we can't have 2d arrays that are serializable


	void initialisationCheck(){
		if (tilesInRow == null) {
			tilesInRow = new List<GameObject> ();
		}
	}

	public void addChunkToEnd(TileChunk tc)
	{
		initialisationCheck ();
		tilesInRow.Add (tc.gameObject);
	}

	public void addChunkToStart(TileChunk tc)
	{
		initialisationCheck ();
		tilesInRow.Insert (0, tc.gameObject);
	}

	public void clearRow()
	{
		tilesInRow = new List<GameObject> ();
	}

	public List<TileChunk> getTilesAsChunks() //gets the chunks in a row
	{
		if (tilesInRow == null) {
			return null;
		}

		List<TileChunk> retVal = new List<TileChunk> ();
		foreach (GameObject g in tilesInRow) {
			retVal.Add(g.GetComponent<TileChunk>());
		}
		return retVal;
	}

	public void hideBlankTiles()
	{
		foreach (TileChunk tc in getTilesAsChunks()) {
			tc.disableUnusedTiles ();
		}
	}

	public void showBlankTiles()
	{
		foreach (TileChunk tc in getTilesAsChunks()) {
			tc.enableUnusedTiles ();
		}
	}
}
