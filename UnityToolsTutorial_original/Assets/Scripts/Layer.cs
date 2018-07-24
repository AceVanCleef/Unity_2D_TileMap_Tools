using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//BRUSHES 20-8-17
//changed index calculation from casting to rounding to make painting more accurate and stop bug where index was out of range



//16-5-17 Layers
//moved most of the tile code here & added references to the Level Creator so it can manipulate the active layer
//added singleton to the levelCreator so it can be accessed here for its references to the testSprite & tilePrefab, assigned where rows should be initialised + setSingleton method
//added code that made it so that the layers are now children of the editor and the rows are children of the layers
//had to change bools for painting here to reference level creator

//TODO: FIX LAYERS SERIALIZATION FOR NEXT EPISODDE,I THINK ITS BECAUSE ITS A LIST???
[Serializable]
public class Layer : MonoBehaviour {
	#if UNITY_EDITOR //mark the class so it only gets compiled if its being run in the editor as you cant build if you have scripts that use editor specidif features

	//public bool isInitialised;
	public GameObject tilePrefab;
	public List<GameObject> rowsOfTiles; //Gameobjects are a serializable type, makes it easier to save data, so we just save all our data in a script and then just keep a refernece to the gameobject then get the script when needed

	//used to work out the position of the cursor in relation to the grid to avoid having to cycle through all the tiles
	public Vector2 bottomLeftTile, topRightTile;
	public int widthInTiles, heightInTiles,widthInChunks,heightInChunks;
	public int xIndex=0,yIndex=0;

	//values for display, not actual
	public Vector3 mouseInScene=Vector3.zero;
	public string output = "";

	//bools for painting an area of tiles
	public bool assignedStart=false,assignedEnd = false;
	public Vector2 multiPaintStart, multiPaintEnd;
	public Vector3 multiPaintMouseStart, multiPaintMouseEnd;

	public GameObject startTile, endTile; //tiles for painting

	//new for layer
	public int layerSortingOrder = 0;

	public void getInfoOnGridDimensions()
	{
		widthInTiles = rowsOfTiles [0].GetComponent<Row> ().tilesInRow.Count * 5;
		heightInTiles = rowsOfTiles.Count * 5;
		widthInChunks = widthInTiles / 5;
		heightInChunks = heightInTiles / 5;
		bottomLeftTile = rowsOfTiles [rowsOfTiles.Count - 1].GetComponent<Row> ().tilesInRow [0].GetComponent<TileChunk> ().startingCoords;
		Row topRow = rowsOfTiles [0].GetComponent<Row> ();
		topRightTile = topRow.tilesInRow [topRow.tilesInRow.Count - 1].GetComponent<TileChunk> ().startingCoords;
		topRightTile = new Vector2 (topRightTile.x + 5, topRightTile.y + 5);

		calculateChunkMouseIsOver ();
	}


	public void calculateChunkMouseIsOver()//should give values that
	{
		xIndex = Mathf.RoundToInt(bottomLeftTile.x - mouseInScene.x);  //changed from casting to round to int
		xIndex= xIndex/5;
		xIndex *= -1;
		yIndex = Mathf.RoundToInt(bottomLeftTile.y - mouseInScene.y);
		yIndex= yIndex/5;
		yIndex *= -1;

		yIndex = heightInChunks - yIndex;
		yIndex -= 1;
		//Debug.Log ("X index = " + xIndex + " Y Index = " + yIndex);
		//writeOutput ();
	//X index is out of range on so when the x increases the y doesnt get rounded until the cursor is abit further towards the chunk
	}

	void writeOutput()
	{
	/*	Row r = rowsOfTiles [yIndex].GetComponent<Row> ();
		TileChunk tc = r.getTilesAsChunks () [xIndex];
		//Debug.Log (tc.startingCoords);
		Tile t = tc.getTile (mouseInScene);
		try{
			if (t == null) {
				Debug.Log("T is null");
			} else {
				output = tc.gameObject.name + " || Tile at " + t.transform.position;
			}
		}
		catch{
			Debug.Log ("Mouse is off grid");
		}*/
	}

	public void createInitialChunk()
	{
		initilisationCheck ();		
		createInitialRow ();
		createChunk (new Vector2 (0, 0), 0, false);
		EditorUtility.SetDirty (this);
	}

	void initilisationCheck() //makes sure that the rows are initialised
	{
		if (rowsOfTiles == null) {
			rowsOfTiles = new List<GameObject> ();
		}
	}

	public void shouldWePaintTile()
	{
		if (LevelCreator.me.paintTiles == true) {
			if (LevelCreator.me.paintSingle == true) {
				if (endTile != null) {
					endTile.GetComponent<SpriteRenderer> ().color = Color.white;
				}
				if (startTile != null) {
					startTile.GetComponent<SpriteRenderer> ().color = Color.white;
				}
				Event e = Event.current; //looks at current event (Keypresses, stuff like that))
				if (e.isMouse) { //if its a mouse click & alt is held then code executes
					if (e.type == EventType.MouseUp) {
						if (e.button == 0 && e.alt == true) {

							if (LevelCreator.me.paintSquare == true) {
								paintSquareArea ();
							} else if (LevelCreator.me.paintCircle == true) {
								paintCircleArea ();
							} else if (LevelCreator.me.spraycan == true) {
								paintSpraycan ();
							} else {
								Row r = rowsOfTiles [yIndex].GetComponent<Row> ();
								TileChunk tc = r.getTilesAsChunks () [xIndex];
								int x = Mathf.RoundToInt (mouseInScene.x);
								int y = Mathf.RoundToInt (mouseInScene.y);
									
								//Debug.Log ("Mouse X = " + x + " Mouse Y = " + y);
								tc.setTile (x, y, LevelCreator.me.currentTileToPaint, false);
								//Debug.Log (tc.startingCoords);
								/*Tile t = tc.getTile (mouseInScene);
								try {
									if (t == null) {
									} else {
										t.setTileSprite (LevelCreator.me.testSprite);
									}
								} catch {
								}*/
							}
						}
					}
				}
			} else if (LevelCreator.me.paintArea == true) {
				
				Event e = Event.current; //looks at current event (Keypresses, stuff like that))
				if (e.isMouse) { //if its a mouse click & alt is held then code executes
					if (e.type == EventType.MouseUp) {
						if (e.button == 0 && e.alt == true) {

							//set initial tile
							multiPaintStart = new Vector2 (xIndex, yIndex);
							multiPaintMouseStart = mouseInScene;
							

							Row myRow = rowsOfTiles [yIndex].GetComponent<Row> ();
							TileChunk tc = myRow.getTilesAsChunks () [xIndex];
							Tile myTile = tc.getTile (multiPaintMouseStart);
							if (startTile != null) {
								startTile.GetComponent<SpriteRenderer> ().color = Color.white;
							}
							startTile = myTile.myTile;
							startTile.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;
							assignedStart = true;
						} else if (e.button == 1 && e.alt == true) {
							multiPaintEnd = new Vector2 (xIndex, yIndex);
							multiPaintMouseEnd = mouseInScene;

							Row myRow = rowsOfTiles [yIndex].GetComponent<Row> ();
							TileChunk tc = myRow.getTilesAsChunks () [xIndex];
							Tile myTile = tc.getTile (multiPaintMouseEnd);
							if (endTile != null) {
								endTile.GetComponent<SpriteRenderer> ().color = Color.white;
							}
							endTile = myTile.myTile;
							endTile.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;

							assignedEnd = true;
						} else if (e.button == 2 && e.alt == true) {
							if (assignedStart == true && assignedEnd == true) {
								int startX = lowest ((int)multiPaintStart.x, (int)multiPaintEnd.x);
								int startY = lowest ((int)multiPaintStart.y, (int)multiPaintEnd.y);
								int endX = highest ((int)multiPaintStart.x, (int)multiPaintEnd.x);
								int endY = highest ((int)multiPaintStart.y, (int)multiPaintEnd.y);
								//sort the indexes so it goes from low to high
								//go through each chunk
								//write some method that paints tiles if they are 
								Debug.Log ("Start " + startX + " " + startY + " End " + endX + " " + endY);

								for (int x = startX; x <= endX; x++) {
									for (int y = startY; y <= endY; y++) {
										Row r = rowsOfTiles [y].GetComponent<Row> ();
										TileChunk tc = r.getTilesAsChunks () [x];
										tc.paintTilesInArea (multiPaintMouseStart, multiPaintMouseEnd, LevelCreator.me.currentTileToPaint, false);
										//tc.paintTilesInArea (multiPaintMouseStart, multiPaintMouseEnd, LevelCreator.me.testSprite);
									}
								}
								endTile.GetComponent<SpriteRenderer> ().color = Color.white;
								startTile.GetComponent<SpriteRenderer> ().color = Color.white;

								assignedEnd = false;
								assignedStart = false;

							} else {

								Debug.Log ("Start and end not assigned");
							}
						}
					}
				}
			} else if (LevelCreator.me.eraseSingle == true) {
				if (endTile != null) {
					endTile.GetComponent<SpriteRenderer> ().color = Color.white;
				}
				if (startTile != null) {
					startTile.GetComponent<SpriteRenderer> ().color = Color.white;
				}
				Event e = Event.current; //looks at current event (Keypresses, stuff like that))
				if (e.isMouse) { //if its a mouse click & alt is held then code executes
					if (e.type == EventType.MouseUp) {
						if (e.button == 0 && e.alt == true) {

							Row r = rowsOfTiles [yIndex].GetComponent<Row> ();
							TileChunk tc = r.getTilesAsChunks () [xIndex];
							int x = Mathf.RoundToInt (mouseInScene.x);
							int y = Mathf.RoundToInt (mouseInScene.y);

							tc.setTile (x, y, LevelCreator.me.tilePrefab, true);

						}
					}
				}
			} else if (LevelCreator.me.eraseArea == true) {
				Event e = Event.current; //looks at current event (Keypresses, stuff like that))
				if (e.isMouse) { //if its a mouse click & alt is held then code executes
					if (e.type == EventType.MouseUp) {
						if (e.button == 0 && e.alt == true) {

							//set initial tile
							multiPaintStart = new Vector2 (xIndex, yIndex);
							multiPaintMouseStart = mouseInScene;
							assignedStart = true;
							Row myRow = rowsOfTiles [yIndex].GetComponent<Row> ();
							TileChunk tc = myRow.getTilesAsChunks () [xIndex];

							Tile myTile = tc.getTile (multiPaintMouseStart);
							if (startTile != null) {
								startTile.GetComponent<SpriteRenderer> ().color = Color.white;
							}
							startTile = myTile.myTile;
							startTile.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;
						} else if (e.button == 1 && e.alt == true) {
							multiPaintEnd = new Vector2 (xIndex, yIndex);
							multiPaintMouseEnd = mouseInScene;
							assignedEnd = true;
							Row myRow = rowsOfTiles [yIndex].GetComponent<Row> ();
							TileChunk tc = myRow.getTilesAsChunks () [xIndex];
							Tile myTile = tc.getTile (multiPaintMouseEnd);
							if (endTile != null) {
								endTile.GetComponent<SpriteRenderer> ().color = Color.white;
							}
							endTile = myTile.myTile;
							endTile.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;
						} else if (e.button == 2 && e.alt == true) {
							if (assignedStart == true && assignedEnd == true) {
								int startX = lowest ((int)multiPaintStart.x, (int)multiPaintEnd.x);
								int startY = lowest ((int)multiPaintStart.y, (int)multiPaintEnd.y);
								int endX = highest ((int)multiPaintStart.x, (int)multiPaintEnd.x);
								int endY = highest ((int)multiPaintStart.y, (int)multiPaintEnd.y);
								//sort the indexes so it goes from low to high
								//go through each chunk
								//write some method that paints tiles if they are 
								Debug.Log ("Start " + startX + " " + startY + " End " + endX + " " + endY);

								for (int x = startX; x <= endX; x++) {
									for (int y = startY; y <= endY; y++) {
										Row r = rowsOfTiles [y].GetComponent<Row> ();
										TileChunk tc = r.getTilesAsChunks () [x];
										tc.paintTilesInArea (multiPaintMouseStart, multiPaintMouseEnd, LevelCreator.me.tilePrefab, true);
										//tc.paintTilesInArea (multiPaintMouseStart, multiPaintMouseEnd, LevelCreator.me.testSprite);
									}
								}
								assignedEnd = false;
								assignedStart = false;
								if (endTile != null) {
									endTile.GetComponent<SpriteRenderer> ().color = Color.white;
								}
								if (startTile != null) {
									startTile.GetComponent<SpriteRenderer> ().color = Color.white;
								}

							} else {

								Debug.Log ("Start and end not assigned");
							}
						}
					}
				}
			} else if (LevelCreator.me.paintPrefab == true) {
				Event e = Event.current; //looks at current event (Keypresses, stuff like that))
				if (e.isMouse) { //if its a mouse click & alt is held then code executes
					if (e.type == EventType.MouseUp) {
						if (e.button == 0 && e.alt == true) {
							MergeTileMaps ();
						}
					}
				}
			}
		}
	}

	int lowest(int a,int b)
	{
		if (a < b) {
			return a;
		} else {
			return b;
		}
	}

	int highest(int a,int b)
	{
		if (a > b) {
			return a;
		} else {
			return b;
		}
	}

	public void reset() //clears any existing tiles
	{
		foreach (GameObject g in rowsOfTiles) {
			Row r = g.GetComponent<Row> ();
			foreach (TileChunk tc in r.getTilesAsChunks()) {
				foreach (Tile t in tc.getAllTiles()) {
					DestroyImmediate (t.myTile);
				}
				tc.resetChunk ();
			}
			r.clearRow ();
			DestroyImmediate (g);
		}
		rowsOfTiles = new List<GameObject> ();
	}


	void createInitialRow() //creates one chunk & row
	{
		GameObject g = new GameObject ();
		g.name = "Row " + rowsOfTiles.Count;
		g.transform.parent = this.transform;
		Row r = g.AddComponent<Row> ();
		rowsOfTiles.Add (g);
	}


	void createRowAtBottom() //creates a row at the bottom of the existing tile map
	{
		GameObject g = new GameObject (); //create new row object
		
		g.transform.parent = this.transform;
		g.name = "Row " + rowsOfTiles.Count;
		g.AddComponent<Row> ();

		Row r = rowsOfTiles [rowsOfTiles.Count-1].GetComponent<Row> (); //store the last existing row
		List<TileChunk> tc = r.getTilesAsChunks ();
		Vector2 spawnPos = Vector2.zero;
		rowsOfTiles.Add (g); //add new rown to bottom



		for(int x =tc.Count-1;x>-1;x--) //add tile chunks to new row to the same dimensions as the old row
		{
			spawnPos = tc [x].startingCoords;
			spawnPos.y -= 5;
			createChunk (spawnPos,  rowsOfTiles.Count-1, true);

		}

	}

	void createRowAtTop()//identical to the create row at the bottom but does it at the top of the list instead
	{
		GameObject g = new GameObject ();
		 g.transform.parent = this.transform;

		g.name = "Row " + rowsOfTiles.Count;
		g.AddComponent<Row> ();


		Row r = rowsOfTiles [0].GetComponent<Row> ();
		List<TileChunk> tc = r.getTilesAsChunks ();
		Vector2 spawnPos = Vector2.zero;

		rowsOfTiles.Insert (0, g);

		for(int x =tc.Count-1;x>-1;x--)
		{
			spawnPos = tc [x].startingCoords;
			spawnPos.y += 5;
			createChunk (spawnPos, 0, true);

		}
	}



	void createChunk(Vector2 startCoords,int rowIndex,bool attachToEnd){ //creates a new chunk based on coord & row index provided
		GameObject chunkParent = new GameObject ();
		TileChunk tc = chunkParent.AddComponent<TileChunk> ();

		tc.initialiseChunk (startCoords);
		chunkParent.name = "Chunk || " + startCoords.ToString ();
		chunkParent.transform.parent = rowsOfTiles [rowIndex].transform;
		for (int x = 0; x < 5; x++) {
			for (int y = 0; y < 5; y++) {
				Vector3 posToSpawn = new Vector3 (startCoords.x + x, startCoords.y + y, 0.0f);
				GameObject g = (GameObject)Instantiate (LevelCreator.me.tilePrefab, posToSpawn, Quaternion.Euler (0, 0, 0));
				g.transform.parent = chunkParent.transform;
				Tile t = g.GetComponent<Tile> ();
				t.myTile = g;
				tc.setTile (x, y, t);
			}
		}
		if (attachToEnd == true) {
			rowsOfTiles [rowIndex].GetComponent<Row> ().addChunkToStart (tc);
		} else {
			rowsOfTiles [rowIndex].GetComponent<Row> ().addChunkToEnd (tc);

		}
	}


	public void ExpandGridLeft()//goes through all existing rows and adds a chunk to the left of each
	{

		foreach (GameObject r in rowsOfTiles) {
			Vector2 spawnPos = r.GetComponent<Row> ().getTilesAsChunks () [0].startingCoords;

			createChunk (new Vector2 (spawnPos.x-5, spawnPos.y),rowsOfTiles.IndexOf(r) , true);
		}
		EditorUtility.SetDirty (this);

	}

	public void ExpandGridRight() //same as left but adds it on the right
	{

		foreach (GameObject r in rowsOfTiles) {
			List<TileChunk> tc = r.GetComponent<Row> ().getTilesAsChunks ();
			Vector2 spawnPos = tc [tc.Count - 1].startingCoords;

			createChunk (new Vector2 (spawnPos.x+5, spawnPos.y),rowsOfTiles.IndexOf(r) , false);
		}

		EditorUtility.SetDirty (this);

	}

	public void ExpandGridTop() //methods for calling top/bottom
	{
		createRowAtTop ();

		EditorUtility.SetDirty (this);

	}

	public void ExpandGridBottom()
	{
		createRowAtBottom ();

		EditorUtility.SetDirty (this);

	}

	public void resetLayer(int newOrder)
	{
		foreach (GameObject g in rowsOfTiles) {
			Row r = g.GetComponent<Row> ();
			foreach (TileChunk tc in r.getTilesAsChunks()) {
				foreach (Tile t in tc.getAllTiles()) {
					t.reInitialise (newOrder);
				}
			}
		}
		layerSortingOrder = newOrder;
	}

	public void setSortingOrder(int newOrder)
	{
		foreach (GameObject g in rowsOfTiles) {
		Row r = g.GetComponent<Row> ();
			foreach (TileChunk tc in r.getTilesAsChunks()) {
				foreach (Tile t in tc.getAllTiles()) {
					t.setSortingOrder (newOrder);
				}
			}
		}
		layerSortingOrder = newOrder;
	}




	void paintSquareArea()
	{
		//Row r = rowsOfTiles [yIndex].GetComponent<Row> ();
		//TileChunk tc = r.getTilesAsChunks () [xIndex];
		//int x = Mathf.RoundToInt (mouseInScene.x);
		//int y = Mathf.RoundToInt (mouseInScene.y);
		
		int sizeOfArea = LevelCreator.me.brushSize;
		int xChunkLowBound = xIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunkLowBound = yIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);

		int xChunkHighBound = xIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunHighBound = yIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);

		Vector3 topRightCorner = mouseInScene + new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);
		Vector3 bottomLeftCorner = mouseInScene - new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);

		for (int x = xChunkLowBound; x < xChunkHighBound; x++) {
			for (int y = yChunkLowBound; y < yChunHighBound; y++) {
				if (x >= 0 && y >= 0 && x< rowsOfTiles[0].GetComponent<Row>().getTilesAsChunks().Count && y<rowsOfTiles.Count) { //can't do if were out of range
					rowsOfTiles[y].GetComponent<Row>().getTilesAsChunks()[x].paintTilesInArea(topRightCorner,bottomLeftCorner, LevelCreator.me.currentTileToPaint,false);
				}
			}
		}
	//get size of square
	//work out the number of chunks based on the size 1 chunk to 5 tiles (add one for good measure) (index + (size/5)+1)
	//work out the corners as a world position
	//go through the chunks and use the paint area to draw them

		//tc.setTile (x, y, LevelCreator.me.tilePrefab,true);
	}

	void paintCircleArea()
	{
		int sizeOfArea = LevelCreator.me.brushSize;
		int xChunkLowBound = xIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunkLowBound = yIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);

		int xChunkHighBound = xIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunHighBound = yIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);

		Vector3 centerTile = rowsOfTiles [yIndex].GetComponent<Row> ().getTilesAsChunks () [xIndex].getTile (mouseInScene).gameObject.transform.position;

		Vector3 topRightCorner = mouseInScene + new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);
		Vector3 bottomLeftCorner = mouseInScene - new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);


			for (int x = xChunkLowBound; x < xChunkHighBound; x++) {
				for (int y = yChunkLowBound; y < yChunHighBound; y++) {
					if (x >= 0 && y >= 0 && x< rowsOfTiles[0].GetComponent<Row>().getTilesAsChunks().Count && y<rowsOfTiles.Count) { //can't do if were out of range
						Row r = rowsOfTiles [y].GetComponent<Row> ();
						TileChunk tc = r.getTilesAsChunks() [x];
						tc.paintTilesIfNear(topRightCorner,bottomLeftCorner, LevelCreator.me.currentTileToPaint,false,centerTile, sizeOfArea/2);

					}
				}
			}
	}

	void paintSpraycan()
	{
		int sizeOfArea = LevelCreator.me.brushSize;
		int xChunkLowBound = xIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunkLowBound = yIndex - (Mathf.RoundToInt(sizeOfArea/2)+2);

		int xChunkHighBound = xIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);
		int yChunHighBound = yIndex + (Mathf.RoundToInt(sizeOfArea/2)+2);

		Vector3 topRightCorner = mouseInScene + new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);
		Vector3 bottomLeftCorner = mouseInScene - new Vector3 (sizeOfArea / 2, sizeOfArea / 2, 0.0f);

		for (int x = xChunkLowBound; x < xChunkHighBound; x++) {
			for (int y = yChunkLowBound; y < yChunHighBound; y++) {
				if (x >= 0 && y >= 0 && x< rowsOfTiles[0].GetComponent<Row>().getTilesAsChunks().Count && y<rowsOfTiles.Count) { //can't do if were out of range
					rowsOfTiles[y].GetComponent<Row>().getTilesAsChunks()[x].spraycanPaintTiles(topRightCorner,bottomLeftCorner, LevelCreator.me.currentTileToPaint,false);
				}
			}
		}
	}


	void MergeTileMaps()
	{
		if (LevelCreator.me.prefabToPaint == null) {
			EditorUtility.DisplayDialog ("Error", "You need to select a prefab to paint onto the current tile map", "Ok");
			return;
		}

		LevelCreator toPaint = LevelCreator.me.prefabToPaint.GetComponent<LevelCreator> ();

		foreach (GameObject l in toPaint.layersInMap) {
			Layer layerToPaint = l.GetComponent<Layer> ();
			Layer layerToPaintOn = this; //need a layer to paint on so that the compiler doesnt complain 
			bool doesOrderExist = false;//used to check if the layers have matching sorting orders, if they do the layers are merged else a new one is created
			foreach (GameObject l2 in LevelCreator.me.layersInMap) {
				Layer compareTo = l2.GetComponent<Layer> ();

				if (compareTo.layerSortingOrder == layerToPaint.layerSortingOrder) {
					doesOrderExist = true;
					layerToPaintOn = compareTo;//just store the layer once we have found it so we don't have to look again
					break;
				} else {
					doesOrderExist = false;
				}
			}

			if (doesOrderExist == true){
				int widthInChunks = layerToPaint.rowsOfTiles [0].GetComponent<Row> ().getTilesAsChunks ().Count;
				int heightOfLayerInChunks = layerToPaint.rowsOfTiles.Count;


				//int sizeOfArea = LevelCreator.me.brushSize;
				int xChunkLowBound = xIndex - (Mathf.RoundToInt(widthInChunks/2));
				int yChunkLowBound = yIndex - (Mathf.RoundToInt(heightOfLayerInChunks/2));

				int xChunkHighBound = xIndex + (Mathf.RoundToInt(widthInChunks/2));
				int yChunHighBound = yIndex + (Mathf.RoundToInt(heightOfLayerInChunks/2));
		
				
				for (int x = xChunkLowBound; x <= xChunkHighBound; x++) {
					for (int y = yChunkLowBound; y <= yChunHighBound; y++) {
						if (x >= 0 && y >= 0 && x < rowsOfTiles [0].GetComponent<Row> ().getTilesAsChunks ().Count && y < rowsOfTiles.Count && (y-yChunkLowBound) <= layerToPaint.rowsOfTiles.Count-1 && (x-xChunkLowBound)<=layerToPaint.rowsOfTiles[0].GetComponent<Row>().getTilesAsChunks().Count-1) { //can't do if were out of range
							Row rowOnTilemap = layerToPaintOn.rowsOfTiles [y].GetComponent<Row> (); //active tilemap we're merging with
							TileChunk tileChunkOnTilemap = rowOnTilemap.getTilesAsChunks () [x];
//							Debug.LogError ("Row on prefab " + (y - yChunkLowBound).ToString());
							Row rowOnPrefab = layerToPaint.rowsOfTiles [y - yChunkLowBound].GetComponent<Row> (); //prefab we're using
							TileChunk tileChunkOnPrefab = rowOnPrefab.getTilesAsChunks()[x-xChunkLowBound];
							//Debug.LogError ("Tile chunk = " + tileChunkOnPrefab.gameObject.name + " Is painted over = " + tileChunkOnTilemap.gameObject.name);
							tileChunkOnTilemap.mergeTileChunks (tileChunkOnPrefab);
						} 

						
					}
				}
			} else {
				GameObject newLayer = LevelCreator.me.createNewLayer (layerToPaint.layerSortingOrder); //do this to create a new layer and get it

				int widthInChunks = layerToPaint.rowsOfTiles [0].GetComponent<Row> ().getTilesAsChunks ().Count;
				int heightOfLayerInChunks = layerToPaint.rowsOfTiles.Count;



				//int sizeOfArea = LevelCreator.me.brushSize;
				int xChunkLowBound = xIndex - (Mathf.RoundToInt(widthInChunks/2));
				int yChunkLowBound = yIndex - (Mathf.RoundToInt(heightOfLayerInChunks/2));

				int xChunkHighBound = xIndex + (Mathf.RoundToInt(widthInChunks/2));
				int yChunHighBound = yIndex + (Mathf.RoundToInt(heightOfLayerInChunks/2));
				Vector3 topRightCorner = mouseInScene + new Vector3 (widthInChunks / 2, heightOfLayerInChunks / 2, 0.0f);
				Vector3 bottomLeftCorner = mouseInScene - new Vector3 (widthInChunks / 2, heightOfLayerInChunks / 2, 0.0f);

				
				for (int x = xChunkLowBound; x < xChunkHighBound; x++) {
					for (int y = yChunkLowBound; y < yChunHighBound; y++) {
						if (x >= 0 && y >= 0 && x < rowsOfTiles [0].GetComponent<Row> ().getTilesAsChunks ().Count && y < rowsOfTiles.Count) { //can't do if were out of range
							Row rowOnTilemap = newLayer.GetComponent<Layer>().rowsOfTiles [y].GetComponent<Row> ();
							TileChunk tileChunkOnTilemap = rowOnTilemap.getTilesAsChunks () [x];

							Row rowOnPrefab = layerToPaint.rowsOfTiles [y-yChunkLowBound].GetComponent<Row> ();
							TileChunk tileChunkOnPrefab = rowOnPrefab.getTilesAsChunks()[x-xChunkLowBound];
							tileChunkOnTilemap.mergeTileChunks (tileChunkOnPrefab);
						} 
						

					}
				}

			}

		}


		//get the two tilemaps
		//go through each of the layers on the tilemap you're wanting to add
		//for each layer
		//	if a layer with that sorting order exists in the map being painted to then overwrite else create a new layer
		//	go through each row and chunk in the brush.
		//		if the tile in the to paint is not blank

		//have a seperate method for copying over a chunk to another for simplicity???


		/*Working out position in the to paint grid:
		/* Work out whether the row is above or below the center point, can be calculated by comparing it to half the number of rows (y < half = above the center, y ==half is on same index else its below)
		* Work out whether the chunk is left or right of the center using the same maths but the number of tile chunks instead of the number of rows (x<half  = to left of center, x==half = on same x, else its to the right)
		* Work out total size of the area to be painted (should just be size of layers)
		* 
		* Use the code for painting a square area to get the tiles that will be painted based on where the mouse is
		* Use similar code to cycle through the chunks on the grid to be painted on and keep seperate x and y indexes to get the chunks & rows out of the tilemap to be painted on
		* copy the chunks over onto each other, will have to limit the painting so the chunks align
		* 
		* Use asset preview for drawing a preview texture (dimensions should be 30 * width/height)
		* */



		//Stuff to mention in tutorial:
	//new set dirty bits to fix bugs
	//gui stuff for selecting prefabs to paint + new variables they are stored in
	//this method
	//new method for createing and returning a new layer
	//method for merging two tile chunks
	}


	#endif


	public void hideBlankTiles()
	{
		foreach (GameObject g in rowsOfTiles) {
			Row r = g.GetComponent<Row> ();
			r.hideBlankTiles ();
		}
	}

	public void showBlankTiles()
	{
		foreach (GameObject g in rowsOfTiles) {
			Row r = g.GetComponent<Row> ();
			r.showBlankTiles ();
		}
	}
}
