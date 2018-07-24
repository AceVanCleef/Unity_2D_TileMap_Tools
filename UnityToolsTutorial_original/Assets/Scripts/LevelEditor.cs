using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Added makeDirty to tile placing, layer adding & extensions to make sure its propperly saved in the scene

[ExecuteInEditMode]
public class LevelEditor : EditorWindow{
	static LevelCreator currentTileMap; //variables have to be static cause its not part of an object like the inspector is
	static int selectedToolbarOption = 0;
	static string[] toolbarOptions = {"Grid Options","Grid Expand","Layer Options","Painting Mode","Sprite Selection"};


	static GameObject[] tiles;
	static GameObject[] tilemaps;
	string tilemapName = "";
	static EditorWindow myWindow; //window that we draw in
	SaveLoadControl slc;

	//new for rules
	static string[] tileProcessOptions = { "View Tiles", "Add Rules" };
	static int selectedProcessOption = 0;


	static Vector2 processScrollPosition,spriteListScrollPosition;
	static GameObject prefabToProcess;
	static bool animate, adjacent, random;
	static Sprite adjacentSprite,changeToSprite;
	static List<Sprite> animationSprites,randomSprites;
	static ProgrammableTiles[] tilesOnCurrentPrefab;
	static myDir dir;
	static Sprite displaySprite;
	static float animationTimer = 0.1f;



	[MenuItem("Window/Unit02Games/2D Level Editor")]
	public static void OnEnable() //called when the tool is opened
	{
		
		if (currentTileMap == null && FindObjectOfType<LevelCreator> () == false) {
			createNewTilemap ();
		} else if (FindObjectOfType<LevelCreator> () == true) {
			currentTileMap = FindObjectOfType<LevelCreator> ();
		}
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		myWindow = EditorWindow.GetWindow(typeof(LevelEditor)); //draws the actual window instead of just using the inspector
		myWindow.minSize = new Vector2(300,600);
		//EditorApplication.update += Update;
	}

	static void createNewTilemap()
	{
		GameObject g = new GameObject ();
		g.transform.position = new Vector3 (0, 0, 0);
		g.name = "Tilemap";
		LevelCreator lc = g.AddComponent<LevelCreator> ();
		currentTileMap = lc;
		lc.createInitialLayer ();
	}

	void OnGUI()
	{
		if (LevelCreator.me == null) {
			GUILayout.Label ("Level Creator is Null");
			if (FindObjectOfType<LevelCreator> () != null) {
				FindObjectOfType<LevelCreator> ().setSingleton ();
			} else {

			}
		} else {
			GUILayout.Label ("Level Creator is not Null");
		}


		if (currentTileMap == null) { //if the tilemap is null for whatever reason then it gives the option to create a new one if it can't find an existing one
			if (FindObjectOfType<LevelCreator> () == false) {
				if (GUILayout.Button ("Initialise Tilemap")) {
					createNewTilemap ();
				}
				return;
			} else {
				currentTileMap = FindObjectOfType<LevelCreator> ();
				return;
			}
		} else if (currentTileMap.isInitialised == false) {
			if (GUILayout.Button ("Initialise Tilemap")) {
				currentTileMap.createInitialLayer ();
			}
			return;
		}
		selectedToolbarOption = GUILayout.Toolbar (selectedToolbarOption, toolbarOptions);
		GUILayout.Label (toolbarOptions [selectedToolbarOption]);

		if (selectedToolbarOption == 0) { //generic options for reset & setting cursor
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Reset Tilemap")) {
				currentTileMap.reset ();
			}
			GUILayout.Label ("Resets the tilemap to a blank grid, all progress will be lost");
			EditorGUILayout.EndHorizontal ();

			if (currentTileMap.drawCursor == true) {
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Disable Cursor")) {
					currentTileMap.drawCursor = false;
				}
				GUILayout.Label ("Disables the in scene cursor.");
				EditorGUILayout.EndHorizontal ();

			} else {
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Enable Cursor")) {
					currentTileMap.drawCursor = true;
				}

				GUILayout.Label ("Enables the in scene cursor.");
				EditorGUILayout.EndHorizontal ();

			}

			if (currentTileMap.showBlankTiles == true) {
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Hide Blank Tiles")) {
					currentTileMap.hideUnusedTiles ();
				}

				GUILayout.Label ("Hides unused tiles.");
				EditorGUILayout.EndHorizontal ();
			} else {
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Show Blank Tiles")) {
					currentTileMap.showUnusedTiles ();
				}

				GUILayout.Label ("Shows unused tiles.");
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.BeginHorizontal ();

			if (slc == null) {
				slc = new SaveLoadControl ();
			}

			if (GUILayout.Button ("Save Tilemap")) {
				
				slc.savePrefabToFolder (currentTileMap.gameObject, tilemapName);
				tilemaps = slc.getAllPrefabs ();
			}

			GUILayout.Label ("Saves the current tilemap to the resources folder");

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();

			tilemapName = GUILayout.TextArea (tilemapName);

			GUILayout.Label ("The name to save the tilemap as.");

			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("Click on a tilemap to load it.");

			if (GUILayout.Button ("Load Existing Tilemaps")) {
				tilemaps = slc.getAllPrefabs ();
			}

			//GameObject[] prefabsToLoad = slc.getAllPrefabs ();

			if (tilemaps == null || tilemaps.Length == 0) {
				GUILayout.Label ("Could not find any tiles, try loading them again.");
			} else {
				processScrollPosition = GUILayout.BeginScrollView (processScrollPosition, GUILayout.Height(500));

				int x = 0;
				int y = 0;


				foreach (GameObject g in tilemaps) {
					if (y == 0) {
						GUILayout.BeginVertical ();
						y++;

					}

					if (x == 0) {
						GUILayout.BeginHorizontal ();
					}

				
					if (g == null) {//g becomes null in the gap between overwriting an existing tilemap & the asset refresh, should stop editor crashing

					} else {
						GUILayout.BeginVertical ();
						Texture2D tex = AssetPreview.GetAssetPreview (g);
						GUILayout.Label (g.name);
						if (GUILayout.Button (tex)) { //load prefab into world
							if (slc.prefabSaveCheck (currentTileMap.gameObject) == false || slc.prefabSaveCheckName (tilemapName) == false) {
								if (EditorUtility.DisplayDialog ("Save", "Do you want to save the current tilemap?", "Save", "Don't Save")) {
									slc.savePrefabToFolder (currentTileMap.gameObject, tilemapName);
								} 
							}
							GameObject loadedMap = (GameObject)Instantiate (g, new Vector3 (0, 0, 0), Quaternion.Euler (0, 0, 0));
							GameObject old = LevelCreator.me.gameObject;
							loadedMap.GetComponent<LevelCreator> ().setSingleton ();
							DestroyImmediate (old.gameObject);
							tilemaps = slc.getAllPrefabs ();
						}

						if (GUILayout.Button ("Delete")) {
							if (EditorUtility.DisplayDialog ("Delete", "Do you want to destroy the tilemap " + g.gameObject.name + "?(cannot be undone)", "Delete", "Cancel")) {
								DestroyImmediate (g, true);
								AssetDatabase.Refresh ();
								tilemaps = slc.getAllPrefabs ();
							}
						}
						GUILayout.EndVertical ();
					}
				
					x++;

					if (x == 3) {
						GUILayout.EndHorizontal ();
						x = 0;
						y++;
					}

					if (g==tilemaps[tilemaps.Length-1]) {
						GUILayout.EndVertical ();
						//return;
					}
				}
				GUILayout.EndScrollView ();

			}

		} else if (selectedToolbarOption == 1) { //expanding the grid
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand Grid on Top Side")) {
				currentTileMap.ExpandGridTop ();
			}
			GUILayout.Label ("Creates a new row of 5*5 tiles on top of the grid.");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand Grid on Bottom Side")) {
				currentTileMap.ExpandGridBottom ();
			}
			GUILayout.Label ("Creates a new row of 5*5 tiles on bottom of the grid.");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand Grid on Left Side")) {
				currentTileMap.ExpandGridLeft ();
			}
			GUILayout.Label ("Adds a 5*5 chunk to the left side of each row.");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand Grid on Right Side")) {
				currentTileMap.ExpandGridRight ();
			}
			GUILayout.Label ("Adds a 5*5 chunk to the right side of each row.");
			EditorGUILayout.EndHorizontal ();
		} else if (selectedToolbarOption == 2) { //Layer selection/manipulation and creating new ones
			GUILayout.Label ("Layer Selection:");
			foreach (GameObject layerObj in currentTileMap.layersInMap) {
				Layer l = layerObj.GetComponent<Layer> ();
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Set active layer " + l.layerSortingOrder)) {
					currentTileMap.setActiveLayer (layerObj);
				}
				GUILayout.Label (AssetPreview.GetAssetPreview (layerObj));
				EditorGUILayout.EndHorizontal ();

			}

			if (GUILayout.Button ("Create New Layer?")) {
				currentTileMap.createNewLayer ();
			}

			GUILayout.Label ("Current Layer Manipulation:");

			GUILayout.Label ("Current Layers Sorting Order: " + currentTileMap.activeLayer.layerSortingOrder);
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Increase Active Sorting Order")) {
				currentTileMap.setActiveLayerSortingOrder (currentTileMap.activeLayer.layerSortingOrder + 1);
			}

			if (GUILayout.Button ("Decrease Active Sorting Order")) {
				currentTileMap.setActiveLayerSortingOrder (currentTileMap.activeLayer.layerSortingOrder - 1);
			}
			EditorGUILayout.EndHorizontal ();

		} else if (selectedToolbarOption == 3) { //painting mode
			if (GUILayout.Button ("Paint Sprites = " + currentTileMap.paintTiles)) {
				currentTileMap.paintTiles = !currentTileMap.paintTiles;
			}


			if (currentTileMap.paintTiles == true) {
				//do bla bla bla

				if (currentTileMap.currentTileToPaint == null) {
					GUILayout.Label ("Please slelect a tile from the next tab to paint.");
					if (GUILayout.Button ("Erase Single = " + currentTileMap.eraseSingle)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = true;
						currentTileMap.paintPrefab = false;
					}

					if (GUILayout.Button ("Erase Area = " + currentTileMap.eraseArea)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = true;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintPrefab = false;
					}

					if (GUILayout.Button ("Paint existing tilemap = " + currentTileMap.paintPrefab)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintPrefab = true;

						LevelCreator.me.calculatePrefabTextureSize ();

					}
				} else {
					GUILayout.Label ("Mouse in world: " + mousePositionInScene.ToString ());

					if (LevelCreator.me.currentTileToPaint == null) {

					} else {
						GUILayout.BeginHorizontal ();

						GUILayout.Label ("Currently Selected Tile:");
						GUILayout.Label (AssetPreview.GetAssetPreview (LevelCreator.me.currentTileToPaint));


						GUILayout.EndHorizontal ();
					}


					if (GUILayout.Button ("Paint Single = " + currentTileMap.paintSingle)) {
						currentTileMap.paintSingle = !currentTileMap.paintSingle;
						if (currentTileMap.paintArea == true) {
							currentTileMap.paintArea = false;
						}

						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintCircle = false;
						currentTileMap.paintSquare = false;
						currentTileMap.paintPrefab = false;
					}

					if (currentTileMap.paintSingle == true) {
						if (GUILayout.Button ("Paint Square = " + currentTileMap.paintSquare)) {
							currentTileMap.paintSquare = !currentTileMap.paintSquare;
							currentTileMap.paintCircle = false;
							currentTileMap.spraycan = false;
						}

						if (GUILayout.Button ("Paint Circle = " + currentTileMap.paintCircle)) {
							currentTileMap.paintSquare = false;
							currentTileMap.paintCircle = !currentTileMap.paintCircle; 
							currentTileMap.spraycan = false;
						}

						if (GUILayout.Button ("Spraycan = " + currentTileMap.spraycan)) {
							currentTileMap.spraycan = !currentTileMap.spraycan;
							currentTileMap.paintSquare = false;
							currentTileMap.paintCircle = false;
						}

						if (currentTileMap.spraycan == true) {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Spraycan Density = " + currentTileMap.spraycanDensity.ToString ());
							currentTileMap.spraycanDensity = (int)GUILayout.HorizontalSlider (currentTileMap.spraycanDensity, 1, 100);
							GUILayout.EndHorizontal ();
						}




						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Brush Size = " + currentTileMap.brushSize.ToString ());
						currentTileMap.brushSize = (int)GUILayout.HorizontalSlider (currentTileMap.brushSize, 3, 100);
						GUILayout.EndHorizontal ();
					}

					if (GUILayout.Button ("Paint Area = " + currentTileMap.paintArea)) {
						currentTileMap.paintArea = !currentTileMap.paintArea;

						if (currentTileMap.paintArea == true) {
							currentTileMap.paintSingle = false;
						}

						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintCircle = false;
						currentTileMap.paintSquare = false;
						currentTileMap.paintPrefab = false;
					}

					if (GUILayout.Button ("Paint existing tilemap = " + currentTileMap.paintPrefab)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintPrefab = true;

						LevelCreator.me.calculatePrefabTextureSize ();
					}

					if (GUILayout.Button ("Erase Single = " + currentTileMap.eraseSingle)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = false;
						currentTileMap.eraseSingle = true;
						currentTileMap.paintCircle = false;
						currentTileMap.paintSquare = false;
						currentTileMap.paintPrefab = false;
					}

					if (GUILayout.Button ("Erase Area = " + currentTileMap.eraseArea)) {
						currentTileMap.paintArea = false;
						currentTileMap.paintSingle = false;
						currentTileMap.eraseArea = true;
						currentTileMap.eraseSingle = false;
						currentTileMap.paintCircle = false;
						currentTileMap.paintSquare = false;
						currentTileMap.paintPrefab = false;
					}

					if (currentTileMap.paintArea == true) {
						//changed layer multi paint bools to public
						GUILayout.Label ("Paint Area Info:");
						if (currentTileMap.activeLayer.assignedStart == true) {
							GUILayout.Label ("Starting Position is: " + currentTileMap.activeLayer.multiPaintMouseStart); 
						} else {
							GUILayout.Label ("Starting position is unassigned, use alt+left mouse to assign");
						}

						if (currentTileMap.activeLayer.assignedEnd == true) {
							GUILayout.Label ("Ending Position is: " + currentTileMap.activeLayer.multiPaintMouseEnd); 
						} else {
							GUILayout.Label ("Ending position is unassigned, use alt+right mouse to assign");
						}


						if (currentTileMap.activeLayer.assignedEnd == true && currentTileMap.activeLayer.assignedStart == true) {
							GUILayout.Label ("Both positions assigned, use alt+middle mouse to paint the area.");
						}
					} else if (currentTileMap.paintSingle == true) {
						GUILayout.Label ("Use alt+left mouse to paint tiles");
					} else if (currentTileMap.eraseArea == true) {
						//changed layer multi paint bools to public
						GUILayout.Label ("Erase Area Info:");
						if (currentTileMap.activeLayer.assignedStart == true) {
							GUILayout.Label ("Starting Position is: " + currentTileMap.activeLayer.multiPaintMouseStart); 
						} else {
							GUILayout.Label ("Starting position is unassigned, use alt+left mouse to assign");
						}

						if (currentTileMap.activeLayer.assignedEnd == true) {
							GUILayout.Label ("Ending Position is: " + currentTileMap.activeLayer.multiPaintMouseEnd); 
						} else {
							GUILayout.Label ("Ending position is unassigned, use alt+right mouse to assign");
						}


						if (currentTileMap.activeLayer.assignedEnd == true && currentTileMap.activeLayer.assignedStart == true) {
							GUILayout.Label ("Both positions assigned, use alt+middle mouse to erase the area.");
						}
					} else if (currentTileMap.eraseSingle == true) {
						GUILayout.Label ("Use alt+left mouse to erase tiles");
					} else if (currentTileMap.paintPrefab == true) {

						if (currentTileMap.prefabToPaint == null) {

						} else {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Tilemap to merge with current : " + currentTileMap.prefabToPaint.name);
							Texture2D tex = AssetPreview.GetAssetPreview (currentTileMap.prefabToPaint);
							GUILayout.Box (tex);
							GUILayout.EndHorizontal ();
						}


						int x = 0;
						int y = 0;
						processScrollPosition = GUILayout.BeginScrollView (processScrollPosition, GUILayout.Height(500));

						if (tilemaps == null) {
							tilemaps = slc.getAllPrefabs ();
						}

						foreach (GameObject g in tilemaps) {
							if (y == 0) {
								GUILayout.BeginVertical ();
								y++;

							}

							if (x == 0) {
								GUILayout.BeginHorizontal ();
							}


							if (g == null) {//g becomes null in the gap between overwriting an existing tilemap & the asset refresh, should stop editor crashing

							} else {
								GUILayout.BeginVertical ();
								Texture2D tex = AssetPreview.GetAssetPreview (g);
								GUILayout.Label (g.name);
								if (GUILayout.Button (tex)) { //load prefab into world
									currentTileMap.prefabToPaint = g;
								}
									
								GUILayout.EndVertical ();
							}

							x++;

							if (x == 3) {
								GUILayout.EndHorizontal ();
								x = 0;
								y++;
							}

							if (g == tilemaps[tilemaps.Length-1]) {
								GUILayout.EndVertical ();
								//return;
							}
						}
						GUILayout.EndScrollView ();

					}
				}

			}
		} else if (selectedToolbarOption == 4) { //selecting sprite

			selectedProcessOption = GUILayout.Toolbar (selectedProcessOption, tileProcessOptions);
			GUILayout.Label (tileProcessOptions [selectedProcessOption]);

			if (selectedProcessOption == 0) {
				GUILayout.Label ("Sprite Selection");
				GUILayout.Label ("Place spritesheets in the resources/RawTiles folder & press the Load Sprites button to turn them into tiles");
				if (GUILayout.Button ("Load Sprites")) {
					currentTileMap.refreshSprites ();
					currentTileMap.processSprites ();
				}

				if (tiles == null || tiles.Length == 0) {
					GUILayout.Label ("Could not find any tiles, try loading them again.");
				} else {
					processScrollPosition = GUILayout.BeginScrollView (processScrollPosition, GUILayout.Height(500));

					int x = 0;
					int y = 0;


					foreach (GameObject g in tiles) {

						if (y == 0) {
							GUILayout.BeginVertical ();
							y++;

						}

						if (x == 0) {
							GUILayout.BeginHorizontal ();
						}

						Texture2D tex = AssetPreview.GetAssetPreview (g);

						if (GUILayout.Button (tex)) {
							currentTileMap.currentTileToPaint = g;
						}

						x++;

						if (x == 4) {
							GUILayout.EndHorizontal ();
							x = 0;
							y++;
						}

						if (g == tiles[tiles.Length-1]) {
							GUILayout.EndVertical ();
							//return;
						}
					}

					GUILayout.EndScrollView ();


				}
			}else if(selectedProcessOption==1){
				GUILayout.Label ("Rule Adder");
				GUILayout.Label ("Select a tile to add rules to.");

				processScrollPosition = GUILayout.BeginScrollView (processScrollPosition, GUILayout.Height(170));
				try{
					if (tiles.Length == 0 || tiles == null) {
						if (GUILayout.Button ("Load Sprites")) {
							currentTileMap.refreshSprites ();
							currentTileMap.processSprites ();
						}
					} else {
						GUILayout.BeginHorizontal ();
						foreach (GameObject g in tiles) {

							Texture2D tex = AssetPreview.GetAssetPreview (g);

							if (GUILayout.Button (tex)) {
								//currentTileMap.currentTileToPaint = g;

								prefabToProcess = g;
								tilesOnCurrentPrefab = prefabToProcess.GetComponents<ProgrammableTiles>();
							}

						}
						GUILayout.EndHorizontal ();
						GUILayout.EndScrollView ();

						if (prefabToProcess == null) {
							GUILayout.Label ("No tile selected for processing.");
						} else {
							GUILayout.BeginHorizontal ();
							GUILayout.Box (AssetPreview.GetAssetPreview (prefabToProcess));

							GUILayout.BeginVertical ();
							if (GUILayout.Button ("Add Neighbour Change Rule")) {
								adjacent = true;
								random = false;
								animate = false;
							}

							if (GUILayout.Button ("Add Random Tile Rule")) {
								adjacent = false;
								random = true;
								animate = false;
							}

							if (GUILayout.Button ("Add Tile Animation")) {
								adjacent = false;
								random = false;
								animate = true;
							}



							GUILayout.EndVertical ();
							GUILayout.EndHorizontal ();

							//static Texture2D adjacentSprite,changeToSprite;
							//static List<Sprite> animationSprites,randomSprites;
							if (adjacent == true) {
								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Adjacent Sprite");
								adjacentSprite = (Sprite)EditorGUILayout.ObjectField ("Image", adjacentSprite, typeof(Sprite), false);
								GUILayout.EndHorizontal ();

								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Sprite to change to");
								changeToSprite = (Sprite)EditorGUILayout.ObjectField ("Image", changeToSprite, typeof(Sprite), false);
								GUILayout.EndHorizontal ();

								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Direction From Origin " + dir);

								if(GUILayout.Button("Up"))
								{
									dir = myDir.up;
								}

								if(GUILayout.Button("Down"))
								{
									dir=myDir.down;
								}

								if(GUILayout.Button("Left"))
								{
									dir=myDir.left;
								}

								if(GUILayout.Button("Right"))
								{
									dir=myDir.right;
								}

								//dir = (myDir)EditorGUILayout.ObjectField ("Direction", dir, typeof(myDir), false);
								GUILayout.EndHorizontal ();

								if(GUILayout.Button("Add adjacency rule to tile.")) //TODO add a display for all existing rules for a tile + a way to clear them
								{
									if(changeToSprite==null || adjacentSprite==null || dir==null)
									{
										EditorUtility.DisplayDialog ("Error", "You need to select a neighbouring sprite, a sprite to change to and a direction to use the neighbour change rules.", "Ok");
									}
									else{
										NeighbourChange nc;

										if(prefabToProcess.GetComponent<NeighbourChange>()!=null)
										{
											nc=prefabToProcess.GetComponent<NeighbourChange>();
										}
										else{
											nc= prefabToProcess.AddComponent<NeighbourChange>();
										}

										nc.addRule(prefabToProcess.GetComponent<SpriteRenderer>().sprite,adjacentSprite,changeToSprite,dir);
										tilesOnCurrentPrefab = prefabToProcess.GetComponents<ProgrammableTiles>();

										AssetDatabase.Refresh();
									}
								}
							}
							else if(animate==true)
							{
								
								GUILayout.Label ("Sprites to form animation.");

								if(animationSprites==null || animationSprites.Count==0)
								{

								}
								else{
									int x = 0;
									spriteListScrollPosition = GUILayout.BeginScrollView (spriteListScrollPosition, GUILayout.Height(170));
									foreach(Sprite s in animationSprites)
									{
										GUILayout.BeginHorizontal();
										GUILayout.Label("Sprite " + x);
										GUILayout.Box(AssetPreview.GetAssetPreview(s));
										x++;

										if(GUILayout.Button("Remove sprite from animation")){
											animationSprites.Remove(s);
											return;//have to return so it doesnt continue the run through the rest of the list after editing
										}
										GUILayout.EndHorizontal();
									}
									GUILayout.EndScrollView();
								}

								GUILayout.BeginHorizontal();
								GUILayout.Label("Animation time per frame");
								animationTimer = float.Parse(GUILayout.TextField(animationTimer.ToString()));
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Sprite to add:");
								displaySprite = (Sprite)EditorGUILayout.ObjectField ("", displaySprite, typeof(Sprite), false);
								GUILayout.EndHorizontal ();

								if(GUILayout.Button("Add sprite to animation"))
								{
									if(displaySprite==null)
									{
										EditorUtility.DisplayDialog ("Error", "Please select a sprite to add to the animation.", "Ok");

									}
									else{
										if(animationSprites==null)
										{
											animationSprites = new List<Sprite>();
										}
										animationSprites.Add(displaySprite);
										displaySprite=null;
									}
								}

								if(GUILayout.Button("Add animation to tile"))
								{
									if(animationSprites==null || animationSprites.Count==0)
									{
										EditorUtility.DisplayDialog ("Error", "You need to least 1 sprite in the animation.", "Ok");

									}
									else if(animationTimer <=0.0f)
									{
										EditorUtility.DisplayDialog ("Error", "Timer reset animation must be above 0.0f.", "Ok");
									}
									else{
										//do all animation stuff
										TileAnimation ta = prefabToProcess.AddComponent<TileAnimation>();
										ta.initialise(animationSprites,animationTimer);
										animationSprites = new List<Sprite>();
										tilesOnCurrentPrefab = prefabToProcess.GetComponents<ProgrammableTiles>();

									}
								}
							}
							else if(random==true)
							{
								GUILayout.Label ("Sprites that tile could be");

								if(randomSprites==null || randomSprites.Count==0)
								{

								}
								else{
									int x = 0;
									spriteListScrollPosition = GUILayout.BeginScrollView (spriteListScrollPosition, GUILayout.Height(170));
									foreach(Sprite s in randomSprites)
									{
										GUILayout.BeginHorizontal();
										GUILayout.Label("Sprite " + x);
										GUILayout.Box(AssetPreview.GetAssetPreview(s));
										x++;

										if(GUILayout.Button("Remove sprite from list")){
											randomSprites.Remove(s);
											return;//have to return so it doesnt continue the run through the rest of the list after editing
										}
										GUILayout.EndHorizontal();
									}
									GUILayout.EndScrollView();
								}

								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Sprite to add:");
								displaySprite = (Sprite)EditorGUILayout.ObjectField ("", displaySprite, typeof(Sprite), false);
								GUILayout.EndHorizontal ();

								if(GUILayout.Button("Add sprite to List"))
								{
									if(displaySprite==null)
									{
										EditorUtility.DisplayDialog ("Error", "Please select a sprite to add to the list.", "Ok");

									}
									else{
										if(randomSprites==null)
										{
											randomSprites = new List<Sprite>();
										}
										randomSprites.Add(displaySprite);
										displaySprite=null;
									}
								}

								if(GUILayout.Button("Add random rule to tile"))
								{
									if(randomSprites==null || randomSprites.Count==0)
									{
										EditorUtility.DisplayDialog ("Error", "You need to least 1 sprite in the list.", "Ok");

									}
									else{
										//do all animation stuff
										RandomTile rt = prefabToProcess.AddComponent<RandomTile>();
										rt.initialise(randomSprites);
										randomSprites = new List<Sprite>();
										tilesOnCurrentPrefab = prefabToProcess.GetComponents<ProgrammableTiles>();

									}
								}
							}

							if(tilesOnCurrentPrefab==null || tilesOnCurrentPrefab.Length==0)
							{

							}
							else{
								GUILayout.Label("Programmable tiles on selected prefab");
								foreach(ProgrammableTiles pt in tilesOnCurrentPrefab)
								{
									GUILayout.BeginHorizontal();

									GUILayout.Label(pt.getInfoOnTile());
									if(GUILayout.Button("Remove Programmable Tile"))
									{
										DestroyImmediate(pt,true);
										tilesOnCurrentPrefab = prefabToProcess.GetComponents<ProgrammableTiles>();
										return;
									}

									GUILayout.EndHorizontal();
								}
							}

						}
					}
				}
				catch{

				}
			}
		} 

	}


	public static void setSprites(GameObject[] toSet) //changed the sprite processor to set it to the level editor script rather than the LevelCreator script
	{
		tiles = toSet; 
	}

	public static Vector3 mousePositionInScene;


	public static Vector3 getMouseInScene()
	{
		return mousePositionInScene;
	}


	static void OnSceneGUI(SceneView sceneView) //GUI for the scene window
	{

		if (currentTileMap == null || currentTileMap.isInitialised == false) {
			return;
		}




		try {
			mousePositionInScene = new Vector3 (Event.current.mousePosition.x, Event.current.mousePosition.y); //gets the mouse position within the scene window
			mousePositionInScene.y = sceneView.camera.pixelHeight - mousePositionInScene.y; //have to do this otherwise the mouse position is inverted (move the mouse up & the pos in world goes down)
			mousePositionInScene = sceneView.camera.ScreenToWorldPoint (mousePositionInScene); //convert to the position from screen to world coordinates
			mousePositionInScene.z = 0;
			currentTileMap.mouseInScene = mousePositionInScene;
			//Debug.Log (mousePositionInScene.ToString ());
		} catch {
			Debug.LogError ("Something went wrong with getting mouse position, mouse not over scene");
		}

		//if (isMouseInScreen () == true) {
			if (currentTileMap.isInitialised == true) {
				currentTileMap.getInfoOnGridDimensions ();
				currentTileMap.shouldWePaintTile ();
				currentTileMap.setCursorPosition ();
			}
		//}

	}

	void OnFocus() {//when window is clicked on
		
		SceneView.onSceneGUIDelegate -= OnSceneGUI;

		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}

	void OnDestroy() {//window is closed
		
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	public void OnInspectorUpdate()
	{
		Repaint();//redraw editor
	}

	static bool isMouseInScreen()
	{
		Vector2 mouse = new Vector2 (mousePositionInScene.x, mousePositionInScene.y);

		Vector2 bl = currentTileMap.activeLayer.bottomLeftTile;
		Vector2 tr = currentTileMap.activeLayer.topRightTile;

		if (mouse.x >= bl.x && mouse.x <= tr.x && mouse.y >= bl.y && mouse.y <= tr.y) {
			return true;
		} else {
			return false;
		}
	}

}
