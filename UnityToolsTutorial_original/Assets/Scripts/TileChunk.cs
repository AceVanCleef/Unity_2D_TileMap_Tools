using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TileChunk : MonoBehaviour{
	//2d arrays are not serializable
	public Vector2 startingCoords;//starting point of the chunk in world coords, tiles are placed to the right and up from this point
	public Tile[] tiles_0,tiles_1,tiles_2,tiles_3,tiles_4; //have to use 5 seperate arrays because 2d arrays are not serializable, these essentially act as the y coord
	public void initialiseChunk(Vector2 coords)
	{
		tiles_0 = new Tile[5];
		tiles_1 = new Tile[5];
		tiles_2 = new Tile[5];
		tiles_3 = new Tile[5];
		tiles_4 = new Tile[5];
		startingCoords = coords;
	}

	public void setTile(int x,int y,Tile t) //adds tiles to the array
	{
		switch (y) {
		case 0:
			tiles_0 [x] = t;
			break;
		case 1:
			tiles_1 [x] = t;
			break;
		case 2:
			tiles_2 [x] = t;
			break;
		case 3:
			tiles_3 [x] = t;
			break;
		case 4:
			tiles_4 [x] = t;
			break;
		default:
			break;
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());
	}

	public void setTile(int x,int y,GameObject t,bool isBlank) //adds tiles to the array
	{

		x-= Mathf.RoundToInt( startingCoords.x);
		y-= Mathf.RoundToInt( startingCoords.y);


		//Debug.Log ("Mouse in scene " + LevelCreator.me.mouseInScene);

		//Debug.Log ("X Index = " + LevelCreator.me.activeLayer.xIndex + " Y index = " + LevelCreator.me.activeLayer.yIndex);

		//Debug.Log (" Start X = " + startingCoords.x + " Start Y = " + startingCoords.y);

		//Debug.Log (" TC2 Mouse X = " + x + " Mouse Y = " + y);

		GameObject old;
		GameObject tile;
		ProgrammableTiles[] tiles;
		int order = 0;
		switch (y) {
		case 0:
			Debug.Log ("0 is  " + x);
			old = tiles_0 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_0 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_0 [x].setSortingOrder (order);
			tiles_0 [x].myTile = tiles_0 [x].gameObject;
			tiles_0 [x].isBlank = isBlank;

			tiles_0 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 1:
			old = tiles_1 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_1 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_1 [x].setSortingOrder (order);
			tiles_1 [x].myTile = tiles_1 [x].gameObject;
			tiles_1 [x].isBlank = isBlank;

			tiles_1 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 2:
			old = tiles_2 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_2[x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_2 [x].setSortingOrder (order);
			tiles_2 [x].myTile = tiles_2 [x].gameObject;
			tiles_2 [x].isBlank = isBlank;

			tiles_2 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 3:
			old = tiles_3 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_3 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_3 [x].setSortingOrder (order);
			tiles_3 [x].transform.position = new Vector3 (startingCoords.x + x, startingCoords.y + y, 0);
			tiles_3 [x].myTile = tiles_3 [x].gameObject;
			tiles_3 [x].isBlank = isBlank;

			tiles_3 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 4:
			old = tiles_4 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_4[x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_4 [x].setSortingOrder (order);
			//tiles_4 [x].transform.position = new Vector3 (startingCoords.x + x, startingCoords.y + y, 0);
			tiles_4 [x].myTile = tiles_4 [x].gameObject;
			tiles_4 [x].isBlank = isBlank;
			tiles_4 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		default:
			break;
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());


	}


	public void setTile(int x,int y,GameObject t) //adds tiles to the array
	{

		//x-= Mathf.RoundToInt( startingCoords.x);
		//y-= Mathf.RoundToInt( startingCoords.y);


		//Debug.Log ("Mouse in scene " + LevelCreator.me.mouseInScene);

		//Debug.Log ("X Index = " + LevelCreator.me.activeLayer.xIndex + " Y index = " + LevelCreator.me.activeLayer.yIndex);

		//Debug.Log (" Start X = " + startingCoords.x + " Start Y = " + startingCoords.y);

		//Debug.Log (" TC2 Mouse X = " + x + " Mouse Y = " + y);

		GameObject old;
		GameObject tile;
		ProgrammableTiles[] tiles;
		int order = 0;
		switch (y) {
		case 0:
			Debug.Log ("0 is  " + x);
			old = tiles_0 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_0 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_0 [x].setSortingOrder (order);
			tiles_0 [x].myTile = tiles_0 [x].gameObject;
			//tiles_0 [x].isBlank = isBlank;

			tiles_0 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 1:
			old = tiles_1 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_1 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_1 [x].setSortingOrder (order);
			tiles_1 [x].myTile = tiles_1 [x].gameObject;
			//tiles_1 [x].isBlank = isBlank;

			tiles_1 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 2:
			old = tiles_2 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_2[x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_2 [x].setSortingOrder (order);
			tiles_2 [x].myTile = tiles_2 [x].gameObject;
		//	tiles_2 [x].isBlank = isBlank;

			tiles_2 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 3:
			old = tiles_3 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_3 [x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_3 [x].setSortingOrder (order);
			tiles_3 [x].transform.position = new Vector3 (startingCoords.x + x, startingCoords.y + y, 0);
			tiles_3 [x].myTile = tiles_3 [x].gameObject;
		//	tiles_3 [x].isBlank = isBlank;

			tiles_3 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		case 4:
			old = tiles_4 [x].gameObject;
			tile = (GameObject) Instantiate (t, old.transform.position, old.transform.rotation);
			tiles_4[x] = tile.GetComponent<Tile> ();
			order = old.GetComponent<SpriteRenderer> ().sortingOrder;
			tiles_4 [x].setSortingOrder (order);
			//tiles_4 [x].transform.position = new Vector3 (startingCoords.x + x, startingCoords.y + y, 0);
			tiles_4 [x].myTile = tiles_4 [x].gameObject;
		//	tiles_4 [x].isBlank = isBlank;
			tiles_4 [x].gameObject.transform.parent = old.transform.parent;
			tiles = tile.GetComponents<ProgrammableTiles>();
			if (tiles == null) {

			} else {
				foreach(ProgrammableTiles tl in tiles)
				{
					tl.OnPlaceTile();
				}
			}

			DestroyImmediate (old.gameObject);
			break;
		default:
			break;
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());


	}

	public Tile getTile(Vector3 posOfMouse)//posOfMouse is the mouse position in world coords
	{
		//if (posOfMouse.x < startingCoords.x || posOfMouse.y < startingCoords.y || posOfMouse.x > (startingCoords.x + 5) || posOfMouse.y > (startingCoords.y + 5)) {
		//	return null;//position not part of this chunk as the chunks start at the bottom left corner
		//}

		try{
			posOfMouse.x -= startingCoords.x; //if we take the starting coords from the position of the mouse in the world then this gives 2 numbers between 0 and 4 which we can round into indexes for accessing the array
			posOfMouse.y -= startingCoords.y;

			int x = Mathf.RoundToInt (posOfMouse.x);
			int y = Mathf.RoundToInt (posOfMouse.y);//TODO PLAY ARROUND WITH ROUNDING TO MAKE MORE ACCURATE
			Debug.Log ("TC Mouse X = " + x + " Mouse Y = " + y);

			switch (y) {
			case 0:
				return tiles_0 [x];
				break;
			case 1:
				return tiles_1 [x];
				break;
			case 2:
				return tiles_2 [x];
				break;
			case 3:			
				return tiles_3 [x];
				break;
			case 4:
				return tiles_4 [x];
				break;
			default:
				return tiles_4 [x];
				break;
			}
		}
		catch{
			return null;
		}
	}

	public Tile getTile(int x,int y)
	{
		switch (y) {
		case 0:
			return tiles_0 [x];
			break;
		case 1:
			return tiles_1 [x];
			break;
		case 2:
			return tiles_2 [x];
			break;
		case 3:			
			return tiles_3 [x];
			break;
		case 4:
			return tiles_4 [x];
			break;
		default:
			return tiles_4 [x];
			break;
		}
	}

	public void mergeTileChunks(TileChunk toMergeWith){
		//Debug.Log ("merging some chunks " + toMergeWith.gameObject.name + " " + this.gameObject.name);
		for (int y = 0; y < 5; y++) {
			for (int x = 0; x < 5; x++) {
				Tile t = toMergeWith.getTile (x, y);
				if (t.isBlank == false) {
					setTile (x, y, t.gameObject);
				} 
			}
		}
	}

	public void paintTilesInArea(Vector3 pos1,Vector3 pos2,Sprite s)
	{
		Vector3 lowest = new Vector3 (Mathf.Min (pos1.x, pos2.x), Mathf.Min (pos1.y, pos2.y));
		Vector3 highest = new Vector3 (Mathf.Max (pos1.x, pos2.x), Mathf.Max (pos1.y, pos2.y));

		List<Tile> myTiles = getAllTiles ();

		foreach (Tile t in myTiles) {
			Vector3 myPos = t.gameObject.transform.position;
			if (myPos.x >= lowest.x && myPos.y >= lowest.y && myPos.x <= highest.x && myPos.y <= highest.y) {	
				t.setTileSprite (s);

			}
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void paintTilesInArea(Vector3 pos1,Vector3 pos2,GameObject s,bool isBlank)
	{
		Vector3 lowest = new Vector3 (Mathf.Min (pos1.x, pos2.x), Mathf.Min (pos1.y, pos2.y));
		Vector3 highest = new Vector3 (Mathf.Max (pos1.x, pos2.x), Mathf.Max (pos1.y, pos2.y));

		lowest.x -= 0.25f;
		lowest.y -= 0.25f;
		highest.x += 0.25f;
		highest.y += 0.25f;

		List<Tile> myTiles = getAllTiles ();

		foreach (Tile t in myTiles) {
			Vector3 myPos = t.gameObject.transform.position;
			if (myPos.x >= lowest.x && myPos.y >= lowest.y && myPos.x <= highest.x && myPos.y <= highest.y) {
				int x = Mathf.RoundToInt (myPos.x);
				int y = Mathf.RoundToInt (myPos.y);
				setTile (x, y, s,isBlank);
			}
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void paintTilesIfNear(Vector3 pos1,Vector3 pos2,GameObject s,bool isBlank,Vector3 posTobeNear,float radius)
	{
		//Vector3 lowest = new Vector3 (Mathf.Min (pos1.x, pos2.x), Mathf.Min (pos1.y, pos2.y));
		//Vector3 highest = new Vector3 (Mathf.Max (pos1.x, pos2.x), Mathf.Max (pos1.y, pos2.y));
		List<Tile> myTiles = getAllTiles ();
		foreach(Tile t in myTiles)
		{
			Vector3 myPos = t.gameObject.transform.position;

			//(x - center_x)^2 + (y - center_y)^2 < radius^2
			float val1 = (myPos.x - posTobeNear.x) * (myPos.x - posTobeNear.x);
			float val2 = (myPos.y - posTobeNear.y) * (myPos.y - posTobeNear.y);
			if ((val1 + val2) < (radius * radius)) {
				int x = Mathf.RoundToInt (myPos.x);
				int y = Mathf.RoundToInt (myPos.y);
				setTile (x, y, s, isBlank);
			}


		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}


	public void spraycanPaintTiles(Vector3 pos1,Vector3 pos2,GameObject s,bool isBlank)
	{
		Vector3 lowest = new Vector3 (Mathf.Min (pos1.x, pos2.x), Mathf.Min (pos1.y, pos2.y));
		Vector3 highest = new Vector3 (Mathf.Max (pos1.x, pos2.x), Mathf.Max (pos1.y, pos2.y));

		List<Tile> myTiles = getAllTiles ();

		foreach (Tile t in myTiles) {
			int r = UnityEngine.Random.Range (0, 100); //had error saying there was ambiguity between unitys random class and 
			if (r >= LevelCreator.me.spraycanDensity) {
				continue;
			}
			Vector3 myPos = t.gameObject.transform.position;
			if (myPos.x >= lowest.x && myPos.y >= lowest.y && myPos.x <= highest.x && myPos.y <= highest.y) {
				int x = Mathf.RoundToInt (myPos.x);
				int y = Mathf.RoundToInt (myPos.y);
				setTile (x, y, s,isBlank);
			}
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}



	public List<Tile> getAllTiles() //gets the tiles
	{
		List<Tile> retVal = new List<Tile> ();

		foreach (Tile t in tiles_0) {
			retVal.Add (t);
		}

		foreach (Tile t in tiles_1) {
			retVal.Add (t);
		}

		foreach (Tile t in tiles_2) {
			retVal.Add (t);
		}
		foreach (Tile t in tiles_3) {
			retVal.Add (t);
		}
		foreach (Tile t in tiles_4) {
			retVal.Add (t);
		}
		return retVal;
	}

	public List<Tile> getTilesInArea(Vector3 pos1,Vector3 pos2)//new for programmable tiles
	{
		List<Tile> retVal = new List<Tile> ();
		Vector3 lowest = new Vector3 (Mathf.Min (pos1.x, pos2.x), Mathf.Min (pos1.y, pos2.y));
		Vector3 highest = new Vector3 (Mathf.Max (pos1.x, pos2.x), Mathf.Max (pos1.y, pos2.y));

		List<Tile> myTiles = getAllTiles ();

		foreach (Tile t in myTiles) {
			Vector3 myPos = t.gameObject.transform.position;
			if (myPos.x >= lowest.x && myPos.y >= lowest.y && myPos.x <= highest.x && myPos.y <= highest.y) {
				retVal.Add (t);
			}
		}

		return retVal;
	}

	public void resetChunk()
	{
		tiles_0 = new Tile[5];
		tiles_1 = new Tile[5];
		tiles_2 = new Tile[5];
		tiles_3 = new Tile[5];
		tiles_4 = new Tile[5];
	}

	public void disableUnusedTiles()
	{
		foreach (Tile t in tiles_0) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (false);
			}
		}

		foreach (Tile t in tiles_1) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (false);
			}
		}

		foreach (Tile t in tiles_2) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (false);
			}
		}

		foreach (Tile t in tiles_3) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (false);
			}
		}

		foreach (Tile t in tiles_4) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (false);
			}
		}
	}

	public void enableUnusedTiles()
	{
		foreach (Tile t in tiles_0) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (true);
			}
		}

		foreach (Tile t in tiles_1) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (true);
			}
		}

		foreach (Tile t in tiles_2) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (true);
			}
		}

		foreach (Tile t in tiles_3) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (true);
			}
		}

		foreach (Tile t in tiles_4) {
			if (t.isBlank == true) {
				t.gameObject.SetActive (true);
			}
		}
	}


}
