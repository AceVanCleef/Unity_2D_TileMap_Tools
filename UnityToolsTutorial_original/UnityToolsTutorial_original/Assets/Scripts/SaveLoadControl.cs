using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


//Added this script & controls in the level editor
//Tilemap array & name string in level editor
//added code to editor to allow a new map to be created if the singleton is null & it can't find a LevelCreator instance in the world

public class SaveLoadControl {

	string directoryForSaving = Application.dataPath + "/Resources/SavedTilemaps";
	string pathEnd = "Assets/Resources/SavedTilemaps";
	public SaveLoadControl()
	{
		initialise ();
	}

	//Normal IO - Full File Path
	//Prefab Utility - Path starting at assets folder
	//Resources - Path starting at resources folder

	void initialise()
	{
		if (Directory.Exists (directoryForSaving) == false) {
			Directory.CreateDirectory (directoryForSaving);
			Debug.Log ("Creating Directory for saving prefabs");
			AssetDatabase.Refresh ();
		}
	}

	public void savePrefabToFolder(GameObject toSave,string name)
	{
		if (File.Exists (directoryForSaving + "/" + name + ".prefab") == false) {
			string path = pathEnd + "/" + name + ".prefab";
			PrefabUtility.CreatePrefab (path, toSave);
			AssetDatabase.Refresh ();
		} else if(File.Exists (directoryForSaving + "/" + name + ".prefab") == true && EditorUtility.DisplayDialog("Overwrite Existing Tilemap","A tilemap with the name " + name + " already exists, do you want to overwrite?","Overwrite","Cancel")){
			//do something to 
			string path = pathEnd + "/" + name + ".prefab";
			PrefabUtility.CreatePrefab (path, toSave);
			AssetDatabase.Refresh ();
		}
	}


	public bool prefabSaveCheck(GameObject obj)
	{
		GameObject[] objects = getAllPrefabs ();
		foreach(GameObject g in objects)
		{
			if (obj == g) {
				return true;
			}
		}
		return false;
	}

	public bool prefabSaveCheckName(string name)
	{
		GameObject[] objects = getAllPrefabs ();
		foreach(GameObject g in objects)
		{
			if (name == g.name) {
				return true;
			}
		}
		return false;
	}

	public GameObject[] getAllPrefabs()
	{
		GameObject[] retVal = Resources.LoadAll<GameObject> ("SavedTilemaps");
		Debug.Log ("Found " + retVal.Length + " tilemaps");
		return retVal;
	}
}
