using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class RangeSpawner : MultiModule {

		[Tooltip("The prefab we're spawning")]
		public GameObject spawnableObject;
		[Tooltip("Tag of the object we wish to spawn near. Leave blank to use the object that this component is attached to.")]
		public string targetTag = "";
		[Tooltip("how many objects should we spawn?")]
		public int numberToSpawn = 5;
		[Tooltip("Should we spawn the objects as soon as it is created?")]
		public bool spawnOnStart = true;
		[Tooltip("how far should the objects be from each other minimum?")]
		public float objectSeparation = 1;//
		[Tooltip("How far should the objects be from the object that they are being spawned around?")]
		public float rootSeparation = 1.5f;//
		[Tooltip("How far on the x and z axes should we try to place the objects from the object that they are being spawned around?")]
		public float xzPlanarRange = 3f;//
		[Tooltip("How many times can we try to place the objects? We will stop trying to place any if we run out of Iterations.")]
		public int maxIterations = 30;

		private List<GameObject> spawnableObjects = new List<GameObject>();//a private list containing all of the objects
		private GameObject targetObject;
		private float currentSeparation = 0;
		private Vector3 placementPoint = Vector3.zero;
		private int currentIterations = 0;

		private int flipFlopX = 1;
		private int flipFlopZ = 1;

		private void OnEnable() {
			if (spawnOnStart)
				SpawnObjects();
		}

		public MessageHelp spawnObjectsHelp = new MessageHelp("SpawnObjects", "Instantly tries to spawn objects around the closest object defined by the 'Target Tag'");
		public void SpawnObjects() {
			if (!string.IsNullOrEmpty(targetTag))
				targetObject = FindClosestByTag(targetTag);//GameObject.FindGameObjectWithTag(playerTag);
			else
				targetObject = gameObject;
			if (targetObject == null) {
				Debug.LogError("Range Spawner " + gameObject.name + " requires a targetable object in the scene!");
				//enabled = false;
				return;
			}

			for (int i = 0; i < numberToSpawn; i++) {
				if (Random.Range(0f, 1f) < .5f) {
					flipFlopX *= -1;
				}
				if (Random.Range(0f, 1f) < .5f) {
					flipFlopZ *= -1;
				}
				placementPoint = new Vector3(targetObject.transform.position.x + Random.Range(-(xzPlanarRange * flipFlopX), (xzPlanarRange * flipFlopX)), targetObject.transform.position.y + .1f, targetObject.transform.position.z + Random.Range(-(xzPlanarRange * flipFlopZ), (xzPlanarRange * flipFlopZ)));

				while (currentIterations < maxIterations) {
					placementPoint = new Vector3(targetObject.transform.position.x + Random.Range(-(xzPlanarRange * flipFlopX), (xzPlanarRange * flipFlopX)), targetObject.transform.position.y + .1f, targetObject.transform.position.z + Random.Range(-(xzPlanarRange * flipFlopZ), (xzPlanarRange * flipFlopZ)));
					currentIterations++;
					if (FindClosestObjectSeparation(placementPoint) >= objectSeparation && Vector3.Distance(placementPoint, targetObject.transform.position) >= rootSeparation)
						break;
				}

				GameObject _newObject = Instantiate(spawnableObject, placementPoint, new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
				spawnableObjects.Add(_newObject);
				_newObject.transform.Rotate(Vector3.up, Random.Range(0, 360));
			}
		}

		float FindClosestObjectSeparation(Vector3 _placementPoint) {
			float ret = Mathf.Infinity;

			if (spawnableObjects.Count <= 0)
				return Mathf.Infinity;

			foreach (GameObject _bone in spawnableObjects) {
				if (Vector3.Distance(_bone.transform.position, _placementPoint) < ret)
					ret = Vector3.Distance(_bone.transform.position, _placementPoint);
			}
			return ret;
		}



	}
}