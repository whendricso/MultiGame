#pragma warning disable 0618
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MultiGame;

namespace MultiGame {

	//functionality to quickly create a prototype game

	public class GameGenerator : MGEditor {

		[Tooltip("The path you would like to save your prefabs to. Must start with Assets/ and needs a trailing forward slash on the end. Spaces are acceptable. Case-sensitive. Recommend saving" +
			" inside a Resources folder, for save/load and online play support.")]
		public string prefabFolderPath = "Assets/Generated/Resources/";
		
		private Terrain terrain;
		private float[,,] splatMap;
		private GameObject water;
		private GameObject floor;//ground plane, if any
		private float[,] newHeights;//flexible height dataset

		public bool terrainFromAlgorithm = true;
		public bool terrainIsStatic = true;
		public bool randomTerrainSeed = true;
		public bool proceduralSplatmap = true;
		public bool proceduralDistribMap = true;
		public Material planeMaterial;

		private Vector2 mainScrollView = new Vector2();
		public bool generateAll = false;//when true, generates a new set of data
		public bool	generateSelected = false;//generates only the enabled sections
		private bool generating = false;//are we currently generating?

		private bool showLevelEditor = true;
		private bool showPlayerEditor = true;
		private Vector2 playerEditorHelpView = new Vector2();
		private bool showNPCEditor = true;
		private Vector2 npcEditorHelpView = new Vector2();
		private bool showObjectEditor = true;
		private Vector2 objectEditorHelpView = new Vector2();
		private bool showItemEditor = true;
		private Vector2 itemEditorHelpView = new Vector2();

		//Lists of generated data
		public MultiGameLevel level = new MultiGameLevel();
		public List<MultiGamePlayer> players = new List<MultiGamePlayer>();
		public List<MultiGameNPC> npcs = new List<MultiGameNPC>();
		public List<MultiGameObject> objects = new List<MultiGameObject>();
		public List<MultiGameItem> items = new List<MultiGameItem>();
		
		//a list to show errors in
		public List<string> errors = new List<string>();
		private Vector2 errorView = new Vector2();
		private bool clearErrors = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This window allows you to quickly generate game content using a handy GUI. By adding more Levels, Players, NPCs, Objects," +
			" and Items, you can leave the details up to MultiGame, which will generate a new set of game assets each time you press 'Generate All' or 'Generate Selected'. You can then adjust these as needed, until" +
			" you have exactly the game you want! See the accompanying documentation file found in:\n\n MultiGame/Scripts/Core/Editor/GameGeneratorDoc");

		//Generate a new game level in the current scene
		[System.Serializable]
		public class MultiGameLevel {

			//general outdoor settings
			public float waterLevel;
			public Material waterMaterial;
			public bool useWaterLevel;
			public float maxTerrainHeight;
			public float textureBorder1;
			public float textureBorder2;
			public float textureBorderBlend;
			public float cliffSteepness;
			public float highlandFactor;
			public float lowlandFactor;
			public Texture2D texture1, texture2, texture3;
			public GameObject detail1, detail2, detail3, flatlandDetail;
			public Texture2D grass1, grass2, grass3, flatlandGrass;
			public float detail1Str, detail2Str, detail3Str, flatlandDetailStr;//number of details placed
			public float grass1Str, grass2Str, grass3Str, flatlandGrassStr;//strength of each grass distribution
			public float importHeightMultiplier;
			public float terrainOffset;//how much to offset the terrain in the Y direction, useful for y = 0 = sea level
			public Vector3 levelSize;
			public Texture2D heightMap;//if this is not null then we will generate a Unity terrain using this height map.
			public Texture2D distributionMap;//if this is not null, we'll use it instead of an algorithm to distribute objects
			public Texture2D cliffTexture;
			public Texture2D lowlandTexture;
	//		public Texture2D highlandTexture;

			//procedural heightmap generator settings
			public float xSeed;
			public float xSeed2;
			public float ySeed;
			public float ySeed2;
			public float amplitude;
			public float amplitude2;
			public float secondaryStrength;
			public float modifierStrength;
			public AnimationCurve terrRadialFalloff;

			public MultiGameLevel () {
				waterLevel = 0f;
				useWaterLevel = false;
				maxTerrainHeight = 100f;
				textureBorder1 = .0305f;
				textureBorder2 = .0531f;
				textureBorderBlend = 12f;
				cliffSteepness = 16f;
				highlandFactor = .0618f;
				lowlandFactor = .985f;
				importHeightMultiplier = 1f;
				terrainOffset = -25f;
				levelSize = new Vector3(1000f,1000f,1000f);

				xSeed = 0f;
				xSeed2 = 0;
				ySeed = 0f;
				ySeed2 = 0f;
				amplitude = .006f;
				amplitude2 = .0125f;
				secondaryStrength = 0.25f;
				modifierStrength = 10f;
				terrRadialFalloff = new AnimationCurve();
			}
		}

		//a class to handle 2D333333 perlin noise settings
		[System.Serializable]
		public class NoiseBlock {
			public float amplitude = .0125f;
			public float x = 0f;
			public float y = 0f;

			public void Randomize(float _scale) {
				x = Random.Range(-_scale, _scale);
				y = Random.Range(-_scale, _scale);
			}

			public float GetPerlin() {
				return Mathf.PerlinNoise(x*amplitude, y*amplitude);
			}

			public float GetPerlinOffset (float _offsetX, float _offsetY) {
				return Mathf.PerlinNoise (x*amplitude + _offsetX*amplitude, y*amplitude + _offsetY*amplitude);
			}

			public Texture2D GenerateCloudTexture (int _width, int _height, bool _alpha, bool _invert) {
				Texture2D _ret = new Texture2D(_width, _height);

				for (int _x = 0; _x < _width; _x++) {
					for (int _y = 0; _y < _height; _y++) {
						float _val = Mathf.PerlinNoise(x* amplitude + _x * amplitude , y*amplitude + _y*amplitude);

						if (!_invert) {
							if (_alpha)
								_ret.SetPixel(_x,_y, new Color(_val, _val, _val, _val));
							else
								_ret.SetPixel(_x,_y, new Color(_val, _val, _val, 1f));
						} else {
							if (_alpha)
								_ret.SetPixel(_x,_y, new Color(1 - _val, 1 - _val, 1 - _val, 1 - _val));
							else
								_ret.SetPixel(_x,_y, new Color(1 - _val, 1 - _val, 1 - _val, 1f));
						}
					}
				}

				return _ret;
			}
		}

		[System.Serializable]
		public class MultiGamePlayer {
			public string name = "";
			public GameObject prefabTemplate;
			public LayerMask mask;
			public GameObject image;


		}

		[System.Serializable]
		public class MultiGameNPC {
			public string name = "";
			public GameObject prefabTemplate;
			public LayerMask mask;
			public GameObject image;
		}

		[System.Serializable]
		public class MultiGameObject {
			public string name = "";//in-game name for this object
			public GameObject prefabTemplate;//optional prefab to supply. This is useful if you want to force all objects to have certain functionality
			public LayerMask mask;//layer mask for this type of object
			public GameObject image;//Optional image, if none is provided then one will be selected from any you may have provided for random selection, or a primitive will be used

			public enum ObjectTypes { 
				Static, //Permanent object generated into the scene. Will be lightmapped & baked with navigation
				Tree, //Terrain or Game Object tree. Terrain trees are fast on GPU but Game Object trees can be interacted with
				Destructible, //Any destructible object
				Building, //A procedurally-generated building
				Container, //An object containing some items for the Inventory system
				PlayerSpawn, //A place where new players can enter the game or continue from where they left off
				NPCSpawn, //A destructible object that spawns NPCs
				Portal //An entrance to another scene
			};

			public ObjectTypes objectType = ObjectTypes.Destructible;
		}

		[System.Serializable]
		public class MultiGameItem {
			public string name = "";//in-game name for this object
			public string key = "";
			public GameObject pickablePrefabTemplate;//optional prefab to supply. This is useful if you want to force all objects to have certain functionality
			public GameObject activePrefabTemplate;//optional prefab to supply. This is useful if you want to force all objects to have certain functionality
			public LayerMask mask;//layer mask for this type of object
			public GameObject image;//Optional image, if none is provided then one will be selected from any you may have provided for random selection, or a primitive will be used
		}

		//Allow the user to open the Game Generator window
		[MenuItem ("MultiGame/Experimental/Game Generator")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(GameGenerator));
		}

		
		void OnGUI () {

			if (generating) {
				EditorGUILayout.LabelField("Generating game content");
				return;
			}

			//------------------------------------------------------------------
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MultiGame Procedural Game Generator");
			EditorGUILayout.Space();
			GUI.backgroundColor = new Color(.4f,.4f,.9f);
			if(GUILayout.Button("Help"))
				help.showInfo = !help.showInfo;
			GUI.backgroundColor = Color.white;
			if (help.showInfo)
				EditorGUILayout.HelpBox(help.helpText, MessageType.Info);
			EditorGUILayout.Space();
			//--------------------General Settings------------------------------
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("General Settings");
			EditorGUILayout.Space();

			mainScrollView = EditorGUILayout.BeginScrollView(mainScrollView);
			prefabFolderPath = EditorGUILayout.TextField(new GUIContent("Prefab Folder Path"),prefabFolderPath);

			if (help.showInfo)
				EditorGUILayout.HelpBox ("Prefab Folder Path tells MultiGame where to save any prefabs it generates. This must be a 'Resources' folder in your project, not a subfolder " +
				                         "of one, otherwise Unity will generate errors when you try and run the game. Recommend Assets/Generated/Resources/", MessageType.Info);

			EditorGUILayout.Space();
		
			

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			//--------------------Level Editor----------------------------------
			showLevelEditor = EditorGUILayout.BeginToggleGroup("Level Generator",showLevelEditor);
			EditorGUILayout.BeginVertical("box");
			if (showLevelEditor) {

				if (help.showInfo)
					EditorGUILayout.HelpBox("Generate a new plane, terrain, ocean"/* TODO: , or dungeon */+" scene complete with object distribution, spawners, items, and enemies. Procedural " +
						"object placement can be driven by a distribution map, for custom height-based terrains, or procedurally.", MessageType.Info);


				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal("box");
				EditorGUILayout.BeginVertical("box");
				terrainFromAlgorithm = EditorGUILayout.Toggle("Procedural Terrain",terrainFromAlgorithm);
				terrainIsStatic = EditorGUILayout.Toggle("Terrain is static",terrainIsStatic);
				EditorGUILayout.EndHorizontal();

				level.levelSize = EditorGUILayout.Vector3Field("Level Size", level.levelSize);


				EditorGUILayout.BeginVertical("box");
				level.useWaterLevel = EditorGUILayout.Toggle("Use Water Level", level.useWaterLevel);
				if (level.useWaterLevel) {
					level.waterLevel = EditorGUILayout.FloatField("Water Level",level.waterLevel);
					level.waterMaterial = EditorGUILayout.ObjectField("Water Material", level.waterMaterial, typeof(Material), false) as Material;
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndVertical();


				if (help.showInfo)
					EditorGUILayout.HelpBox("Disable procedural terrain and assign a height map if you have one from another application you'd like to use. Otherwise, MultiGame" +
					                        "will attempt to generate a level based on your settings. The resolution of the terrain is mapped to world coordinates, so scaling the terrain " +
					                        "transform will change it's resolution relative to real world scale", MessageType.Info); 
				EditorGUILayout.BeginHorizontal("box");
				level.maxTerrainHeight = EditorGUILayout.FloatField("Max Terrain Height", level.maxTerrainHeight);
				if (level.heightMap != null && !terrainFromAlgorithm) {

					level.importHeightMultiplier = EditorGUILayout.FloatField("Height Multiplier", level.importHeightMultiplier);
				}
				level.terrainOffset = EditorGUILayout.FloatField("Terrain Offset", level.terrainOffset);
				
				EditorGUILayout.EndHorizontal();

				if (help.showInfo)
					EditorGUILayout.HelpBox("Max Terrain Height is used to indicate the maximum altitude of the terrain independently from Level Size and ajusting this will alter the " +
						"terrain generator's result. Terrain Offset adjusts the Y level of the terrain, which is useful if you want sea level to = Y = 0", MessageType.Info);


				ShowTerrainBox();
				



				

			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
			//------------------Player Editor-----------------------------------
			showPlayerEditor = EditorGUILayout.BeginToggleGroup("Player Generator", showPlayerEditor);
			if (showPlayerEditor) {

				playerEditorHelpView = EditorGUILayout.BeginScrollView(playerEditorHelpView, GUILayout.ExpandWidth(true));
	//			players = EditorGUILayout.ObjectField(players,  (System.Type)players.GetType(), false);
	//			if (players.Count < 1)
				EditorGUILayout.EndScrollView();

			}
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
			//------------------NPC Editor--------------------------------------
			showNPCEditor = EditorGUILayout.BeginToggleGroup("NPC Generator", showNPCEditor);
			

			npcEditorHelpView = EditorGUILayout.BeginScrollView(npcEditorHelpView, GUILayout.ExpandWidth(true));
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.Space();
			//-----------------Object Editor------------------------------------
			showObjectEditor = EditorGUILayout.BeginToggleGroup("Object Generator", showObjectEditor);
			

			objectEditorHelpView = EditorGUILayout.BeginScrollView(objectEditorHelpView, GUILayout.ExpandWidth(true));
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.Space();
			//------------------Item Editor-------------------------------------
			showItemEditor = EditorGUILayout.BeginToggleGroup("Item Generator",showItemEditor);
			

			itemEditorHelpView = EditorGUILayout.BeginScrollView(itemEditorHelpView, GUILayout.ExpandWidth(true));
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.Space();
			//------------------------------------------------------------------
			EditorGUILayout.Space();
			
			EditorGUILayout.Space();

			if (!generating) {
				if (!generateSelected)
					generateAll = EditorGUILayout.Toggle("Generate All", generateAll);
				if (!generateAll)
					generateSelected = EditorGUILayout.Toggle("Generate Selected", generateSelected);
			}

			if (generateSelected) {
				GenerateSelected();
				generateSelected = false;
				generateAll = false;
			} else if (generateAll) {
				GenerateAll();
				generateAll = false;
				generateSelected = false;
			}
			EditorGUILayout.Space();



			//error display, at the bottom
			clearErrors = EditorGUILayout.Toggle("Clear error list" , clearErrors);
			if (clearErrors) {
				clearErrors = false;
				errors.Clear();
				errors.Insert(0,"Clear");
			}
			errorView = EditorGUILayout.BeginScrollView(errorView, "box", GUILayout.ExpandWidth(true), GUILayout.Height(128f));

			foreach (string _err in errors) 
				EditorGUILayout.LabelField(_err);

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndScrollView();
		}

		private void ShowTerrainBox() {
			//mapping assignments for non-procedural terrain
			if (!terrainFromAlgorithm) {
				EditorGUILayout.BeginHorizontal("box");
				level.heightMap = (Texture2D)EditorGUILayout.ObjectField("Height Map", level.heightMap, typeof(Texture2D), false);
				level.distributionMap = (Texture2D)EditorGUILayout.ObjectField("Distribution Map",level.distributionMap, typeof(Texture2D), false);
				EditorGUILayout.EndHorizontal();
				ShowTextureBox();
				
			}
			
			//procedural settings
			if (terrainFromAlgorithm) {
				EditorGUILayout.BeginHorizontal("box");//Island
				
				level.terrRadialFalloff = EditorGUILayout.CurveField("Island Modifier", level.terrRadialFalloff);
				level.modifierStrength = EditorGUILayout.Slider("Modifier Strength", level.modifierStrength,0.001f,1f);
				EditorGUILayout.EndHorizontal();//Island ^
				
				if (help.showInfo)
					EditorGUILayout.HelpBox("Left(0) represents the height at the center of the island, while the right side of the graph represents (1) the edge of the island terrain. This" +
					                        " will modify the height of the terrain in a radial fashion, allowing you to create hills, valleys, plateus, etc and control the way the terrain flows around the edges and center " +
					                        "without needing to edit the mapping by hand in an external application like Photoshop or GIMP.", MessageType.Info);
				
				EditorGUILayout.BeginHorizontal("box");//2
				level.amplitude = EditorGUILayout.FloatField ("Noise Amplitude",level.amplitude);
				level.amplitude2 = EditorGUILayout.FloatField ("Secondary Amplitude",level.amplitude2);
				EditorGUILayout.EndHorizontal();//2
				if (help.showInfo)
					EditorGUILayout.HelpBox("Amplitude is the 'waviness' and you should set this very low. If you are having trouble, try 0.004. Modifier strength controls how much the 'Island Modifier' " +
					                        "influences the shape. The secondary amplitude corresponds with the secondary noise (of course) and this is added on top of the result from the main " +
					                        "noise multiplied by the island modifier." , MessageType.Info);
				
				EditorGUILayout.BeginHorizontal("box");
				
				EditorGUILayout.BeginVertical("box");
				randomTerrainSeed = EditorGUILayout.Toggle("Random Seed", randomTerrainSeed);
				level.secondaryStrength = EditorGUILayout.FloatField ("2nd Noise Power", level.secondaryStrength);
				EditorGUILayout.EndVertical();
				
				if (!randomTerrainSeed) {
					
					EditorGUILayout.BeginVertical("box");
					level.xSeed = EditorGUILayout.FloatField ("X Seed", level.xSeed);
					level.ySeed = EditorGUILayout.FloatField ("Y Seed", level.ySeed);
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("box");
					level.xSeed2 = EditorGUILayout.FloatField ("X Seed 2", level.xSeed2);
					level.ySeed2 = EditorGUILayout.FloatField ("Y Seed 2", level.ySeed2);
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();

				if (help.showInfo)
					EditorGUILayout.HelpBox ("Random Seed will generate a new seed every time, disable this if you want to fine-tune the generated result instead. " +
					                         "The seed is represented as an X and Y coordinate, meaning you can move the seed location a few units to one side to move a mountain as much. The " +
					                         "first seed corresponds with the 'main' noise, which generates height variations and is multiplied by the Island Modifier. After that, the 'secondary' noise " +
					                         "is applied to add variations to the terrain. ", MessageType.Info);

				proceduralSplatmap = EditorGUILayout.Toggle("Procedural Texture", proceduralSplatmap);
				if (help.showInfo)
					EditorGUILayout.HelpBox ("If enabled, MultiGame will generate a splat map for you, based on some settings.", MessageType.Info);

				if (proceduralSplatmap)
					ShowTextureBox();
				proceduralDistribMap = EditorGUILayout.Toggle("Procedural Distribution Map", proceduralDistribMap);
				if (help.showInfo)
					EditorGUILayout.HelpBox("If enabled, MultiGame will automatically paint detail meshes and grass billboards across the terrain", MessageType.Info);
				if (proceduralDistribMap)
					ShowDetailBox();
				
			}
		}

		private void ShowTextureBox () {
			EditorGUILayout.BeginVertical("box");
			
			EditorGUILayout.BeginHorizontal("box");
			//---------


			EditorGUILayout.BeginVertical("box");
			level.texture1 = (Texture2D)EditorGUILayout.ObjectField("Lowland", level.texture1, typeof(Texture2D), false);
			level.texture2 = (Texture2D)EditorGUILayout.ObjectField("Midland", level.texture2, typeof(Texture2D), false);
			level.texture3 = (Texture2D)EditorGUILayout.ObjectField("Highland", level.texture3, typeof(Texture2D), false);
			level.cliffTexture = (Texture2D)EditorGUILayout.ObjectField("Cliff", level.cliffTexture, typeof(Texture2D), false);
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginVertical("box");
	//		level.highlandTexture = (Texture2D)EditorGUILayout.ObjectField("Highlands", level.highlandTexture, typeof(Texture2D), false);
			level.lowlandTexture = (Texture2D)EditorGUILayout.ObjectField("Flatlands", level.lowlandTexture, typeof(Texture2D), false);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical("box");
			level.textureBorder1 = EditorGUILayout.Slider("Low Texture Border", level.textureBorder1,0.002f,.098f);
			level.textureBorder2 = EditorGUILayout.Slider("High Texture Border", level.textureBorder2,0.002f,.098f);
			level.textureBorderBlend = EditorGUILayout.Slider("Texture Blend", level.textureBorderBlend,.001f,25f);
			level.cliffSteepness = EditorGUILayout.Slider("Min. Cliff Steepness", level.cliffSteepness,.001f,100f);
	//		level.highlandFactor = EditorGUILayout.Slider("Highland Cutoff", level.highlandFactor,.002f,.098f);
			level.lowlandFactor = EditorGUILayout.Slider("Flatland Factor", level.lowlandFactor,.95f,4.05f);
			EditorGUILayout.EndVertical();

			if (help.showInfo)
				EditorGUILayout.HelpBox ("The Low, Mid, and Highland textures are applied first based on altitude, then the Flatlands (dirt) is blended on top. " +
				                         "Finally, the cliff texture is applied based on slope. Low and High texture borders control the altitude borders between the three base textures. " +
				                         "Texture Blend controls the harshness of the fade between texture border lines. Min. Cliff Steepness controls the cutoff at which we begin blending in " +
				                         "cliff texture, and Flatland Factor controls the coverage of Flatland over the terrain considering both altitude and slope.", MessageType.Info);

			//---------
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.EndVertical();

		}

		private void ShowDetailBox () {
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal("box");
			//--------
			EditorGUILayout.BeginVertical("box");
			
			level.detail1 = EditorGUILayout.ObjectField("Lowland Detail",level.detail1, typeof(GameObject), false) as GameObject;
			level.detail2 = EditorGUILayout.ObjectField("Midland Detail",level.detail2, typeof(GameObject), false) as GameObject;
			level.detail3 = EditorGUILayout.ObjectField("Highland Detail",level.detail3, typeof(GameObject), false) as GameObject;
			level.flatlandDetail = EditorGUILayout.ObjectField("Flatland Detail",level.flatlandDetail, typeof(GameObject), false) as GameObject;

			if (help.showInfo)
				EditorGUILayout.HelpBox ("Low, Mid, Highland, and Flatland detail meshes and grass textures. Detail meshes should be very small, as Unity can render hundreds of them up close at once. " +
				                         "MultiGame will distribute these based on the textures on the terrain.", MessageType.Info);


			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical("box");
			level.grass1 = EditorGUILayout.ObjectField("Lowland Grass", level.grass1, typeof(Texture2D), false) as Texture2D;
			level.grass2 = EditorGUILayout.ObjectField("Midland Grass", level.grass2, typeof(Texture2D), false) as Texture2D;
			level.grass3 = EditorGUILayout.ObjectField("Highland Grass", level.grass3, typeof(Texture2D), false) as Texture2D;
			level.flatlandGrass = EditorGUILayout.ObjectField("Flatland Grass", level.flatlandGrass, typeof(Texture2D), false) as Texture2D;
			EditorGUILayout.EndVertical();
			
			//---------
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();


		}

		private void GenerateLevel () {
			generating = true;
			errors.Insert(0,"Generating level");

			//sea level handling
			if (level.useWaterLevel) {
				if (water == null) {
					water = GameObject.CreatePrimitive(PrimitiveType.Plane);
					level.waterMaterial = Resources.Load<Material>("Water");
				}

				water.transform.localScale = new Vector3(level.levelSize.x, 1f, level.levelSize.z);
				water.name = "Water";
	//			water.layer = 3;
				water.transform.position = new Vector3(0f, level.waterLevel, 0f);
				Renderer _rend = water.GetComponent<Renderer>();
				if (level.waterMaterial != null)
					_rend.sharedMaterial = level.waterMaterial;

			}

			//decide which generator to use
			if (!terrainFromAlgorithm) {
				//build a Unity terrain or flat plane

				if (terrain == null)
					terrain = FindObjectOfType<Terrain>();
				if (terrain != null)
					SplatmapUnityTerrain();

				if (level.heightMap == null) {//no heightmap, no procedure, so just build a flat level. This can also be used for oceans although heightmap is recommended
					if (floor == null)
						floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
					floor.transform.localScale = new Vector3(this.level.levelSize.x,1f,this.level.levelSize.z);
					floor.isStatic = terrainIsStatic;
					floor.transform.position = Vector3.zero;
					floor.transform.Translate(0f, level.terrainOffset, 0f);
				} else {
					HeightmapUnityTerrain();
				}

			} else {//fast and dirty terrain generator
				GenerateUnityTerrain();
			}

			//TODO: Populate the level with objects, items, enemies
		}

		//user opted for procedural Unity terrain, generate one now
		private void GenerateUnityTerrain () {
			errors.Insert(0,"Generating Unity terrain procedurally");
			terrain = FindObjectOfType<Terrain>();
			
			if (terrain == null)
				terrain = new GameObject("Terrain", typeof(Terrain), typeof(TerrainCollider)).GetComponent<Terrain>();

			terrain.gameObject.isStatic = terrainIsStatic;
			terrain.gameObject.transform.position = Vector3.zero;
			terrain.gameObject.transform.Translate(0f, level.terrainOffset, 0f);

			if (terrain.terrainData == null)
				terrain.terrainData = new TerrainData();

			try {
				terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
			} catch {
				errors.Insert(0,"Terrain failed to assign data to a collider!");
			}

			terrain.terrainData.heightmapResolution = (int)Mathf.Ceil(level.levelSize.x);
			terrain.terrainData.size = new Vector3(level.levelSize.x, level.maxTerrainHeight, level.levelSize.x);
			newHeights = new float[terrain.terrainData.heightmapResolution,terrain.terrainData.heightmapResolution];
			
			terrain.gameObject.transform.Translate(-.5f * terrain.terrainData.size.x, 0f, -.5f * terrain.terrainData.size.z);// center the terrain on the world origin automatically
			
			for (int i = 0; i < this.terrain.terrainData.heightmapResolution; i++) {
				for (int j = 0; j < this.terrain.terrainData.heightmapResolution; j++) {


					//Put some noise into the heightmap,
					newHeights[j,i] = Mathf.PerlinNoise(
						((level.xSeed * level.amplitude) + (i * level.amplitude)) , 
						((level.ySeed * level.amplitude) + (j * level.amplitude))
					);
					//then multiply the heightmap by the island modifier to allow control of the type of terrain we're making:
					//- flat gives us raw noise data
					//- slope, with the hill on the left gives an island
					//- slope, with the hill on the right gives a valley
					//- Since it's a curve-based modifier, the designer has fast and easy control. Want an island with a volcano, or a lake in the middle? Easy.
					//this makes it a lot easier to rapidly get the type of scene you want, instead of placing everything by hand or hitting 'Generate' a million times
					if (level.terrRadialFalloff.keys.Length < 1) {
						level.terrRadialFalloff.AddKey(0f,1f);//if the curve is empty, make an island automatically
						level.terrRadialFalloff.AddKey(1f, 0f);
					}
					newHeights[j,i] *= (((this.level.terrRadialFalloff.Evaluate(NormalizedDistanceFromCenter(i,j)))*level.modifierStrength) * level.maxTerrainHeight) / level.maxTerrainHeight;

					//a secondary noise is added, to give some additional interest & texture.
					newHeights[j,i] += level.secondaryStrength * Mathf.PerlinNoise(
						((level.xSeed2 * level.amplitude2) + (i * level.amplitude2)) , 
						((level.ySeed2 * level.amplitude2) + (j * level.amplitude2))
					);
					
				}
			}
			
			terrain.terrainData.SetHeights(0,0,newHeights);

			if (proceduralSplatmap)
				SplatmapUnityTerrain();

			if (proceduralDistribMap)
				DistributionMapUnityTerrain();
		}

		private void FillMissingDetails () {

			DetailPrototype[] _newDetails;
			//detail meshes
			DetailPrototype _d1 = new DetailPrototype();
//			terrain.terrainData.det
			DetailPrototype _d2 = new DetailPrototype();
			DetailPrototype _d3 = new DetailPrototype();
			DetailPrototype _df = new DetailPrototype();

			//grass billboards
			DetailPrototype _g1 = new DetailPrototype();
			DetailPrototype _g2 = new DetailPrototype();
			DetailPrototype _g3 = new DetailPrototype();
			DetailPrototype _gf = new DetailPrototype();
			
			//detail meshes
//			if (terrain.terrainData.detailPrototypes[0] == null) {
				_d1.prototype = Resources.Load("Boulder") as GameObject;
//			}
//			else
//				_d1.prototype = terrain.terrainData.detailPrototypes[0].prototype;


//			if (terrain.terrainData.detailPrototypes[1] == null) {
				_d2.prototype = Resources.Load("Boulder") as GameObject;
//			}
//			else
//				_d2.prototype = terrain.terrainData.detailPrototypes[1].prototype;

//			if (terrain.terrainData.detailPrototypes[2] == null) {
				_d3.prototype = Resources.Load("Boulder") as GameObject;
//			}
//			else
//				_d3.prototype = terrain.terrainData.detailPrototypes[2].prototype;

//			if (terrain.terrainData.detailPrototypes[3] == null) {
				_df.prototype = Resources.Load("Boulder") as GameObject;
//			}
//			else
//				_df.prototype = terrain.terrainData.detailPrototypes[3].prototype;

			//grass billboards

//			if (terrain.terrainData.detailPrototypes[4] == null) {
			_g1.prototypeTexture = Resources.Load("GrassBlades") as Texture2D;
//			}
//			else
//				_g1.prototype = terrain.terrainData.detailPrototypes[4].prototype;

//			if (terrain.terrainData.detailPrototypes[5] == null) {
			_g2.prototypeTexture = Resources.Load("GrassBlades") as Texture2D;
//			}
//			else
//				_g2.prototype = terrain.terrainData.detailPrototypes[5].prototype;

//			if (terrain.terrainData.detailPrototypes[6] == null) {
			_g3.prototypeTexture = Resources.Load("GrassBlades") as Texture2D;
//			}
//			else
//				_g3.prototype = terrain.terrainData.detailPrototypes[6].prototype;

//			if (terrain.terrainData.detailPrototypes[7] == null) {
			_gf.prototypeTexture = Resources.Load("GrassBlades") as Texture2D;
//			}
//			else
//				_gf.prototype = terrain.terrainData.detailPrototypes[7].prototype;

			_d1.usePrototypeMesh = true;
			_d2.usePrototypeMesh = true;
			_d3.usePrototypeMesh = true;

			_g1.usePrototypeMesh = false;
			_g2.usePrototypeMesh = false;
			_g3.usePrototypeMesh = false;
			_gf.usePrototypeMesh = false;

			//assign all to the terrain array
			_newDetails = new DetailPrototype[8] {_d1, _d2, _d3, _df, _g1, _g2, _g3, _gf};

//			for (int p = 0; 0 < 8; p++) {
//				if (p < 4)
//					_newDetails[p].usePrototypeMesh = true;
//				else
//					_newDetails[p].usePrototypeMesh = false;
//			}

			terrain.terrainData.detailPrototypes = _newDetails;


		}

		private void DistributionMapUnityTerrain () {
			terrain.terrainData.SetDetailResolution((int)level.levelSize.x, 16);

			FillMissingDetails();

//			float y_01;
//			float x_01;
//			float height;
//			float steepness;

			float[,,] alphaMap = terrain.terrainData.GetAlphamaps(0,0,terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

			if (level.distributionMap != null) {
				int[,] _map;
				for (int l = 0; l < terrain.terrainData.detailPrototypes.Length; l++ ) {
					_map = terrain.terrainData.GetDetailLayer(0,0,terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, l);
					for (int i= 0; i < terrain.terrainData.detailWidth; i++) {
						for (int j = 0; j < terrain.terrainData.detailHeight; j++) {

							// Normalise x/y coordinates to range 0-1
//							y_01 = (float)i/(float)terrain.terrainData.alphamapHeight;
//							x_01 = (float)j/(float)terrain.terrainData.alphamapWidth;

//							steepness = terrain.terrainData.GetSteepness(y_01,x_01);
//							height = terrain.terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrain.terrainData.heightmapHeight),Mathf.RoundToInt(x_01 * terrain.terrainData.heightmapWidth) );

							switch (l) {//set the detail for each based on the terrain texture
							case 0:
								_map[i,j] = Mathf.RoundToInt( alphaMap[i,j,0] * level.detail1Str);
								break;
							case 1:
								_map[i,j] = Mathf.RoundToInt( alphaMap[i,j,1] * level.detail2Str);
								break;
							case 2:
								_map[i,j] = Mathf.RoundToInt( alphaMap[i,j,2] * level.detail3Str);
								break;
							case 3://flatland
								_map[i,j] = Mathf.RoundToInt( alphaMap[i, j, 3] * level.flatlandDetailStr);
								break;

								//Also set the grass billboards
							}
							terrain.terrainData.SetDetailLayer(0,0,l, _map);
						}
					}
				}
			}
		}
			
		private void DistributeGameObjects () {
			//TODO: distribute discreet objects
		}

		private void SplatmapUnityTerrain () {
			errors.Insert(0, "Splatmapping Unity Terrain");
			terrain.terrainData.alphamapResolution = terrain.terrainData.heightmapResolution;
			splatMap = new float[terrain.terrainData.alphamapWidth,terrain.terrainData.alphamapHeight,5];//there are 5 layers on a generated mapping


			FillMissingTextures();

			float y_01;
			float x_01;
			
			float height;
//			Vector3 normal;
			float steepness;
			float[] splatWeights = new float[terrain.terrainData.alphamapLayers];
//			float percentOfHeight;

			for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
			{
				for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
				{

					// Normalise x/y coordinates to range 0-1
					y_01 = (float)y/(float)terrain.terrainData.alphamapHeight;
					x_01 = (float)x/(float)terrain.terrainData.alphamapWidth;

					height = terrain.terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrain.terrainData.heightmapResolution),Mathf.RoundToInt(x_01 * terrain.terrainData.heightmapResolution) );
//					normal = terrain.terrainData.GetInterpolatedNormal(y_01,x_01);
					steepness = terrain.terrainData.GetSteepness(y_01,x_01);
//					percentOfHeight = height / terrain.terrainData.heightmapHeight;

					//first, we will do the underpainting, the three main textures are generated in first
	//				splatWeights[1] = splatWeights[0] + percentOfHeight * level.textureBorder2;//grassland/forest
					splatWeights[0] =  Mathf.Clamp01((terrain.terrainData.heightmapResolution*level.textureBorder1 - height)/level.textureBorderBlend);
					splatWeights[1] = .5f;//splatWeights[1] + percentOfHeight;//grassland/forest
					splatWeights[2] = Mathf.Clamp01(1f- ((terrain.terrainData.heightmapResolution*level.textureBorder2 - height)/(level.textureBorderBlend)));
	//				splatWeights[3] = Mathf.Clamp01( ( Mathf.Clamp01(1f- ((terrain.terrainData.heightmapHeight*level.highlandFactor - height)/(level.textureBorderBlend)*10f)) * Mathf.Clamp01(2 * normal.normalized.y)) / level.textureBorderBlend);
					splatWeights[3] = Mathf.Clamp01 (   ((  (1.0f - Mathf.Clamp01((steepness*steepness/(terrain.terrainData.heightmapResolution)    )/* * level.lowlandFactor*/  + height* (1-level.lowlandFactor) )     ) )/* - splatWeights[3]*/) /(level.textureBorderBlend) );

	//				splatWeights[3] =  (height * Mathf.Clamp01(normal.normalized.y/level.textureBorderBlend));
	//
	//				splatWeights[4] = Mathf.Clamp01( ( (terrain.terrainData.heightmapHeight * level.lowlandFactor - height * level.lowlandFactor) * Mathf.Clamp01(normal.y)) / level.textureBorderBlend);
	//				//next we can do the cliff/slope texture
					splatWeights[4] = Mathf.Clamp01( (steepness - level.cliffSteepness)/level.textureBorderBlend);//cliff
					//splatWeights[3] =Mathf.Clamp01(1f- ((((terrain.terrainData.heightmapHeight*level.textureBorder2) * (level.highlandFactor/100f)) - height)/level.textureBorderBlend));//snowcap
	//
	//				splatWeights[4] = height * (1f - Mathf.Clamp01(steepness*steepness/(terrain.terrainData.heightmapHeight/5f)));//Highland
	//				splatWeights[5] = (1f - Mathf.Clamp01(steepness*steepness/(terrain.terrainData.heightmapHeight/5f)))/height;//Lowland
	//
	////				//now we can actually adjust the texture weights:
	////
	////				// Texture[0] has constant influence
	////				splatWeights[0] = 0.5f;
	////				
	////				// Texture[1] is stronger at lower altitudes
	////				splatWeights[1] = Mathf.Clamp01((terrain.terrainData.heightmapHeight - height));
	////				
	////				// Texture[2] stronger on flatter terrain
	////				// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
	////				// Subtract result from 1.0 to give greater weighting to flat surfaces
	////				splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrain.terrainData.heightmapHeight/5.0f));
	////				
	////				// Texture[3] increases with height but only on surfaces facing positive Z axis
	////				splatWeights[3] = height * Mathf.Clamp01(normal.z);
	//
	//
	//
					// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
					float z = 0;

					foreach(float _f in splatWeights)
						z+=_f;
					
					// Loop through each terrain texture
					for(int k = 0; k<terrain.terrainData.alphamapLayers; k++){
						
						// Normalize so that sum of all texture weights = 1
						splatWeights[k] /= z;
						
						// Assign this point to the splatmap array
						try {
							splatMap[x, y, k] = splatWeights[k];
						} catch {
							errors.Insert(0,"Index out of range " + x + ", " + y + ", " + k);
						}
					}
				}
			}
			terrain.terrainData.SetAlphamaps(0,0,splatMap);

		}

		private void FillMissingTextures () {

			if (level.texture1 == null) {//gradial low
				level.texture1 = new Texture2D(2,2);

				Color _tx1 = new Color(.7f, .7f, .56f, .098f);
				level.texture1.SetPixels( new Color[]{ _tx1 , _tx1 , _tx1 , _tx1});
			}
				
			if (level.texture2 == null) {//gradial medium
				level.texture2 = new Texture2D(2,2);

				Color _tx2 = new Color(.2f, .35f, .2f, .1f);
				level.texture2.SetPixels( new Color[]{ _tx2 , _tx2 , _tx2 , _tx2});
			}
				
			if (level.texture3 == null) {//gradial high
				level.texture3 = new Texture2D(2,2);

				Color _tx3 = new Color(.34f, .34f, .34f, .025f);
				level.texture3.SetPixels( new Color[]{ _tx3 , _tx3 , _tx3 , _tx3});
			}

			if (level.lowlandTexture == null) {//low flatland
				level.lowlandTexture = new Texture2D(2,2);

				Color _low = new Color(.475f, .475f, .32f, .1f);
				level.lowlandTexture.SetPixels( new Color[]{ _low , _low , _low , _low});
			}
	//		
			//		if (level.highlandTexture == null && terrain.terrainData.alphamapTextures[4] == null) {//high flatland
	//			level.highlandTexture = new Texture2D(2,2);
	//			
	//			Color _high = new Color(.95f, .95f, .98f, .1f);
	//			level.highlandTexture.SetPixels( new Color[]{ _high , _high , _high , _high});
	//		}

			if (level.cliffTexture == null) {//high steepness
				level.cliffTexture = new Texture2D(2,2);
				
				Color _cliff = new Color(.2f, .2f, .18f, .025f);
				level.cliffTexture.SetPixels( new Color[]{ _cliff , _cliff , _cliff , _cliff});
			}

	//		Texture2D normalMap = new Texture2D(2,2);
			
	//		normalMap.SetPixels(new Color[] {Color.blue, Color.blue, Color.blue, Color.blue});

			SplatPrototype _spl1 = new SplatPrototype();
			_spl1.texture = level.texture1;
	//		_spl1.normalMap = normalMap;

			SplatPrototype _spl2 = new SplatPrototype();
			_spl2.texture = level.texture2;
	//		_spl2.normalMap = normalMap;
			
			SplatPrototype _spl3 = new SplatPrototype();
			_spl3.texture = level.texture3;
	//		_spl3.normalMap = normalMap;
			
	//		SplatPrototype _shi = new SplatPrototype();
	//		_shi.texture = level.highlandTexture;
	//		_shi.normalMap = normalMap;
			
			SplatPrototype _slo = new SplatPrototype();
			_slo.texture = level.lowlandTexture;
	//		_slo.normalMap = normalMap;
			
			SplatPrototype _sclf = new SplatPrototype();
			_sclf.texture = level.cliffTexture;
	//		_sclf.normalMap = normalMap;
			
			terrain.terrainData.splatPrototypes = new SplatPrototype[]{ _spl1, _spl2, _spl3,/* _shi,*/ _slo, _sclf };

		}

		//user supplied a heightmap, apply it using settings
		private void HeightmapUnityTerrain () {
			errors.Insert(0,"Generating Unity terrain from heightmap");

			terrain = FindObjectOfType<Terrain>();

			if (terrain == null)
				terrain = new GameObject("Terrain", typeof(Terrain), typeof(TerrainCollider)).GetComponent<Terrain>();

			terrain.gameObject.isStatic = terrainIsStatic;
			terrain.gameObject.transform.position = Vector3.zero;
			terrain.gameObject.transform.Translate(0f, level.terrainOffset, 0f);
			
			if (terrain.terrainData == null)
				terrain.terrainData = new TerrainData();

			try {
				terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
				
			} catch {
				errors.Insert(0,"Terrain failed to assign data to a collider!");
			}
			
			try {

				terrain.terrainData.heightmapResolution = level.heightMap.width;
				terrain.terrainData.size = new Vector3(terrain.terrainData.size.x, level.maxTerrainHeight, terrain.terrainData.size.z);
				newHeights = new float[level.heightMap.width,level.heightMap.height];
				
				terrain.gameObject.transform.Translate(-.5f * terrain.terrainData.size.x, 0f, -.5f * terrain.terrainData.size.z);// center the terrain on the world origin automatically
				
				for (int i = 0; i < this.level.heightMap.width; i++) {
					for (int j = 0; j < this.level.heightMap.width; j++) {
						
						newHeights[j,i] = level.heightMap.GetPixel(j,i).r  * level.importHeightMultiplier;
					}
				}
				
				terrain.terrainData.SetHeights(0,0,newHeights);
				
			} catch {
				errors.Insert(0,"Error! heightmap must be set to 'Advanced' texture type, with read/write enabled!");
			}

			//next, it needs textures!
			SplatmapUnityTerrain();
		}

		private void GeneratePlayers () {
			generating = true;
		}

		private void GenerateNPCs () {
			generating = true;
		}

		private void GenerateObjects () {
			generating = true;
			if (terrainFromAlgorithm) {
				//TODO: Generate distribution for objects

	//			terrain.terrainData.
			}
		}

		private void GenerateItems () {
			generating = true;
		}

		public void GenerateSelected () {
			if (randomTerrainSeed) {
				level.xSeed = Random.Range(-10000, 10000);
				level.xSeed2 = Random.Range(-10000, 10000);
				level.ySeed = Random.Range(-10000, 10000);
				level.ySeed2 = Random.Range(-10000, 10000);
			}
			if (showItemEditor)
				GenerateItems();
			if (showNPCEditor)
				GenerateNPCs();
			if (showObjectEditor)
				GenerateObjects();
			if (showPlayerEditor)
				GeneratePlayers ();
			if (showLevelEditor)
				GenerateLevel();
			generating = false;
			errors.Insert(0,"MultiGame generated successfully!");
		}

		public void GenerateAll () {
			if (randomTerrainSeed) {
				level.xSeed = Random.Range(-10000, 10000);
				level.xSeed2 = Random.Range(-10000, 10000);
				level.ySeed = Random.Range(-10000, 10000);
				level.ySeed2 = Random.Range(-10000, 10000);
			}
			GenerateItems();
			GenerateNPCs();
			GenerateObjects();
			GeneratePlayers ();
			GenerateLevel();
			generating = false;
			errors.Insert(0,"MultiGame generated successfully!");
		}

		public GameObject MakeRandomPrimitive() {
			int _rand = Random.Range(0, 3);
			GameObject _prim = null;

			switch (_rand) {
			case 0:
				_prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
				break;
			case 1:
				_prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				break;
			case 2:
				_prim = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				break;
			case 3:
				_prim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				break;
			}

			return _prim;
		}

		public float NormalizedDistanceFromCenter(float _x, float _y) {
			float _center = (terrain.terrainData.heightmapResolution * .5f);

			return Vector2.Distance(new Vector2(_x, _y), new Vector2(_center,_center)) / _center ;
		}

	}
}
//MultiGame and all related materials are copyright 2015 William Hendrickson all rights reserved.






















































