using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//9 7 17 - editing tiles in the grid & painting them
//Edited condition to check whether the cursor was over the tile chunk
//added method to get info on width, height, the bottom left & top right tiles
//wrote new way to access elements in grid without needing to cycle through them
//added methods to paint tiles as single ones and groups
//added cursor
[Serializable]
public class LevelCreator : MonoBehaviour {
	#if UNITY_EDITOR //mark the class so it only gets compiled if its being run in the editor as you cant build if you have scripts that use editor specidif features

	public bool isInitialised;
	public GameObject tilePrefab;

	//values for display, not actual
	public Vector3 mouseInScene=Vector3.zero;
	public string output = "";

	public Sprite testSprite,cursorSprite,blankSprite;
	public bool paintTiles =false;
	public bool paintSingle = true, paintArea = false,eraseSingle=false,eraseArea=false;

	public bool paintSquare = false,paintCircle=false,spraycan = false;
	public int brushSize = 1,spraycanDensity=1;

	public bool showBlankTiles = true;
	//bools for painting an area of tiles


	public GameObject cursor;
	public bool drawCursor = false;

	//New for layer
	public List<GameObject> layersInMap;

	GameObject activeLayerObject;
	public Layer activeLayer;
	public static LevelCreator me;

	//new for importing resources
	SpriteProcessor sp;

	public GameObject[] tiles;

	public GameObject currentTileToPaint;


	//new for painting prefabs
	public GameObject prefabToPaint;
	public bool paintPrefab = false;
	public int widthOfPrefab = 1;
	public int heightOfPrefab = 1;//these are the dimensions of the texture that will work as the cursor


	public Texture2D circleTexture;
	void initialiseCursor()
	{
			cursor = (GameObject)Instantiate (new GameObject (), Vector3.zero, Quaternion.Euler (0, 0, 0));
			cursor.gameObject.name = "Cursor";
			SpriteRenderer sr = cursor.AddComponent<SpriteRenderer> ();
			sr.sprite = cursorSprite;
			sr.sortingOrder = 9999;
	}

	public void setCursorPosition()
	{
		if (drawCursor == false) {
			if (cursor == null) {

			}
			else{
				if (cursor.activeInHierarchy == true) {
					cursor.SetActive (false);
				}
			}
			return;
		}

		if (cursor == null) {
			initialiseCursor ();
		}

		if (cursor.activeInHierarchy == false) {
			cursor.SetActive (true);
		}

		if (paintPrefab == true) {
			
			calculatePrefabTextureSize ();
			SpriteRenderer sr = cursor.GetComponent<SpriteRenderer> ();
			sr.sprite = cursorSprite;
			cursor.transform.localScale = new Vector3 (widthOfPrefab, heightOfPrefab, 1);

	//Need to work out a way to round to nearest multiple of five?
		} else {
			cursor.transform.localScale = new Vector3 (1, 1, 1);

			SpriteRenderer sr = cursor.GetComponent<SpriteRenderer> ();
			sr.sprite = cursorSprite;
		}

		float x = Mathf.Round (mouseInScene.x);
		float y = Mathf.Round (mouseInScene.y);
		//Debug.Log ("CP Mouse X = " + x + " Mouse Y = " + y);
		if (paintPrefab == false) {
			cursor.transform.position = new Vector3 (x, y, 0);
		} else {
			GameObject tile = activeLayer.rowsOfTiles [activeLayer.yIndex].GetComponent<Row> ().getTilesAsChunks () [activeLayer.xIndex].getTile (2, 2).gameObject;
			
			cursor.transform.position = tile.transform.position;//just sets the position to be the center of the chunk the mouse is over
		}
	}




	public void getInfoOnGridDimensions()
	{
		activeLayer.mouseInScene = mouseInScene;
		activeLayer.getInfoOnGridDimensions ();		
		calculateChunkMouseIsOver ();
	}


	void calculateChunkMouseIsOver()//should give values that
	{
		activeLayer.calculateChunkMouseIsOver ();
		
	}

	void initialiseValues()
	{
		if (layersInMap == null) {
			layersInMap = new List<GameObject> ();
		}
		setSingleton ();
	}


	public void createInitialLayer()
	{
		initialiseValues ();
		GameObject g = (GameObject)Instantiate (new GameObject (), Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.name = "Layer " + 0;
		g.transform.parent = this.transform;

		Layer l = g.AddComponent<Layer> ();
		layersInMap.Add (g);
		activeLayerObject = g;
		activeLayer = l;
		
		l.createInitialChunk ();
		EditorUtility.SetDirty (this.gameObject);
		EditorUtility.SetDirty (g);

		isInitialised = true;

		
	}

	public void setActiveLayerSortingOrder(int order)
	{
		activeLayer.setSortingOrder (order);
	}


	public void createNewLayer()
	{
		GameObject g = (GameObject)Instantiate (layersInMap [layersInMap.Count-1], Vector3.zero, Quaternion.Euler (0, 0, 0));
		Layer l = g.GetComponent<Layer> ();
		l.resetLayer (l.layerSortingOrder + 1);
		g.name = "Layer " + l.layerSortingOrder;
		g.transform.parent = this.transform;
		layersInMap.Add (g);
		EditorUtility.SetDirty (this);

		EditorUtility.SetDirty (g);

	}

	public GameObject createNewLayer(int sortingOrder)
	{
		GameObject g = (GameObject)Instantiate (layersInMap [layersInMap.Count-1], Vector3.zero, Quaternion.Euler (0, 0, 0));
		Layer l = g.GetComponent<Layer> ();
		l.resetLayer (sortingOrder);
		g.name = "Layer " + l.layerSortingOrder;
		g.transform.parent = this.transform;
		layersInMap.Add (g);
		EditorUtility.SetDirty (this);

		EditorUtility.SetDirty (g);
		return g;
	}

	public void setActiveLayer(GameObject layer)
	{
		activeLayerObject = layer;
		activeLayer = activeLayerObject.GetComponent<Layer> ();
	}

	public void setSingleton()
	{
		me = this;
	}

	void initilisationCheck() 
	{
		//if (rowsOfTiles == null) {
		//	rowsOfTiles = new List<GameObject> ();
		//}
	}

	public void shouldWePaintTile()
	{
		if (currentTileToPaint == null) {

		} else {
			activeLayer.shouldWePaintTile ();
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
		foreach (GameObject g in layersInMap) {
			g.GetComponent<Layer> ().reset ();
			DestroyImmediate (g);
		}
		layersInMap = new List<GameObject> ();
		isInitialised = false;
		
	}




	public void ExpandGridLeft()//goes through all existing rows and adds a chunk to the left of each
	{
		foreach (GameObject g in layersInMap) {
			g.GetComponent<Layer> ().ExpandGridLeft ();
		}
		
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void ExpandGridRight() //same as left but adds it on the right
	{
		foreach (GameObject g in layersInMap) {
			g.GetComponent<Layer> ().ExpandGridRight ();
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void ExpandGridTop() //methods for calling top/bottom
	{
		foreach (GameObject g in layersInMap) {
			g.GetComponent<Layer> ().ExpandGridTop ();
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void ExpandGridBottom()
	{
		foreach (GameObject g in layersInMap) {
			g.GetComponent<Layer> ().ExpandGridBottom ();
		}
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());

	}

	public void refreshSprites()
	{
		if (sp == null) {
			sp = new SpriteProcessor ();
		}

		sp.readSpritesInFromInputFolder ();
	}

	public int numSpritesFound()
	{
		if (sp == null) {
			return 0;
		}
		else{
			return sp.getNumSpritesFound ();
		}
	}

	public void processSprites()
	{
		if (sp == null) {
			sp = new SpriteProcessor ();
		}

		sp.processSprites ();
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		if (paintSquare == true || spraycan == true) {
			Gizmos.DrawWireCube (new Vector3 (mouseInScene.x, mouseInScene.y, -1), new Vector3 (brushSize, brushSize, 1));
		} else if (paintCircle == true) {
			Gizmos.DrawWireSphere (new Vector3 (mouseInScene.x, mouseInScene.y, -1), (brushSize / 2));
		} 
	}


	public void calculatePrefabTextureSize()
	{
		if (prefabToPaint == null) {
			return;
		}
		//Debug.LogError ("Calculating texture size");
		LevelCreator lc = prefabToPaint.GetComponent<LevelCreator> ();
		Layer l1 = lc.layersInMap [0].GetComponent<Layer>();
		heightOfPrefab = l1.rowsOfTiles.Count * 5; //number of rows * height of each row * pixels per unit
		Row r = l1.rowsOfTiles [0].GetComponent<Row> ();
		widthOfPrefab = r.getTilesAsChunks ().Count * 5;
	}



	#endif

	void Start()
	{
		cursor.SetActive (false);
		hideUnusedTiles ();
	}

	public void hideUnusedTiles()
	{
		foreach (GameObject g in layersInMap) {
			Layer l = g.GetComponent<Layer> ();
			l.hideBlankTiles ();
			showBlankTiles = false;
		}
	}

	public void showUnusedTiles()
	{
		foreach (GameObject g in layersInMap) {
			Layer l = g.GetComponent<Layer> ();
			l.showBlankTiles ();
			showBlankTiles = true;
		}
	}
}
