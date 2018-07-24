using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolScript))]
public class ToolScriptInspector :  Editor {

	Vector3 mousePosition;
	ToolScript myScript;
	bool createObjects = false;

	public Vector3 getMouseInScene()
	{
		return mousePosition;
	}

	void OnSceneGUI() //GUI for the scene window
	{  
		if (myScript == null) {
			myScript = (ToolScript)target;//gets the instance of the script that it is an inspector for if its null
		}
			
		mousePosition = new Vector3( Event.current.mousePosition.x,Event.current.mousePosition.y); //gets the mouse position within the scene window
		mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y; //have to do this otherwise the mouse position is inverted (move the mouse up & the pos in world goes down)
		mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition); //convert to the position from screen to world coordinates
		mousePosition.z = 0;


		if (createObjects == true) {
			Event e = Event.current; //looks at current event (Keypresses, stuff like that))
			if(e.isMouse) //if its a mouse click & alt is held then code executes
			{
				if (e.type == EventType.MouseUp) {
					if (e.button == 0 && e.alt == true) {

						myScript.createObject (getMouseInScene()); 
						EditorUtility.SetDirty (myScript); //marks the objects as being dirty (I think its like saying that the object has changed since the last time the scene was saved)
						EditorUtility.SetDirty (myScript.gameObject);
					}
				}
			}
		}
	}

	public override void OnInspectorGUI() //GUI for the inspector (bit where scripts info is)
	{
		
		DrawDefaultInspector (); //draws the unity defined stuff for the inspector e.g. variables

		if(GUILayout.Button("Create object on mouse click = " + createObjects))
		{
			createObjects = !createObjects;
		}

		try{ //have a try to stop error when elements are removed from the list
			foreach (IngameObject g in myScript.createdObjects) {
				if (GUILayout.Button ("Destroy object @ " + g.get ().transform.position)) {
					myScript.destroyObject (g);
				}
			}
		}
		catch{

		}

	}

}

