using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class TerraVolLocalSerializer : MonoBehaviour {

		public string mapName;
		TerraMap terraMap;

		void Start () {
			if (string.IsNullOrEmpty(mapName)) {
				mapName = Application.loadedLevelName;
			}
			if (terraMap == null)
				terraMap = GameObject.FindObjectOfType<TerraMap>();
		}

		public void Save (string _mapName) {
			if (string.IsNullOrEmpty(_mapName))
				_mapName = mapName;

			try {
				terraMap.Save(Application.persistentDataPath + _mapName);
			}
			catch {
				Debug.LogError("TerraVol Local Serializer failed to save the map!" + _mapName);
			}
		}

		public void Load (string _mapName) {
			if (string.IsNullOrEmpty(_mapName))
				_mapName = mapName;
			try {
				terraMap.Load(Application.persistentDataPath + _mapName);
			}
			catch {
				Debug.LogError("TerraVol Local Serializer failed to load the map! " + _mapName);
			}
			
		}
	}
}