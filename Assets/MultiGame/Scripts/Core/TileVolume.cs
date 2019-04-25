using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class TileVolume : MultiModule {


		[Tooltip("How many tiles are in the grid?")]
		public IntVector3 size = new IntVector3(100,100,100);
		[Tooltip("How large is each tile in the grid?")]
		public Vector3 tileSize = Vector3.one;

		[Tooltip("Should we shuffle the tile prototypes automatially?")]
		public bool shuffle = true;
		[Tooltip("Are we editing the tiles in the Unity editor right now?")]
		public bool editMode = false;

		public int currentFilled = 0;
		public int currentFloor = 0;
		public int currentWall = 0;

		[HideInInspector]
		public Tile[,,] tiles;

		[Tooltip("Should the border tiles be open by default? If false, these will be filled.")]
		public bool openBorders = true;

		[Tooltip("Prefabs used for filled tiles")]
		public GameObject[] filled;
		[Tooltip("Prefabs for floors are placed when a filled tile is underneath")]
		public GameObject[] floor;
		[Tooltip("Wall prefabs are placed on top of the floor, covering filled tile edges")]
		public GameObject[] wall;
		[Tooltip("Ceiling prefabs are placed when a filled tile is above")]
		public GameObject[] ceiling;
		private GameObject autoPlane;
		private Collider planeCollider;
		private RaycastHit hinfo;
		private bool didHit = false;
		[HideInInspector]
		public IntVector3 gridPoint;

		[System.Serializable]
		public class Tile {
			public GameObject rootObj;
			public IntVector3 position;
			public enum Modalities {Filled, Floor, Empty};
			public Modalities modality = Modalities.Empty;
			public GameObject floor;
			public GameObject ceiling;
			public GameObject fWall;
			public GameObject bWall;
			public GameObject lWall;
			public GameObject rWall;
			public GameObject fill;
			public Decorator[] decorators;
		}

		[System.Serializable]
		public class Decorator {
			public enum DecorationTypes {Free, Floor, Wall, Ceiling};
			[Tooltip("'Free' decorations are not parented to the tile but snap to surface when spawned, other types are parented to the appropriate part of the tile.")]
			public DecorationTypes decorationType = DecorationTypes.Free;
			[Tooltip("How much clearence on each axis (in tiles) this needs")]
			public IntVector3 minimumClearence = IntVector3.zero;
			[Tooltip("The collision layers of objects we want this decorator to be spaced apart from")]
			public LayerMask clearenceRayMask = LayerMask.NameToLayer("Default");
			[RequiredFieldAttribute("The prefab we want to spawn as a decoration")]
			public GameObject decorationPrefab;
		}

		public void UpdateTile (Tile _tile) {
			bool f, b, l, r, u, d = false;
			ClearTile(_tile);
			_tile.rootObj = new GameObject("Tile ( " + _tile.position.x + ", " + _tile.position.y + ", " + _tile.position.z + " )");

			if (CheckIsBorderTile(_tile)) {
				if (openBorders)
					_tile.modality = Tile.Modalities.Empty;
				else
					_tile.modality = Tile.Modalities.Filled;
			} else {
				//not on the border, so we can assume GetTile won't return null for these
				if (!CheckIsIsland(_tile) && !CheckIsEnclosed(_tile)) {
					f = (GetTile(_tile.position.x, _tile.position.y, _tile.position.z + 1).modality == Tile.Modalities.Filled);
					b = (GetTile(_tile.position.x, _tile.position.y, _tile.position.z - 1).modality == Tile.Modalities.Filled);
					l = (GetTile(_tile.position.x - 1, _tile.position.y, _tile.position.z).modality == Tile.Modalities.Filled);
					r = (GetTile(_tile.position.x + 1, _tile.position.y, _tile.position.z).modality == Tile.Modalities.Filled);
					u = (GetTile(_tile.position.x, _tile.position.y + 1, _tile.position.z).modality == Tile.Modalities.Filled);
					d = (GetTile(_tile.position.x, _tile.position.y - 1, _tile.position.z).modality == Tile.Modalities.Filled);

					if (f) {
						_tile.fWall = Instantiate(wall[Random.Range(0, wall.Length)],transform.position,transform.rotation) as GameObject;
					}
					if (b) {
						_tile.bWall = Instantiate(wall[Random.Range(0, wall.Length)],transform.position,transform.rotation) as GameObject;
						_tile.bWall.transform.RotateAround(transform.position,Vector3.up, 180f);
					}
					if (l) {
						_tile.lWall = Instantiate(wall[Random.Range(0, wall.Length)],transform.position,transform.rotation) as GameObject;
						_tile.lWall.transform.RotateAround(transform.position,Vector3.up, 270f);
					}
					if (r) {
						_tile.rWall = Instantiate(wall[Random.Range(0, wall.Length)],transform.position,transform.rotation) as GameObject;
						_tile.rWall.transform.RotateAround(transform.position,Vector3.up, 90f);
					}
					if (u)
						_tile.ceiling = Instantiate(ceiling[Random.Range(0, ceiling.Length)],transform.position,transform.rotation) as GameObject;
					if (d)
						_tile.floor = Instantiate(floor[Random.Range(0, floor.Length)],transform.position,transform.rotation) as GameObject;

//					_tile.fWall.transform.SetParent(

				} else {
					if (CheckIsEnclosed(_tile)) {
						//TODO: FINISH THIS
					} else {
						
					}
				}

			}
		}

		void OnSceneGUI () {
			#if UNITY_EDITOR

			if(!editMode) {
				if(autoPlane != null)
					DestroyImmediate(autoPlane);
				return;
			}
			if (autoPlane == null)
				autoPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			if (Camera.current == null)
				return;
			Gizmos.color = new Color(1f,1f,1f,.7f);
			autoPlane.GetComponent<MeshRenderer>().sharedMaterial.color = Gizmos.color;
			autoPlane.transform.localPosition = Vector3.zero;
			if (planeCollider == null)
				planeCollider = autoPlane.GetComponent<Collider>();

			didHit = planeCollider.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition),out hinfo, 100000f);
			if (!didHit)
				return;

			Vector3 _locusPoint = new Vector3(Mathf.RoundToInt( hinfo.point.x), Mathf.RoundToInt( hinfo.point.y), Mathf.RoundToInt( hinfo.point.z));
			Gizmos.DrawWireCube(_locusPoint, transform.localScale);
			//re-use this temp variable in a line-by-line transformation op
			_locusPoint = transform.InverseTransformPoint(_locusPoint);
			gridPoint = new IntVector3(Mathf.RoundToInt( _locusPoint.x), Mathf.RoundToInt( _locusPoint.y), Mathf.RoundToInt( _locusPoint.z));

			if (GetTile(gridPoint.x, gridPoint.y, gridPoint.z) != null) {
				if (Event.current.keyCode == KeyCode.C) {
					GetTile(gridPoint.x, gridPoint.y, gridPoint.z).modality = Tile.Modalities.Empty;
				} else if (Event.current.keyCode == KeyCode.V) {
					GetTile(gridPoint.x, gridPoint.y, gridPoint.z).modality = Tile.Modalities.Floor;
				} else if (Event.current.keyCode == KeyCode.B) {
					GetTile(gridPoint.x, gridPoint.y, gridPoint.z).modality = Tile.Modalities.Filled;
				}
			}
			#endif
		}

		public Tile GetTile (int _x, int _y, int _z) {
			Tile _ret = null;
			if (_x >= 0 && _y >= 0 && _z >= 0) {
				if (_x < tiles.GetLength(0) && _y < tiles.GetLength(1) && _z < tiles.GetLength(2)) {
					_ret = tiles[_x,_y,_z];
				}
			}
			return _ret;
		}

		public void ClearTile (Tile _tile) {
			if (_tile.rootObj != null)
				DestroyImmediate(_tile.rootObj);
			if (_tile.fWall != null)
				DestroyImmediate(_tile.fWall);
			if (_tile.bWall != null)
				DestroyImmediate(_tile.bWall);
			if (_tile.lWall != null)
				DestroyImmediate(_tile.lWall);
			if (_tile.rWall != null)
				DestroyImmediate(_tile.rWall);
			if (_tile.fill != null)
				DestroyImmediate(_tile.fill);
			if (_tile.floor != null)
				DestroyImmediate(_tile.floor);
			if (_tile.ceiling!= null)
				DestroyImmediate(_tile.ceiling);
		}

		public bool CheckIsBorderTile(Tile _tile){
			bool _ret = false;

			if (_tile.position.x == 0 || _tile.position.x == (size.x - 1))
				_ret = true;
			if (_tile.position.y == 0 || _tile.position.y == (size.y - 1))
				_ret = true;
			if (_tile.position.z == 0 || _tile.position.z == (size.z - 1))
				_ret = true;
			
			return _ret;
		}

		public bool CheckIsEnclosed (Tile _tile) {
			bool _ret = true;
			bool f, b, l, r, u, d = false;
			f = (tiles[_tile.position.x, _tile.position.y, _tile.position.z + 1].modality == Tile.Modalities.Filled);
			b = (tiles[_tile.position.x, _tile.position.y, _tile.position.z - 1].modality == Tile.Modalities.Filled);
			l = (tiles[_tile.position.x - 1, _tile.position.y, _tile.position.z].modality == Tile.Modalities.Filled);
			r = (tiles[_tile.position.x + 1, _tile.position.y, _tile.position.z].modality == Tile.Modalities.Filled);
			u = (tiles[_tile.position.x, _tile.position.y + 1, _tile.position.z].modality == Tile.Modalities.Filled);
			d = (tiles[_tile.position.x, _tile.position.y - 1, _tile.position.z].modality == Tile.Modalities.Filled);

			if (!f)
				_ret = false;
			if (!b)
				_ret = false;
			if (!l)
				_ret = false;
			if (!r)
				_ret = false;
			if (!u)
				_ret = false;
			if (!d)
				_ret = false;

			return _ret;
		}

		public bool CheckIsIsland (Tile _tile) {
			bool _ret = true;
			bool f, b, l, r, u, d = false;
			f = (tiles[_tile.position.x, _tile.position.y, _tile.position.z + 1].modality == Tile.Modalities.Filled);
			b = (tiles[_tile.position.x, _tile.position.y, _tile.position.z - 1].modality == Tile.Modalities.Filled);
			l = (tiles[_tile.position.x - 1, _tile.position.y, _tile.position.z].modality == Tile.Modalities.Filled);
			r = (tiles[_tile.position.x + 1, _tile.position.y, _tile.position.z].modality == Tile.Modalities.Filled);
			u = (tiles[_tile.position.x, _tile.position.y + 1, _tile.position.z].modality == Tile.Modalities.Filled);
			d = (tiles[_tile.position.x, _tile.position.y - 1, _tile.position.z].modality == Tile.Modalities.Filled);

			if (f)
				_ret = false;
			if (b)
				_ret = false;
			if (l)
				_ret = false;
			if (r)
				_ret = false;
			if (u)
				_ret = false;
			if (d)
				_ret = false;

			return _ret;
		}
	}
}




















