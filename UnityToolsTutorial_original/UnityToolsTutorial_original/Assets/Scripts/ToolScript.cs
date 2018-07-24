using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour {
	public List<IngameObject> createdObjects;
	public List<GameObject> createdObjectsAsGameObjects;
	public GameObject prefabExample;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void createObject(Vector3 pos)
	{
		if (createdObjects == null) {
			createdObjects = new List<IngameObject> ();
		}

		if (createdObjectsAsGameObjects == null) {
			createdObjectsAsGameObjects = new List<GameObject> ();
		}

		GameObject g = (GameObject)Instantiate (prefabExample, pos, Quaternion.Euler (0, 0, 0));

		createdObjects.Add (new IngameObject (g));

		createdObjectsAsGameObjects.Add (g);
	}


	public void destroyObject(IngameObject toDestroy)
	{
		createdObjectsAsGameObjects.Remove (toDestroy.get ());
		createdObjects.Remove (toDestroy);
		DestroyImmediate (toDestroy.get ());
	}

}


[System.Serializable] //have the objects for two reasons, 1 is due to potential issues with values being saved after running the game, sometimes happens sometimes doesnt. 2 is to make it easier to manipulate, can just write methods here to do stuff
public class IngameObject
{
	public GameObject myObject;
	public IngameObject(GameObject myObj)
	{
		myObject = myObj;
	}

	public GameObject get()
	{
		return myObject;
	}
}

