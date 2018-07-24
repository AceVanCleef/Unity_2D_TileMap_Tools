using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
//importing assets - sun 23 07 17
//marked layers as dirty so they serialize propperly
//added this class to read in sprites from resources folder

//TODO: 
//make interface for chaning the sprites of tiles (Change them to gameobject/just use sprites???)
public class SpriteProcessor  {

	string inputFolderPath = Application.dataPath + "/Resources/RawTiles";
	string outputFolderPath = Application.dataPath + "/Resources/OutputTiles";

	string inputFolderResourceLocal = "RawTiles";
	string outputFolderResourceLocal = "OutputTiles";

	public Sprite[] spritesThatHaveBeenFound;

	public void readSpritesInFromInputFolder()
	{
		if (Directory.Exists (inputFolderPath)) {
			Debug.Log ("Found the input folder "+ inputFolderPath);
		} else {
			Debug.Log ("Cant find the input folder " + inputFolderPath);
		}

		if (Directory.Exists (outputFolderPath)) {
			Debug.Log ("Found the output folder " + outputFolderPath);
		} else {
			Debug.Log ("Cant find the output folder " + outputFolderPath);
		}

	

		spritesThatHaveBeenFound = Resources.LoadAll<Sprite> (inputFolderResourceLocal);//loads all sprites in this directory within the resources folder which acts as the root, dont think you can have nested folders
		Debug.Log (spritesThatHaveBeenFound.Length);
	}

	public int getNumSpritesFound()
	{
		if (spritesThatHaveBeenFound == null) {
			return 0;
		} else {
			return spritesThatHaveBeenFound.Length;
		}
	}

	public void processSprites()
	{
		foreach (Sprite s in spritesThatHaveBeenFound) {
			if (File.Exists (outputFolderPath + "/tile_" + s.name+".prefab")) {
				//prefab of tile exists, no need to create another
				Debug.Log("Tile already exists, skipping");
			} else {
				GameObject tilePrefab = new GameObject ();
				SpriteRenderer sr = tilePrefab.AddComponent<SpriteRenderer> ();
				sr.sprite = s;
				tilePrefab.AddComponent<Tile> ();
				tilePrefab.name = "tile_" + s.name;
				PrefabUtility.CreatePrefab ("Assets/Resources/"+outputFolderResourceLocal +"/"+ tilePrefab.name + ".prefab", tilePrefab);
				//AssetDatabase.CreateAsset(tilePrefab,outputFolderPath + tilePrefab.name);
				AssetDatabase.Refresh ();
				GameObject.DestroyImmediate (tilePrefab);
			}
		}

		GameObject[] tiles = Resources.LoadAll<GameObject> (outputFolderResourceLocal);
		//LevelCreator.me.tiles = tiles;
		LevelEditor.setSprites(tiles);
	}

}

