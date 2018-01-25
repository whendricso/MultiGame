using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

//	[AddComponentMenu("MultiGame/General/Unit Spawner")]
	public class UnitSpawner : MultiModule {
		
		public enum Modes {Periodic, TriggerEnter, RadiusBehind};
		public Modes mode = Modes.Periodic;	
		public float radius = 30.0f;
		public bool spawnOnce = false;
		public GameObject unit;
		[ReorderableAttribute]
		public GameObject[] spawnPoints;
		public float spawnDelay = 30.0f;
		public bool usePrefabTagAndLayer = true;
		public string unitTag = "Enemy";
		public int unitLayer = 0;
		public int maxUnits = 100;

		public HelpInfo help = new HelpInfo("This component has been deprecated, there are new systems that can do the same thing better, but since it's convenient to use" +
			" it's still included with MultiGame" +
			"\nThis component is designed to spawn enemies based on the player's position relative to it. You can attach a trigger to the object and set the mode to " +
			"\"TriggerEnter\" and units will spawn when the player enters the trigger. Player must be tagged \"Player\" for this to work." +
			"\n \"RadiusBehind\" mode spawns units only if the player is within a given radius but also facing away from this object's origin.");
		
		void Start () {
			if (unit ==  null) {
				Debug.LogError("Unit Spawner requires a Unit to spawn!");
				enabled = false;
				return;
			}
			if (spawnDelay > 0 && mode == Modes.Periodic)
				StartCoroutine(PeriodicSpawn(spawnDelay));
		}
		
		void OnTriggerEnter (Collider other) {
			if (GetNumUnits() > maxUnits)
				return;
			if (mode != Modes.TriggerEnter)
				return;
			if (other.gameObject.tag != "Player")
				return;
			StartCoroutine(PeriodicSpawn(0.0f));
		}
		
		IEnumerator PeriodicSpawn(float delay) {
			yield return new WaitForSeconds(delay);
			GameObject plyr = GameObject.FindGameObjectWithTag("Player");
			if (plyr == null) {
				StopAllCoroutines();
				StartCoroutine(PeriodicSpawn(spawnDelay));
			}
			else {
		 		float playerDot = Vector3.Dot(transform.eulerAngles.normalized, plyr.transform.eulerAngles.normalized);
				if ((mode == Modes.RadiusBehind && Vector3.Distance(transform.position, plyr.transform.position) > radius)) {
					spawnDelay = 0.0f;
				}
				else {
					if (GetNumUnits() < maxUnits) {
						if (spawnPoints.Length > 0) {
							if (!(mode == Modes.RadiusBehind && playerDot > 0.0f))  {
								foreach (GameObject spawnPoint in spawnPoints) {
									GameObject spawned = Instantiate(unit, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
									if (!usePrefabTagAndLayer) {
										spawned.tag = unitTag;
										spawned.layer = unitLayer;
									}
								}
							}
						}
						else {
							if (!(mode == Modes.RadiusBehind && playerDot > 0.0f))  {
								GameObject spawned = Instantiate(unit, transform.position, transform.rotation) as GameObject;
								if (!usePrefabTagAndLayer) {
									spawned.tag = unitTag;
									spawned.layer = unitLayer;
								}
							}
						}
					}
					if (spawnOnce)
						Destroy(gameObject);
					else {
						if (delay > 0)
								StartCoroutine(PeriodicSpawn(spawnDelay));
					}
						
				}
			}
		}
		
		int GetNumUnits () {
			return GameObject.FindGameObjectsWithTag(unitTag).Length;
		}
	}
}