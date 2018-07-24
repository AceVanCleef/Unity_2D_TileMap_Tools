using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorInspector :  Editor{

	//just inspector for the LevelCreator

	LevelCreator myTarget;
	public Vector3 mousePositionInScene;
	void targetCheck()
	{
		if (myTarget == null) {
			myTarget = (LevelCreator)target;
		}
	}

	public Vector3 getMouseInScene()
	{
		return mousePositionInScene;
	}

	void setMousePositionInScene()
	{
		mousePositionInScene = new Vector3( Event.current.mousePosition.x,Event.current.mousePosition.y); //gets the mouse position within the scene window
		mousePositionInScene.y = SceneView.currentDrawingSceneView.camera.pixelHeight -mousePositionInScene.y; //have to do this otherwise the mouse position is inverted (move the mouse up & the pos in world goes down)
		mousePositionInScene = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePositionInScene); //convert to the position from screen to world coordinates
		mousePositionInScene.z = 0;

		//LevelEditor.mousePositionInScene = mousePositionInScene;
		//myTarget.mouseInScene = mousePositionInScene;

	}

	void OnSceneGUI() //GUI for the scene window
	{
		//setMousePositionInScene ();
		//targetCheck ();
		//myTarget.setSingleton ();

		//if (myTarget.isInitialised == true) {
		//	
		//	myTarget.getInfoOnGridDimensions ();
		//	myTarget.shouldWePaintTile ();
		//	myTarget.setCursorPosition ();
		//}
	}

	public override void OnInspectorGUI() //GUI for the inspector (bit where scripts info is)
	{
		DrawDefaultInspector ();

		if (myTarget == null) {

		} else {
			if (myTarget.isInitialised == false) { 
				if (GUILayout.Button ("Create new editor map")) {
					myTarget.createInitialLayer();
				}
			} else {

				GUILayout.Label ("___________________________");
				GUILayout.Label ("Tile Map Options");

				if (GUILayout.Button ("DEBUG_ reset editor map")) {
					myTarget.reset ();
				}

				if (GUILayout.Button ("Draw Cursor " + myTarget.drawCursor)) {
					myTarget.drawCursor = !myTarget.drawCursor;
				}

				if (GUILayout.Button ("Create New Layer?")) {
					myTarget.createNewLayer();
				}
				GUILayout.Label ("___________________________");


				GUILayout.Label ("___________________________");
				GUILayout.Label ("Active Layer is " + myTarget.activeLayer.gameObject.name + " Layers sorting order is " + myTarget.activeLayer.layerSortingOrder);
				foreach (GameObject layerObj in myTarget.layersInMap) {
					Layer l = layerObj.GetComponent<Layer> ();
					if (GUILayout.Button ("Set active layer " + l.layerSortingOrder)) {
						myTarget.setActiveLayer (layerObj);
					}
				}
				GUILayout.Label ("___________________________");
				GUILayout.Label ("Active Layer Options");

				if (GUILayout.Button ("Increase Active Sorting Order")) {
					myTarget.setActiveLayerSortingOrder (myTarget.activeLayer.layerSortingOrder + 1);
				}

				if (GUILayout.Button ("Decrease Active Sorting Order")) {
					myTarget.setActiveLayerSortingOrder (myTarget.activeLayer.layerSortingOrder - 1);
				}

				GUILayout.Label ("___________________________");

				GUILayout.Label ("XIndex "+myTarget.activeLayer.xIndex);
				GUILayout.Label ("YIndex "+myTarget.activeLayer.yIndex);

				GUILayout.Label ("___________________________");
				GUILayout.Label ("Grid Controls");

				if (GUILayout.Button ("Expand grid on left side")) {
					myTarget.ExpandGridLeft ();
				}

				if (GUILayout.Button ("Expand grid on right side")) {
					myTarget.ExpandGridRight ();
				}

				if (GUILayout.Button ("Expand grid on top side")) {
					myTarget.ExpandGridTop ();
				}

				if (GUILayout.Button ("Expand grid on bottom side")) {
					myTarget.ExpandGridBottom ();
				}


				GUILayout.Label ("___________________________");

				GUILayout.Label ("Sprite Painter Options");

				if (GUILayout.Button ("Paint Sprites = " + myTarget.paintTiles)) {
					myTarget.paintTiles = !myTarget.paintTiles;
				}


				if (myTarget.paintTiles == true) {
					//do bla bla bla

					if (GUILayout.Button ("Paint Single = " + myTarget.paintSingle)) {
						myTarget.paintSingle = !myTarget.paintSingle;
						if (myTarget.paintArea == true) {
							myTarget.paintArea = false;
						}
					}

					if (GUILayout.Button ("Paint Area = " + myTarget.paintArea)) {
						myTarget.paintArea = !myTarget.paintArea;

						if (myTarget.paintArea == true) {
							myTarget.paintSingle = false;
						}
					}
				}
				GUILayout.Label ("___________________________");
				GUILayout.Label ("Sprite Control");

				GUILayout.Label ("Sprites Found = " + myTarget.numSpritesFound());
				if (GUILayout.Button ("Load Sprites")) {
					myTarget.refreshSprites ();
					myTarget.processSprites ();
				}

				foreach (GameObject g in myTarget.tiles) {
					Texture2D tex = AssetPreview.GetAssetPreview (g);

					if (GUILayout.Button (tex)) {
						myTarget.currentTileToPaint = g;
					}
				}

			}
		}
	}
}
